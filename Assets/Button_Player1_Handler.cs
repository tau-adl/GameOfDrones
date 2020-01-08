using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Player1_Handler : MonoBehaviour
{
    private GameManager GameManager;
    private void Awake()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void Set_PlayerID()
    {
        //GameManager.Select_PlayerID(1);
        GameManager.PlayerID = 1;
        Debug.Log("Select_Player1 was clicked, PlayerID = 1");
    }

}
