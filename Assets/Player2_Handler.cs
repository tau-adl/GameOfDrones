using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player2_Handler : MonoBehaviour
{
    private GameManager GameManager;

    private Rigidbody rb;
    private float dirX, dirY, dirZ;
    private float moveSpeedX = 3f;
    private float moveSpeedY = 3f;
    private float moveSpeedZ = 3f;
    private Vector3 Player_direction;

    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameManager.PlayerID.ToString());

        dirX = 0f;
        dirZ = CrossPlatformInputManager.GetAxis("Horizontal");   // Left-Right
        dirY = CrossPlatformInputManager.GetAxis("Vertical");     // Up-Down

        if (GameManager.PlayerID == 2)
        {
            Player_direction = new Vector3(dirX * moveSpeedX, dirY * moveSpeedY, dirZ * moveSpeedZ);
            transform.Translate(Player_direction * Time.deltaTime);
        }

        // Send movement to drone code here
        if (dirZ ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Left");
        if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Right");
        if (dirY ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Up");
        if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Down");
    }
}
