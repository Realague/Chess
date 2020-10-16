using System.Collections;
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

    void OnMouseDown()
    {
        if (GameMaster.instance.selectedPiece == gameObject)
        {
            GameMaster.instance.DeleteMoves();
            return;
        }
        else
        {
            if (GameMaster.instance.CreateMovePawn(transform.position + new Vector3(0, 0, 1), this.gameObject) && isFirstMove == true)
            {
                GameMaster.instance.CreateMovePawn(transform.position + new Vector3(0, 0, 2), this.gameObject);
            }
            GameMaster.instance.CreateAttackMovePawn(transform.position + new Vector3(1, 0, 1), this.gameObject);
            GameMaster.instance.CreateAttackMovePawn(transform.position + new Vector3(-1, 0, 1), this.gameObject);
        }
    }

}
