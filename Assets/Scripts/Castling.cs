using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castling : MonoBehaviour
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
        float z = GameMaster.instance.selectedPiece.transform.position.z;
        if (GameMaster.instance.selectedPiece.transform.position.x == 0)
        {
            GameMaster.instance.board.MovePiece(new Vector3(2, 0, z), GameMaster.instance.selectedPiece);
            GameObject parent = GameMaster.instance.selectedPiece;
            parent.transform.position = new Vector3(2, 0, z);
            GameObject king = GameMaster.instance.board.CheckCase(this.transform.position);
            GameMaster.instance.board.MovePiece(new Vector3(1, 0, z), king);
            king.transform.position = new Vector3(1, 0, z);
        } else if (GameMaster.instance.selectedPiece.transform.position.x == 7)
        {
            GameMaster.instance.board.MovePiece(new Vector3(5, 0, z), GameMaster.instance.selectedPiece);
            GameObject parent = GameMaster.instance.selectedPiece;
            parent.transform.position = new Vector3(5, 0, z);
            GameObject king = GameMaster.instance.board.CheckCase(this.transform.position);
            GameMaster.instance.board.MovePiece(new Vector3(6, 0, z), king);
            king.transform.position = new Vector3(6, 0, z);
        }

        GameMaster.instance.DeleteMoves();
    }
}
