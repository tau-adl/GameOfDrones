using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Player2_Handler : MonoBehaviour
{
    private GameManager GameManager;
    private void Awake()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void Set_PlayerID()
    {
        //GameManager.Select_PlayerID(2);
        GameManager.PlayerID = 2;
        Debug.Log("Select_Player2 was clicked, PlayerID = 2");
    }
}