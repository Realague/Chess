using System.Collections;
using System.Collections.Generic;
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
        GameMaster.instance.board.MovePiece(transform.position, GameMaster.instance.selectedPiece);
        GameObject parent = GameMaster.instance.selectedPiece;
        parent.transform.position = transform.position;
        if (parent.GetComponent<Piece>().type == PieceType.Pawn)
        {
            PawnScript pawnScript = parent.GetComponent<PawnScript>();
            pawnScript.isFirstMove = false;
        }
        else if (parent.GetComponent<Piece>().type == PieceType.Rook)
        {
            RookScript rookScript = parent.GetComponent<RookScript>();
            rookScript.isFirstMove = false;
        }
        else if (parent.GetComponent<Piece>().type == PieceType.King)
        {
            KingScript kingScript = parent.GetComponent<KingScript>();
            kingScript.isFirstMove = false;
        }
        GameMaster.instance.DeleteMoves();
    }
}
