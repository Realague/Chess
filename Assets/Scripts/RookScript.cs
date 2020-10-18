using System.Collections.Generic;
using UnityEngine;

public class RookScript : MonoBehaviour
{

    public bool isFirstMove = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<KeyValuePair<Vector3, MoveType>> RookMovement(Vector3 position, Board board, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;

        for (int i = 1; moveType == MoveType.Move; i++)
        {
            moveType = board.CheckMove(position + new Vector3(i, 0, 0), piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(i, 0, 0), moveType));
            } else if (board.CheckCastling(position + new Vector3(i, 0, 0), piece))
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(i, 0, 0), MoveType.Castling));
            }
        }
        moveType = MoveType.Move;
        for (int i = 1; moveType == MoveType.Move; i++)
        {
            moveType = board.CheckMove(position + new Vector3(-i, 0, 0), piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(-i, 0, 0), moveType));
            } else if (board.CheckCastling(position + new Vector3(-i, 0, 0), piece))
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(-i, 0, 0), MoveType.Castling));
            }
        }
        moveType = MoveType.Move;
        for (int i = 1; moveType == MoveType.Move; i++)
        {
            moveType = board.CheckMove(position + new Vector3(0, 0, i), piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, i), moveType));
            }
        }
        moveType = MoveType.Move;
        for (int i = 1; moveType == MoveType.Move; i++)
        {
            moveType = board.CheckMove(position + new Vector3(0, 0, -i), piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, -i), moveType));
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
            var movements = RookMovement(transform.position, GameMaster.instance.board, this.gameObject);
            if (movements.Count != 0)
            {
                GameMaster.instance.CreateMoves(movements, this.gameObject);
            }
        }
    }
}
