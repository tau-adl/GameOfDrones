using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Button_Player2_Handler : MonoBehaviour
{
    private GameManager GameManager;
    public NetworkManager manager;

    private void Awake()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
        manager     = GameObject.FindObjectOfType<NetworkManager>();  //instead of: GetComponent<NetworkManager>();
        if (manager == null) Debug.Log("Button_Player2_Handler: manger == null");
    }

    public void Set_PlayerID()
    {
        GameManager.PlayerID = 2;
        Debug.Log("Select_Player2 was clicked, PlayerID = 2");

        // Start Client
        bool noConnection = (manager.client == null || manager.client.connection == null || manager.client.connection.connectionId == -1);
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (noConnection)
            {
                manager.StartClient();
                Debug.Log("StartClient");
            }
        }
    }

    public void Set_NetPlayerID()
    {
        GameManager.PlayerID = 2;
        Debug.Log("Select_Player2 was clicked (Net Version), PlayerID = 2");

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