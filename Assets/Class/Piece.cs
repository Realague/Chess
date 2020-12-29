using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    public PieceType type;

    [SerializeField]
    public Color side;

    public bool isFirstMove = true;

    void OnMouseDown()
    {
        if (side == GameMaster.instance.turn)
        {
            if (GameMaster.instance.selectedPiece == gameObject)
            {
                GameMaster.instance.DeleteMoves();
                return;
            }
            else
            {
                Piece piece = this.GetComponent<Piece>();
                var movements = GameMaster.instance.board.GetMovementsByPieceType(GameMaster.instance.board.pieces[(int)this.transform.position.x, (int)this.transform.position.z]);
                if (movements.Count != 0)
                {
                    GameMaster.instance.CreateMoves(movements, this.gameObject);
                }
            }
        }
    }

}
