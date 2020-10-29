using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MonoBehaviour
{
    private static Vector3[] positions = new Vector3[] { new Vector3(2, 0, 1), new Vector3(2, 0, -1), new Vector3(-2, 0, -1), new Vector3(-2, 0, 1), new Vector3(-1, 0, -2), new Vector3(1, 0, 2), new Vector3(1, 0, -2), new Vector3(-1, 0, 2) };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static List<KeyValuePair<Vector3, MoveType>> KnightMovement(Vector3 position, Board board, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;


        foreach (Vector3 pos in positions)
        {
            moveType = board.CheckMove(position + pos, piece);
            if (moveType != MoveType.Blocked)
            {
                movements.Add(new KeyValuePair<Vector3, MoveType>(position + pos, moveType));
            }
        }

        return movements;
    }

    void OnMouseDown()
    {
        if (GetComponent<Piece>().side == GameMaster.instance.turn)
        {
            if (GameMaster.instance.selectedPiece == gameObject)
            {
                GameMaster.instance.DeleteMoves();
                return;
            }
            else
            {
                var movements = KnightMovement(transform.position, GameMaster.instance.board, this.gameObject);
                if (movements.Count != 0)
                {
                    GameMaster.instance.CreateMoves(movements, this.gameObject);
                }
            }
        }
    }
}
