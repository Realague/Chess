using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public Dictionary<Vector3, GameObject> pieces;

    private static int depth = 0;

    public Board(List<GameObject> lightPieces, List<GameObject> darkPieces)
    {
        pieces = new Dictionary<Vector3, GameObject>();

        int i = 0;
        foreach (GameObject piece in darkPieces)
        {
            pieces.Add(new Vector3(i % 8, 0, i / 8), piece);
            i++;
        }

        i = 0;
        foreach (GameObject piece in lightPieces)
        {
            pieces.Add(new Vector3(i % 8, 0, 7 - i / 8), piece);
            i++;
        }
    }

    public Board(Board board)
    {
        pieces = new Dictionary<Vector3, GameObject>(board.pieces);
    }

    public void MovePiece(Vector3 position, GameObject piece)
    {
        pieces[position] = pieces[piece.transform.position];
        pieces.Remove(piece.transform.position);
    }

    public GameObject CheckCase(Vector3 position)
    {
        if (pieces.ContainsKey(position))
        {
            return pieces[position];
        }
        return null;
    }

    //Determine the move type
    public MoveType CheckMove(Vector3 position, GameObject piece)
    {
        if (position.x >= 0 && position.x < 8 && position.z >= 0 && position.z < 8)
        {
            GameObject result = CheckCase(position);
            if (result == null)
            {
                return MoveType.Move;
            }
            else if (result.GetComponent<Piece>().side != piece.GetComponent<Piece>().side)
            {
                return MoveType.Attack;
            }
        }
        return MoveType.Blocked;
    }

    //Check if castling is possible
    public bool CheckCastling(Vector3 position, GameObject rook)
    {
        GameObject king = CheckCase(position);
        if (king == null)
        {
            return false;
        }

        Piece kingPiece = king.GetComponent<Piece>();
        Piece rookPiece = rook.GetComponent<Piece>();

        if (rook.GetComponent<Piece>().type == PieceType.Rook && king.GetComponent<Piece>().type == PieceType.King
            && kingPiece.side == rookPiece.side && kingPiece.isFirstMove && rookPiece.isFirstMove)
        {
            return true;
        }
        return false;
    }

    public List<KeyValuePair<Vector3, MoveType>> GetMovementsByPieceType(GameObject piece)
    {
        depth++;
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        switch (piece.GetComponent<Piece>().type)
        {
            case PieceType.Bishop:
                movements = BishopMovement(piece.transform.position, piece);
                break;
            case PieceType.King:
                movements = KingMovement(piece.transform.position, piece);
                break;
            case PieceType.Pawn:
                movements = PawnMovement(piece.transform.position, piece);
                break;
            case PieceType.Knight:
                movements = KnightMovement(piece.transform.position, piece);
                break;
            case PieceType.Queen:
                movements = QueenMovement(piece.transform.position, piece);
                break;
            case PieceType.Rook:
                movements = RookMovement(piece.transform.position, piece);
                break;
            default:
                break;
        }

        if (depth < 2)
        {
            movements = RemoveCheck(piece, movements);
            depth--;
            return movements;
        }
        else
        {
            depth--;
            return movements;
        }
    }

    public List<KeyValuePair<Vector3, MoveType>> RemoveCheck(GameObject piece, List<KeyValuePair<Vector3, MoveType>> movements)
    {
        if (piece.GetComponent<Piece>().type == PieceType.King)
        {
            return movements;
        }

        for (int i = 0; i != movements.Count; i++)
        {
            Board board = new Board(this);
            if (movements[i].Value == MoveType.Attack)
            {
                board.pieces.Remove(movements[i].Key);
            }
            board.MovePiece(movements[i].Key, piece);
            if (board.Check())
            {
                movements.RemoveAt(i);
                i--;
            }
        }
        return movements;
    }

    public bool CheckAttack(Vector3 position, Color side, string d)
    {
        foreach (GameObject piece in pieces.Values)
        {
            if (piece != null && piece.GetComponent<Piece>().side != side)
            {
                foreach (KeyValuePair<Vector3, MoveType> movement in GetMovementsByPieceType(piece))
                {
                    if (movement.Key == position && movement.Value != MoveType.MovePawn)
                    {
                        return true;
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

        foreach (GameObject piece in pieces.Values)
        {
            Piece pieceInfo = piece.GetComponent<Piece>();
            if (pieceInfo.side == side && pieceInfo.type == PieceType.King)
            {
                return CheckAttack(piece.transform.position, side, "Check");
            }
        }
        return true;
    }

    public List<KeyValuePair<Vector3, MoveType>> BishopMovement(Vector3 position, GameObject piece)
    {
        Vector2[] vectors = new Vector2[] { new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();

        foreach (Vector2 vector2 in vectors)
        {
            MoveType moveType = MoveType.Move;
            for (int i = 1; moveType == MoveType.Move; i++)
            {
                moveType = CheckMove(position + new Vector3(i * vector2.x, 0, i * vector2.y), piece);
                if (moveType != MoveType.Blocked)
                {
                    movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(i * vector2.x, 0, i * vector2.y), moveType));
                }
            }
        }
        return movements;
    }

    public List<KeyValuePair<Vector3, MoveType>> KingMovement(Vector3 position, GameObject piece)
    {
        Vector3[] positions = new Vector3[] { new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;

        foreach (Vector3 pos in positions)
        {
            moveType = CheckMove(position + pos, piece);
            Board boardCopy = new Board(this);
            if (moveType == MoveType.Attack)
            {
                boardCopy.pieces.Remove(position + pos);
            }
            boardCopy.MovePiece(position + pos, piece);
            if (moveType != MoveType.Blocked && (depth >= 2 || !boardCopy.CheckAttack(position + pos, piece.GetComponent<Piece>().side, "King")))
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + pos, moveType));
            }
        }

        return movements;
    }

    public List<KeyValuePair<Vector3, MoveType>> KnightMovement(Vector3 position, GameObject piece)
    {
        Vector3[] positions = new Vector3[] { new Vector3(2, 0, 1), new Vector3(2, 0, -1), new Vector3(-2, 0, -1), new Vector3(-2, 0, 1), new Vector3(-1, 0, -2), new Vector3(1, 0, 2), new Vector3(1, 0, -2), new Vector3(-1, 0, 2) };
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;

        foreach (Vector3 pos in positions)
        {
            moveType = CheckMove(position + pos, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + pos, moveType));
            }
        }

        return movements;
    }

    public List<KeyValuePair<Vector3, MoveType>> PawnMovement(Vector3 position, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;
        int multiplier = 1;

        if (piece.GetComponent<Piece>().side == Color.Light)
        {
            multiplier = -1;
        }

        moveType = CheckMove(position + new Vector3(0, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Move)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, 1 * multiplier), MoveType.MovePawn));
            if (piece.GetComponent<Piece>().isFirstMove)
            {
                moveType = CheckMove(position + new Vector3(0, 0, 2 * multiplier), piece);
                if (moveType != MoveType.Blocked && moveType != MoveType.Attack)
                {
                    movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, 2 * multiplier), MoveType.MovePawn));
                }
            }
        }

        moveType = CheckMove(position + new Vector3(1, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(1, 0, 1 * multiplier), moveType));
        }

        moveType = CheckMove(position + new Vector3(-1, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(-1, 0, 1 * multiplier), moveType));
        }

        return movements;
    }

    public List<KeyValuePair<Vector3, MoveType>> QueenMovement(Vector3 position, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        movements = RookMovement(position, piece);
        //printMovements(movements);
        movements.AddRange(BishopMovement(position, piece));
        return movements;
    }

    public static void printMovements(List<KeyValuePair<Vector3, MoveType>> moves)
    {
        foreach (KeyValuePair<Vector3, MoveType> keyValuePair in moves)
        {
            Debug.Log(keyValuePair.Key + " " + keyValuePair.Value + "\n");
        }
    }

    public List<KeyValuePair<Vector3, MoveType>> RookMovement(Vector3 position, GameObject piece)
    {
        Vector2[] vectors = new Vector2[] { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();

        foreach (Vector2 vector2 in vectors)
        {
            MoveType moveType = MoveType.Move;
            for (int i = 1; moveType == MoveType.Move; i++)
            {
                moveType = CheckMove(position + new Vector3(i * vector2.x, 0, i * vector2.y), piece);
                if (moveType != MoveType.Blocked)
                {
                    movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(i * vector2.x, 0, i * vector2.y), moveType));
                }
                else if (CheckCastling(position + new Vector3(i * vector2.x, 0, i * vector2.y), piece))
                {
                    movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(i * vector2.x, 0, i * vector2.y), MoveType.Castling));
                }
            }
        }

        return movements;
    }

}
