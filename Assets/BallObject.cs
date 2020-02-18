using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallObject : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (!isLocalPlayer) { return; }   // object belongs to another player
        Debug.Log("BallObject-Start");
        //CmdSpawnBall();   // Request server to spawn an object
    }

    public GameObject BallUnitPrefab;

    // Update is called once per frame
    void Update()
    {
        
    }


    /*
    //////////////////   COMMANDS   ///////////////////////////////////
    // commands are special functions that ONLY get executed on the server
    // A way to send commands from the client to the server
    [Command]
    public void CmdSpawnBall()
    {
        // The following code will run only on the server
        Debug.Log("CmdSpawnBall");
        GameObject go = Instantiate(BallUnitPrefab);
        // Propagate object to client (and wire up the NetworkIdentity)
        NetworkServer.Spawn(go);
    }*/



}
