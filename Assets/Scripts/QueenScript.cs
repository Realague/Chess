﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<KeyValuePair<Vector3, MoveType>> QueenMovement(Vector3 position, Board board, GameObject piece)
    {
        List<KeyValuePair<Vector3, MoveType>> movements = new List<KeyValuePair<Vector3, MoveType>>();
        movements = RookScript.RookMovement(position, board, piece);
        movements.AddRange(BishopScript.BishopMovement(position, board, piece));
        return movements;
    }

    void OnMouseDown()
    {
        if (GetComponent<Piece>().side == GameMaster.instance.turn)
        {
            if (GameMaster.instance.selectedPiece == gameObject)
            {
                GameMaster.instance.DeleteMoves();
                return;
            }
            else
            {
                var movements = QueenMovement(transform.position, GameMaster.instance.board, this.gameObject);
                if (movements.Count != 0)
                {
                    GameMaster.instance.CreateMoves(movements, this.gameObject);
                }
            }
        }
    }
}
