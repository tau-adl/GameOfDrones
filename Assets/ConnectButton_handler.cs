using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectButton_handler : MonoBehaviour
{
    private GameManager GameManager;


    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // called upon pressing ConnectButton
    public async void Connect2Drone()
    {
        if (GameManager.PlayerID == 1)
        {
            Debug.Log("Connecting to Drone1");
            // Connect to Drone1 call here
            if (GameManager._telloClient == null)
            {
                GameManager._telloClient = new TelloClientNative();
                await GameManager._telloClient.StartAsync();
                Debug.Log("Connected to Drone1");
            }
            else
            {
                Debug.Log("Error: Trying to connect to a connected drone");
            }
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("Connecting to Drone2");
            // Connect to Drone2 call here
            Debug.Log("Error: Trying to connect drone2 - NOT SUPPORTED YET");
        }
        else
        {
            Debug.Log("Error when connecting to drone, Invalid PlayerId");
        }
    }



    // called upon pressing ConnectButton
    public void Disconnect2Drone()
    {
        if (GameManager.PlayerID == 1)
        {
            Debug.Log("Disconnecting from Drone1");
            // Disconnect from Drone1 call here
            if (GameManager._telloClient == null)
            {
                Debug.Log("Error: Trying to disconnect from a disconnected drone");
            }
            else
            {
                GameManager._telloClient.Close();
                GameManager._telloClient = null;
            }
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("Disconnecting from Drone2");
            // Disconnect from Drone2 call here
            Debug.Log("Error: Trying to disconnect drone2 - NOT SUPPORTED YET");
        }
        else
        {
            Debug.Log("Error when disconnecting from drone, Invalid PlayerId");
        }
    }
}
