using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Button_Player1_Handler : MonoBehaviour
{
    private GameManager GameManager;
    public NetworkManager manager;

    private void Awake()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
        manager     = GameObject.FindObjectOfType<NetworkManager>();  //instead of: GetComponent<NetworkManager>();
        if (manager == null) Debug.Log("Button_Player1_Handler: manger == null");
    }

    public void Set_PlayerID()
    {
        //GameManager.Select_PlayerID(1);
        GameManager.PlayerID = 1;
        Debug.Log("Select_Player1 was clicked, PlayerID = 1");

        // Start Host
        bool noConnection = (manager.client == null || manager.client.connection == null || manager.client.connection.connectionId == -1);
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (noConnection)
            {
                manager.StartHost();
                Debug.Log("StartHost");
            }
        }
    }

    public void Set_NetPlayerID()
    {
        //GameManager.Select_PlayerID(1);
        GameManager.PlayerID = 1;
        Debug.Log("Select_Player1 was clicked (Net Version), PlayerID = 1");

        // Start Host
        bool noConnection = (manager.client == null || manager.client.connection == null || manager.client.connection.connectionId == -1);
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (noConnection)
            {
                manager.StartMatchMaker();
                Debug.Log("StartMatchMaker");
            }
        }
    }

}
