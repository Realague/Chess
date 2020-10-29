using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public Dictionary<Vector3, GameObject> pieces;

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

    public bool CheckCastling(Vector3 position, GameObject rook)
    {
        GameObject king = CheckCase(position);
        if (king == null)
        {
            return false;
        }

        Piece kingType = king.GetComponent<Piece>();
        Piece RookType = rook.GetComponent<Piece>();

        if (rook.GetComponent<Piece>().type == PieceType.Rook && king.GetComponent<Piece>().type == PieceType.King
            && kingType.side == RookType.side
            && king.GetComponent<KingScript>().isFirstMove && rook.GetComponent<RookScript>().isFirstMove)
        {
            return true;
        }
        return false;
    }

    public List<KeyValuePair<Vector3, MoveType>> GetMovementsByPieceType(GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        switch (piece.GetComponent<Piece>().type)
        {
            case PieceType.Bishop:
                movements = BishopScript.BishopMovement(piece.transform.position, this, piece);
                break;
            case PieceType.King:
                movements = KingScript.KingMovement(piece.transform.position, this, piece, false);
                break;
            case PieceType.Pawn:
                movements = PawnScript.PawnMovement(piece.transform.position, this, piece);
                break;
            case PieceType.Knight:
                movements = KnightScript.KnightMovement(piece.transform.position, this, piece);
                break;
            case PieceType.Queen:
                movements = QueenScript.QueenMovement(piece.transform.position, this, piece);
                break;
            case PieceType.Rook:
                movements = RookScript.RookMovement(piece.transform.position, this, piece);
                break;
            default:
                break;
        }
        return movements;
    }

    public bool CheckAttack(Vector3 position, Color side)
    {
        foreach (GameObject piece in pieces.Values)
        {
            if (piece != null && piece.GetComponent<Piece>().side != side)
            {
                foreach (KeyValuePair<Vector3, MoveType> pos in GetMovementsByPieceType(piece))
                {
                    if (pos.Key == position && pos.Value != MoveType.MovePawn)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
