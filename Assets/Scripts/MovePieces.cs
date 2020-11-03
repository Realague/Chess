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
        parent.GetComponent<Piece>().isFirstMove = false;
        GameMaster.instance.DeleteMoves();
        GameMaster.instance.turn = GameMaster.instance.turn == Color.Light ? Color.Dark : Color.Light;
    }
}
