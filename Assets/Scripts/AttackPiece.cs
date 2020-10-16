using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPiece : MonoBehaviour
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
        GameObject parent = GameMaster.instance.selectedPiece;
        GameMaster.instance.DeletePiece(transform.position);
        parent.transform.position = transform.position;
        if (parent.GetComponent<Piece>().type == PieceType.Pawn)
        {
            PawnScript pawnScript = parent.GetComponent<PawnScript>();
            pawnScript.isFirstMove = false;
        }
        GameMaster.instance.DeleteMoves();
    }
}
