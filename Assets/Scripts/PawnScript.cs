using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnScript : MonoBehaviour
{

    public bool isFirstMove;

    // Start is called before the first frame update
    void Start()
    {
        isFirstMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<KeyValuePair<Vector3, MoveType>> PawnMovement(Vector3 position, Board board, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        MoveType moveType = MoveType.Move;
        int multiplier = 1;

        if (piece.GetComponent<Piece>().side == Color.Light)
        {
            multiplier = -1;
        }

        moveType = board.CheckMove(position + new Vector3(0, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Move)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, 1 * multiplier), MoveType.MovePawn));
            if (piece.GetComponent<PawnScript>().isFirstMove)
            {
                moveType = board.CheckMove(position + new Vector3(0, 0, 2 * multiplier), piece);
                if (moveType != MoveType.Blocked && moveType != MoveType.Attack)
                {
                    movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(0, 0, 2 * multiplier), MoveType.MovePawn));
                }
            }
        }

        moveType = board.CheckMove(position + new Vector3(1, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(1, 0, 1 * multiplier), moveType));
        }

        moveType = board.CheckMove(position + new Vector3(-1, 0, 1 * multiplier), piece);
        if (moveType == MoveType.Attack)
        {
            movements.Add(new KeyValuePair<Vector3, MoveType>(position + new Vector3(-1, 0, 1 * multiplier), moveType));
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
            var movements = PawnMovement(transform.position, GameMaster.instance.board, this.gameObject);
            if (movements.Count != 0)
            {
                GameMaster.instance.CreateMoves(movements, this.gameObject);
            }
        }
    }

}
