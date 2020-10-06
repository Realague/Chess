using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static bool isPlayerTurn = true;

    private bool isPlayerStarting = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!isPlayerStarting)
        {
            isPlayerTurn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerTurn)
        {

        }
    }
}
