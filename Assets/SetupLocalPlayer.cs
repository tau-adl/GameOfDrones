using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour
{

    void Start()
    {
        if (isLocalPlayer)  // isLocalPlayer is derived from the NetworkBehaviour
        {
            //this.enabled = true;
            this.transform.Translate(new Vector3(2.5f, 11, 10));
            this.transform.Rotate(new Vector3(0, 180, 0));
            GetComponent<Player1_Handler>().enabled = true;
        }
    }
}
