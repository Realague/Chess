using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public Vector3 position;

    public GameObject piece;

    public MoveType moveType;

    public Movement(Vector3 position, GameObject piece, MoveType moveType)
    {
        this.position = position;
        this.piece = piece;
        this.moveType = moveType;
    }

    public void DoMovement()
    {

    }

}