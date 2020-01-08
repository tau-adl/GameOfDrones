using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOff_button : MonoBehaviour
{
    private GameManager GameManager;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // called upon pressing ConnectButton
    public async void SendTakeoff()
    {
        if (GameManager.PlayerID == 1)
        {
            Debug.Log("Takeoff Drone1");
            // Send Takeoff to Drone1 here
            if (GameManager._telloClient != null)
            {
                GameManager._droneMotorsOn = await GameManager._telloClient.TakeOffAsync();
            }
            else Debug.Log("Drone1 SendTakeOff FAILED");
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("Takeoff Drone2");
            // Send Takeoff to Drone2 here
            Debug.Log("Takeoff Drone2 - NOT SUPPORTED YET");
        }
        else
        {
            Debug.Log("Error when sending Takeoff drone, Invalid PlayerId");
        }
    }


    // called upon pressing ConnectButton
    public async void SendLand()
    {
        if (GameManager.PlayerID == 1)
        {
            Debug.Log("Land Drone1");
            // Send Land to Drone1 here
            if (GameManager._telloClient != null)
            {
                GameManager._droneMotorsOn = !await GameManager._telloClient.LandAsync();
            }
            else Debug.Log("Drone1 SendLand FAILED");
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("Land Drone2");
            // Send Land to Drone2 here
            Debug.Log("Land Drone2 - NOT SUPPORTED YET");
        }
        else
        {
            Debug.Log("Error when sending Land drone, Invalid PlayerId");
        }
    }

}
