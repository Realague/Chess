using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<KeyValuePair<Vector3, MoveType>> QueenMovement(Vector3 position, Dictionary<Vector3, GameObject> pieces, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        movements = RookScript.RookMovement(position, pieces, piece);
        movements.AddRange(BishopScript.BishopMovement(position, pieces, piece));
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
            var movements = QueenMovement(transform.position, GameMaster.instance.pieces, this.gameObject);
            if (movements.Count != 0)
            {
                GameMaster.instance.CreateMoves(movements, this.gameObject);
            }
        }
    }
}
