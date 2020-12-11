using UnityEngine;

public class Movement
{
    public Vector3 position;

    public GameObject piece;

    public MoveType moveType;

    public Movement(Vector3 position, GameObject piece, MoveType moveType)
    {
        this.position = position;
        this.piece = piece;
        this.moveType = moveType;
    }

    public void DoMovement(Board board, bool isVirtual)
    {
        switch (moveType)
        {
            case MoveType.Move:
                board.MovePiece(position, piece, isVirtual);
                break;
            case MoveType.MovePawn:
                board.MovePiece(position, piece, isVirtual);
                HandlePawnFinish(position, board, isVirtual);
                break;
            case MoveType.Attack:
                if (!isVirtual)
                {
                    GameMaster.instance.DeletePiece(position);
                } else
                {
                    board.RemovePiece(position);
                }
                board.MovePiece(position, piece, isVirtual);
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
        }
    }

    private void HandleCastling(Board board, bool isVirtual)
    {
        float z = piece.transform.position.z;
        if (piece.transform.position.x == 0)
        {
            board.MovePiece(new Vector3(2, 0, z), piece, isVirtual);
            GameObject king = board.CheckCase(position);
            board.MovePiece(new Vector3(1, 0, z), king, isVirtual);
        }
        else if (piece.transform.position.x == 7)
        {
            board.MovePiece(new Vector3(5, 0, z), piece, isVirtual);
            GameObject king = board.CheckCase(position);
            board.MovePiece(new Vector3(6, 0, z), king, isVirtual);
        }
    }

    private void HandlePawnFinish(Vector3 position, Board board,  bool isVirtual)
    {
        if (piece.GetComponent<Piece>().type == PieceType.Pawn) {
            if (piece.GetComponent<Piece>().side == Color.Light && position.z == 0) {
                if (isVirtual) {
                    board.RemovePiece(position);
                    board.AddPiece(GameMaster.Instantiate(GameMaster.instance.queenLight, new Vector3(-100, -100, -100), new Quaternion(0, 180, 0, 0)));
                } else {
                    GameMaster.instance.DeletePiece(position);
                    board.AddPiece(GameMaster.Instantiate(GameMaster.instance.queenLight, position, new Quaternion(0, 180, 0, 0)));
                }
            } else if (piece.GetComponent<Piece>().side == Color.Dark && position.z == 7) {
                if (isVirtual) {
                    board.RemovePiece(position);
                    board.AddPiece(GameMaster.Instantiate(GameMaster.instance.queenDark, new Vector3(-100, -100, -100), new Quaternion(0, 180, 0, 0)));
                } else {
                    GameMaster.instance.DeletePiece(position);
                    board.AddPiece(GameMaster.Instantiate(GameMaster.instance.queenDark, position, new Quaternion(0, 0, 0, 0)));
                }
            }
        }
    }

}