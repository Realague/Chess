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
        parent.GetComponent<Piece>().isFirstMove = false;

        GameMaster.instance.DeleteMoves();
        GameMaster.instance.turn = GameMaster.instance.turn == Color.Light ? Color.Dark : Color.Light;
    }
}
