using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Networking;
using UnityEngine;
using System;

public class ButtonStart_Handler : MonoBehaviour
{
    private BallMovement BallMovement;

    // Start is called before the first frame update
    void Start()
    {
        BallMovement = GameObject.FindObjectOfType<BallMovement>();
    }

    // Called upon StartButton press
    public void Set_GameOn()
    {
        if (BallMovement.Ball_is_moving) Debug.Log("BallMovement.Ball_is_moving = true");
        else                             Debug.Log("BallMovement.Ball_is_moving = false");

        BallMovement.Ball_is_moving = true;

        if (BallMovement.Ball_is_moving) Debug.Log("BallMovement.Ball_is_moving = true");
        else Debug.Log("BallMovement.Ball_is_moving = false");

        // TBD: send Ball_is_moving to Player2

        Debug.Log("Game is ON!!!");
    }

    /*
    [Command]
    public void CmdSet_GameOn()
    {
        // This will sent a command to the server to change Ball_is_moving
        BallMovement.Ball_is_moving = true;
    }
    */
}
