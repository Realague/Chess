using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<KeyValuePair<Vector3, MoveType>> RookMovement(Vector3 position, Dictionary<Vector3, GameObject> pieces, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;

        for (int i = 1; moveType != MoveType.Blocked; i++)
        {
            moveType = PieceMovementHelper.CheckMove(position + new Vector3(i, 0, 0), pieces, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position, moveType));
            }
        }
        for (int i = 1; moveType != MoveType.Blocked; i++)
        {
            moveType = PieceMovementHelper.CheckMove(position + new Vector3(-i, 0, 0), pieces, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position, moveType));
            }
        }
        for (int i = 1; moveType != MoveType.Blocked; i++)
        {
            moveType = PieceMovementHelper.CheckMove(position + new Vector3(0, 0, i), pieces, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position, moveType));
            }
        }
        for (int i = 1; moveType != MoveType.Blocked; i++)
        {
            moveType = PieceMovementHelper.CheckMove(position + new Vector3(0, 0, -i), pieces, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position, moveType));
            }
        }
        return movements;
    }

    void OnMouseDown()
    {
        if (GameMaster.instance.selectedPiece == gameObject)
        {
            GameMaster.instance.DeleteMoves();
            return;
        }
        else
        {
            var movements = RookMovement(transform.position, GameMaster.instance.pieces, this.gameObject);
            if (movements.Count != 0)
            {
                GameMaster.instance.CreateMoves(movements, this.gameObject);
            }
        }
    }
}
