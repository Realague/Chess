using UnityEngine;

public class Movement
{
    public Vector2 position;

    public VirtualPiece piece;

    public MoveType moveType;

    public VirtualPiece pieceAttacked;

    public VirtualPiece queen = null;

    public Movement(Vector2 position, VirtualPiece piece, MoveType moveType, VirtualPiece pieceAttacked)
    {
        this.position = position;
        this.piece = piece;
        this.moveType = moveType;
        this.pieceAttacked = pieceAttacked;
    }

    public Movement(Movement move) {
        this.position = move.position;
        this.piece = new VirtualPiece(move.piece);
        this.moveType = move.moveType;
        if (move.pieceAttacked != null) {
            this.pieceAttacked = new VirtualPiece(move.pieceAttacked);
        } else {
            this.pieceAttacked = null;
        }
    }

    public void DoMovement(Board board, bool isVirtual)
    {
        switch (moveType)
        {
            case MoveType.Move:
                if (!isVirtual) {
                    GameMaster.instance.Move(piece.position, position);
                }
                board.MovePiece(position, piece.position, piece);
                break;
            case MoveType.MovePawn:
                if (!isVirtual) {
                    GameMaster.instance.Move(piece.position, position);
                }
                board.MovePiece(position, piece.position);
                HandlePawnFinish(position, board, isVirtual);
                break;
            case MoveType.Attack:
                if (!isVirtual)
                {
                    GameMaster.instance.DeletePiece(position);
                    GameMaster.instance.Move(piece.position, position);
                } else
                {
                    board.RemovePiece(position);
                }
                board.MovePiece(position, piece.position);
                HandlePawnFinish(position, board, isVirtual);
                break;
            case MoveType.Castling:
                HandleCastling(board, isVirtual);
                break;
            default:
                break;
        }

        if (!isVirtual)
        {
            GameMaster.instance.EndTurn();
        } /*else {
            board.previousMove.Push(new Movement(this));
        }*/
    }

    private void HandleCastling(Board board, bool isVirtual)
    {
        float y = piece.position.y;
        if (piece.position.x == 0)
        {
            VirtualPiece king = board.CheckCase(position);
            if (!isVirtual) {
                GameMaster.instance.Move(king.position, new Vector2(1, y));
                GameMaster.instance.Move(piece.position, new Vector2(2, y));
            }
            board.MovePiece(new Vector2(1, y), king.position);
            board.MovePiece(new Vector2(2, y), piece.position);
        } else if (piece.position.x == 7)
        {
            VirtualPiece king = board.CheckCase(position);
            if (!isVirtual) {
                GameMaster.instance.Move(king.position, new Vector2(6, y));
                GameMaster.instance.Move(piece.position, new Vector2(5, y));
            }
            board.MovePiece(new Vector2(6, y), king.position);
            board.MovePiece(new Vector2(5, y), piece.position);
        }
    }

    private void HandlePawnFinish(Vector2 position, Board board,  bool isVirtual)
    {
        if (piece.type == PieceType.Pawn) {
            if (piece.side == Color.Light && position.y == 0) {
                board.RemovePiece(position);
                if (!isVirtual) {
                    GameMaster.instance.DeletePiece(position);
                    GameMaster.instance.piecesObject.Add(position, GameMaster.Instantiate(GameMaster.instance.queenLight, Utils.Vector2ToVector3(position), new Quaternion(0, 180, 0, 0)));
                }
                board.AddPiece(new VirtualPiece(PieceType.Queen, Color.Light, position, true));
                queen = board.pieces[(int)position.x, (int)position.y];
            } else if (piece.side == Color.Dark && position.y == 7) {
                board.RemovePiece(position);
                if (!isVirtual) {
                    GameMaster.instance.DeletePiece(position);
                    GameMaster.instance.piecesObject.Add(position, GameMaster.Instantiate(GameMaster.instance.queenDark, Utils.Vector2ToVector3(position), new Quaternion(0, 0, 0, 0)));
                }
                board.AddPiece(new VirtualPiece(PieceType.Queen, Color.Dark, position, true));
                queen = board.pieces[(int)position.x, (int)position.y];
            }
        }
    }

}