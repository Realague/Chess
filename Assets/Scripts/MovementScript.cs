using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    public Movement movement;

    void OnMouseDown() {
        movement.DoMovement(GameMaster.instance.board, false);
    }
}
