using System.Collections;
using System.Collections.Generic;
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

    public void DoMovementVirtually(Board board)
    {

    }

    public void DoMovement(Board board, bool isVirtual)
    {
        switch (moveType)
        {
            case MoveType.Move:
                board.MovePiece(position, piece);
                if (!isVirtual)
                {
                    piece.transform.position = position;
                }
                break;
            case MoveType.MovePawn:
                board.MovePiece(position, piece);
                HandlePawnFinish(position, board, isVirtual);
                if (!isVirtual)
                {
                    piece.transform.position = position;
                }
                break;
            case MoveType.Attack:
                if (!isVirtual)
                {
                    GameMaster.instance.DeletePiece(position);
                    piece.transform.position = position;
                } else
                {
                    board.RemovePiece(position);
                    board.MovePiece(position, piece);
                }
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
            board.MovePiece(new Vector3(2, 0, z), piece);
            GameObject king = board.CheckCase(position);
            board.MovePiece(new Vector3(1, 0, z), king);
            if (!isVirtual)
            {
                piece.transform.position = new Vector3(2, 0, z);
                king.transform.position = new Vector3(1, 0, z);
            }
        }
        else if (piece.transform.position.x == 7)
        {
            board.MovePiece(new Vector3(5, 0, z), piece);
            GameObject king = board.CheckCase(position);
            board.MovePiece(new Vector3(6, 0, z), king);

            if (!isVirtual)
            {
                piece.transform.position = new Vector3(5, 0, z);
                king.transform.position = new Vector3(6, 0, z);
            }
        }

        GameMaster.instance.DeleteMoves();
    }

    private void HandlePawnFinish(Vector3 position, Board board,  bool isVirtual)
    {
        if (piece.GetComponent<Piece>().side == Color.Light && position.z == 0)
        {
            if (isVirtual)
            {
                board.RemovePiece(position);
                board.AddPiece(GameMaster.Instantiate(GameMaster.queenLight, new Vector3(-100, -100, -100), new Quaternion(0, 180, 0, 0)));
            }
            else
            {
               GameMaster.instance.DeletePiece(position);
               board.AddPiece(GameMaster.Instantiate(GameMaster.queenLight, position, new Quaternion(0, 180, 0, 0)));
            }
        }
        else if (piece.GetComponent<Piece>().side == Color.Dark && position.z == 7)
        {
            if (isVirtual)
            {
                board.RemovePiece(position);
                board.AddPiece(GameMaster.Instantiate(GameMaster.queenDark, new Vector3(-100, -100, -100), new Quaternion(0, 180, 0, 0)));
            } else
            {
                GameMaster.instance.DeletePiece(position);
                board.AddPiece(GameMaster.Instantiate(GameMaster.queenDark, position, new Quaternion(0, 0, 0, 0)));
            }

        }
    }

}