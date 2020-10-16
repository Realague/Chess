using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovementHelper
{
    public static GameObject CheckCase(Vector3 position, Dictionary<Vector3, GameObject> pieces)
    {
        if (pieces.ContainsKey(position))
        {
            return pieces[position];
        }
        return null;
    }

    public static MoveType CheckMove(Vector3 position, Dictionary<Vector3, GameObject> pieces, GameObject piece)
    {
        if (position.x >= 0 && position.x < 8 && position.z >= 0 && position.z < 8)
        {
            GameObject result = CheckCase(position, pieces);
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
}
