using UnityEngine;

public class MovePieces : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
       new Movement(transform.position, GameMaster.instance.selectedPiece,
             GameMaster.instance.selectedPiece.GetComponentInParent<Piece>().type == PieceType.Pawn ? MoveType.MovePawn : MoveType.Move)
            .DoMovement(GameMaster.instance.board, false);
    }
}
