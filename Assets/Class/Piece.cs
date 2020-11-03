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
                var movements = GameMaster.instance.board.GetMovementsByPieceType(this.gameObject);
                if (movements.Count != 0)
                {
                    GameMaster.instance.CreateMoves(movements, this.gameObject);
                }
            }
        }
    }

}
