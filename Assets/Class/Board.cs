using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public VirtualPiece[,] pieces = new VirtualPiece[8, 8];

    private static int depth = 0;

    public Stack<Movement> previousMove = new Stack<Movement>();

    public Board(Dictionary<Vector2, GameObject> piecesObject)
    {
        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                pieces[i, j] = null;
            }
        }

        foreach (KeyValuePair<Vector2, GameObject> entry in piecesObject) {
            pieces[(int)entry.Key.x, (int)entry.Key.y] = new VirtualPiece(entry.Value.GetComponent<Piece>(), entry.Key);
        }
    }

    public Board(Board board)
    {
        pieces = new VirtualPiece[8, 8];
        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                if (board.pieces[i, j] == null) {
                    pieces[i, j] = null;
                } else {
                    pieces[i, j] = new VirtualPiece(board.pieces[i, j]);
                }
            }
        }
    }

    public void AddPiece(Piece piece, Vector2 position)
    {
        pieces[(int)position.x, (int)position.y] = new VirtualPiece(piece, position);
    }

    public void AddPiece(VirtualPiece piece) {
        //Debug.Log("piece: " + piece.position);
        pieces[(int)piece.position.x, (int)piece.position.y] = piece;
    }

    public void RemovePiece(Vector2 position)
    {
        pieces[(int)position.x, (int)position.y] = null;
    }

    public void MovePiece(Vector2 position, Vector2 oldPosition, VirtualPiece pieceA) {
        pieces[(int)oldPosition.x, (int)oldPosition.y] = null;
        pieces[(int)position.x, (int)position.y] = new VirtualPiece(pieceA);
        pieces[(int)position.x, (int)position.y].position = position;
    }

    public void MovePiece(Vector2 position, Vector2 oldPosition) {
        //Debug.Log(position);
        //Debug.Log(oldPosition);
        VirtualPiece piece = pieces[(int)oldPosition.x, (int)oldPosition.y];
        //if (piece == null) {
          //  return;
        //}
        //Debug.Log(piece);
        pieces[(int)position.x, (int)position.y] = new VirtualPiece(piece);
        pieces[(int)oldPosition.x, (int)oldPosition.y] = null;
        pieces[(int)position.x, (int)position.y].position = position;
    }

    public VirtualPiece CheckCase(Vector2 position)
    {
        return pieces[(int)position.x, (int)position.y];
    }

    //Determine the move type
    public MoveType CheckMove(Vector2 position, VirtualPiece piece)
    {
        if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
        {
            VirtualPiece result = CheckCase(position);
            if (result == null)
            {
                return MoveType.Move;
            }
            else if (result.side != piece.side)
            {
                return MoveType.Attack;
            }
        }

        return MoveType.Blocked;
    }

    //Check if castling is possible
    public bool CheckCastling(Vector2 position, VirtualPiece rook)
    {
        VirtualPiece king = CheckCase(position);
        if (king == null)
        {
            return false;
        }

        if (rook.type == PieceType.Rook && king.type == PieceType.King && king.side == rook.side && king.isFirstMove && rook.isFirstMove)
        {
            return true;
        }
        return false;
    }

    public List<Movement> GetAllMovements(Color side)
    {
        List<Movement> movements = new List<Movement>();
        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                if (pieces[i, j] != null && side == pieces[i, j].side) {
                    movements.AddRange(GetMovementsByPieceType(pieces[i, j]));
                }
            }
        }
        
        return movements;
    }

    public List<Movement> GetMovementsByPieceType(VirtualPiece piece)
    {
        depth++;
        List<Movement> movements = new List<Movement>();
        switch (piece.type)
        {
            case PieceType.Bishop:
                movements = BishopMovement(piece.position, piece);
                break;
            case PieceType.King:
                movements = KingMovement(piece.position, piece);
                break;
            case PieceType.Pawn:
                movements = PawnMovement(piece.position, piece);
                break;
            case PieceType.Knight:
                movements = KnightMovement(piece.position, piece);
                break;
            case PieceType.Queen:
                movements = QueenMovement(piece.position, piece);
                break;
            case PieceType.Rook:
                movements = RookMovement(piece.position, piece);
                break;
            default:
                break;
        }

        if (depth < 2)
        {
           movements = RemoveCheck(piece, movements);
        }
        depth--;
        return movements;
    }

    public List<Movement> RemoveCheck(VirtualPiece piece, List<Movement> movements)
    {
        if (piece.type == PieceType.King)
        {
            return movements;
        }

        for (int i = 0; i != movements.Count; i++)
        {
            if (movements[i].moveType != MoveType.Castling)
            {
                previousMove.Push(movements[i]);
                if (movements[i].moveType == MoveType.Attack)
                {
                    RemovePiece(movements[i].position);
                }
                MovePiece(movements[i].position, piece.position);
                if (Check())
                {
                    movements.RemoveAt(i);
                    i--;
                }
                UndoMove();
            }
        }
        return movements;
    }

    public bool CheckAttack(Vector2 position, Color side)
    {
        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                if (pieces[i, j] != null && side != pieces[i, j].side) {
                    foreach (Movement movement in GetMovementsByPieceType(pieces[i, j])) {
                        if (movement.position == position && movement.moveType != MoveType.MovePawn) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    //Check if king is in danger
    public bool Check()
    {
        Color side = GameMaster.instance.turn;

        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                if (pieces[i, j] != null && side == pieces[i, j].side && pieces[i, j].type == PieceType.King) {
                    return CheckAttack(pieces[i, j].position, side);
                }
            }
        }
        return true;
    }

    public bool Check(Color side) {
        for (int i = 0; i != 8; i++) {
            for (int j = 0; j != 8; j++) {
                if (pieces[i, j] != null && side == pieces[i, j].side && pieces[i, j].type == PieceType.King) {
                    return CheckAttack(pieces[i, j].position, side);
                }
            }
        }
        return true;
    }

    public List<Movement> BishopMovement(Vector2 position, VirtualPiece piece)
    {
        Vector2[] vectors = new Vector2[] { new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
        List<Movement> movements = new List<Movement>();

        foreach (Vector2 vector2 in vectors)
        {
            MoveType moveType = MoveType.Move;
            for (int i = 1; moveType == MoveType.Move; i++)
            {
                Vector2 newPosition = position + new Vector2(i * vector2.x, i * vector2.y);
                moveType = CheckMove(newPosition, piece);
                if (moveType != MoveType.Blocked)
                {
                    movements.Add(new Movement(newPosition, new VirtualPiece(piece), moveType, moveType == MoveType.Attack ? CheckCase(newPosition) : null));
                }
            }
        }
        return movements;
    }

    public List<Movement> KingMovement(Vector2 position, VirtualPiece piece)
    {
        Vector2[] positions = new Vector2[] { new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };

        List<Movement> movements = new List<Movement>();
        MoveType moveType = MoveType.Move;

        foreach (Vector2 pos in positions)
        {
            moveType = CheckMove(position + pos, piece);
            if (moveType != MoveType.Blocked) {
                previousMove.Push(new Movement(position + pos, new VirtualPiece(piece), moveType, moveType == MoveType.Attack ? CheckCase(position + pos) : null));
                if (moveType == MoveType.Attack) {
                    RemovePiece(position + pos);
                }
                MovePiece(position + pos, piece.position);
                if (depth >= 2 || !CheckAttack(position + pos, piece.side)) {
                    movements.Add(new Movement(position + pos, new VirtualPiece(piece), moveType, moveType == MoveType.Attack ? CheckCase(position + pos) : null));
                }
                UndoMove();
            }
        }

        return movements;
    }

    public List<Movement> KnightMovement(Vector2 position, VirtualPiece piece)
    {
        Vector2[] positions = new Vector2[] { new Vector2(2, 1), new Vector2(2, -1), new Vector2(-2, -1), new Vector2(-2, 1), new Vector2(-1, -2), new Vector2(1, 2), new Vector2(1, -2), new Vector2(-1, 2) };
        List<Movement> movements = new List<Movement>();
        MoveType moveType = MoveType.Move;

        foreach (Vector2 pos in positions)
        {
            moveType = CheckMove(position + pos, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new Movement(position + pos, new VirtualPiece(piece), moveType, moveType == MoveType.Attack ? CheckCase(position + pos) : null));
            }
        }

        return movements;
    }

    public List<Movement> PawnMovement(Vector2 position, VirtualPiece piece)
    {
        List<Movement> movements = new List<Movement>();
        MoveType moveType = MoveType.Move;
        int multiplier = 1;

        if (piece.side == Color.Light)
        {
            multiplier = -1;
        }

        moveType = CheckMove(position + new Vector2(0, 1 * multiplier), piece);
        if (moveType == MoveType.Move)
        {
            movements.Add(new Movement(position + new Vector2(0, 1 * multiplier), new VirtualPiece(piece), MoveType.MovePawn, null));
            if (piece.isFirstMove)
            {
                moveType = CheckMove(position + new Vector2(0, 2 * multiplier), piece);
                if (moveType != MoveType.Blocked && moveType != MoveType.Attack)
                {
                    movements.Add(new Movement(position + new Vector2(0, 2 * multiplier), new VirtualPiece(piece), MoveType.MovePawn, null));
                }
            }
        }

        moveType = CheckMove(position + new Vector2(1, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new Movement(position + new Vector2(1, 1 * multiplier), new VirtualPiece(piece), moveType, CheckCase(position + new Vector2(1, 1 * multiplier))));
        }

        moveType = CheckMove(position + new Vector2(-1, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new Movement(position + new Vector2(-1, 1 * multiplier), new VirtualPiece(piece), moveType, CheckCase(position + new Vector2(-1, 1 * multiplier))));
        }

        return movements;
    }

    public List<Movement> QueenMovement(Vector2 position, VirtualPiece piece)
    {
        List<Movement> movements;
        movements = RookMovement(position, piece);
        movements.AddRange(BishopMovement(position, piece));
        return movements;
    }

    public List<Movement> RookMovement(Vector2 position, VirtualPiece piece)
    {
        Vector2[] vectors = new Vector2[] { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        List<Movement> movements = new List<Movement>();

        foreach (Vector2 vector2 in vectors)
        {
            MoveType moveType = MoveType.Move;
            for (int i = 1; moveType == MoveType.Move; i++)
            {
                Vector2 newPosition = position + new Vector2(i * vector2.x, i * vector2.y);
                if (newPosition.x >= 0 && newPosition.x < 8 && newPosition.y >= 0 && newPosition.y < 8) {
                    moveType = CheckMove(newPosition, piece);
                    if (moveType != MoveType.Blocked) {
                        movements.Add(new Movement(newPosition, new VirtualPiece(piece), moveType, moveType == MoveType.Attack ? CheckCase(newPosition) : null));
                    } else if (depth < 2 && CheckCastling(newPosition, piece) && !RemoveCheckCastling(piece)) {
                        movements.Add(new Movement(newPosition, new VirtualPiece(piece), MoveType.Castling, CheckCase(newPosition)));
                    }
                } else {
                    moveType = MoveType.Blocked;
                }
            }
        }
        return movements;
    }

    public bool RemoveCheckCastling(VirtualPiece piece)
    {
        float y = piece.position.y;
        if (piece.position.x == 0)
        {
            if (CheckAttack(new Vector2(1, y), piece.side) || CheckAttack(new Vector2(2, y), piece.side) ||
                (y == 0 && CheckAttack(new Vector2(3, y), piece.side)))
            {
                return true;
            }
        } else if (piece.position.x == 7)
        {
            if (CheckAttack(new Vector2(5, y), piece.side) || CheckAttack(new Vector2(6, y), piece.side) ||
                (y == 7 && CheckAttack(new Vector2(4, y), piece.side)))
            {
                return true;
            }
        }
        return false;
    }

    public void UndoMove() {
        Movement move = previousMove.Pop();
        if (move.moveType != MoveType.Castling && move.moveType != MoveType.Attack) {
             MovePiece(move.piece.position, move.position);
        } else if (move.moveType == MoveType.Attack) {
            MovePiece(move.piece.position, move.position);
            AddPiece(new VirtualPiece(move.pieceAttacked));
        } else if (move.moveType == MoveType.Castling) {
            float y = move.piece.position.y;
            if (move.piece.position.x == 0) {
                MovePiece(move.position, new Vector2(1, y));
                MovePiece(move.piece.position, new Vector2(2, y));
            } else if (move.piece.position.x == 7) {
                MovePiece(move.position, new Vector2(6, y));
                MovePiece(move.piece.position, new Vector2(5, y));
            }
        }
    }

}
