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
        GameMaster.instance.DeletePiece(transform.position);
        GameMaster.instance.Move(transform.position);
    }
}
