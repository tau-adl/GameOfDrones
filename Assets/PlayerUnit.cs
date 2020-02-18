using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour
{
    // Unit variables
    //----------------------
    private GameManager GameManager;

    private float dirX, dirY, dirZ;
    private readonly float moveSpeedX = 3f;
    private readonly float moveSpeedY = 3f;
    private readonly float moveSpeedZ = -3f;

    // Drone throttling
    bool drone_throttling_enable = false;
    private int _cycle_size = 100;
    private int _cycle_counter = 1;
    private int _cycle_update_threshold = 20;
    private bool _drone_update;

    // Drone vars
    private int _frameCount;
    public Text droneLogText;

    void Start()
    {
        if (!isLocalPlayer) { return; }         // Don't effect an object that belongs to another player

        GameManager = GameObject.FindObjectOfType<GameManager>();
        if (GameManager.PlayerID == 1)
        {
            Debug.Log("PlayerUnit, Start: Player1 was spawn");
            this.transform.position = new Vector3(2.5f, 11, 10);
            CmdSpawnBall();                         // Request server to spawn an object
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("PlayerUnit, Start: Player2 was spawn");
            this.transform.position = new Vector3(17.5f, 11, 10);
            this.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else { Debug.Log("PlayerUnit, Start: ERROR. PlayerID != 1 or 2x"); }
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        GameManager = GameObject.FindObjectOfType<GameManager>();
        
        // Movement logic here
        dirX = 0f;
        dirZ = CrossPlatformInputManager.GetAxis("Horizontal");   // Left-Right
        dirY = CrossPlatformInputManager.GetAxis("Vertical");     // Up-Down
        if ((++_frameCount == 60) && (GameManager._telloClient != null))
        {
            _frameCount = 0;
            droneLogText.text = string.Format("Drone: online ({0:F0}% battery)", GameManager._telloClient.BatteryPercent);
        }

        // Drone throttling
        _drone_update = !drone_throttling_enable || (_cycle_counter <= _cycle_update_threshold);
        if ((++_cycle_counter == _cycle_size) && (GameManager._telloClient != null)) { _cycle_counter = 1; }

        if (dirZ != 0 || dirY != 0 || dirX != 0)
        {
            Vector3 Player_direction = new Vector3(dirX * moveSpeedX, dirY * moveSpeedY, dirZ * moveSpeedZ);
            transform.position += (Player_direction * Time.deltaTime);
        }

        // Send movement to drone code here
        if (GameManager._telloClient != null)
        {
            if (dirZ != 0 || dirY != 0 || dirX != 0) { Debug.Log("PlayerObject, Update: DirZ=" + dirZ + ", DirY=" + dirY + ", DirX=" + dirX); }
            if (_drone_update && GameManager.turn_is_on)
            {
                GameManager._telloClient.TurnRight    = (dirZ ==  1);
                GameManager._telloClient.TurnLeft     = (dirZ == -1);
                GameManager._telloClient.MoveBackward = (dirY ==  1);
                GameManager._telloClient.MoveForward  = (dirY == -1);
            }
            else if (_drone_update && !GameManager.turn_is_on)
            {
                GameManager._telloClient.MoveRight = (dirZ ==  1);
                GameManager._telloClient.MoveLeft  = (dirZ == -1);
                GameManager._telloClient.MoveDown  = (dirY ==  1);
                GameManager._telloClient.MoveUp    = (dirY == -1);
            }
            else
            {
                GameManager._telloClient.TurnRight    = false;
                GameManager._telloClient.TurnLeft     = false;
                GameManager._telloClient.MoveBackward = false;
                GameManager._telloClient.MoveForward  = false;
                GameManager._telloClient.MoveRight    = false;
                GameManager._telloClient.MoveLeft     = false;
                GameManager._telloClient.MoveDown     = false;
                GameManager._telloClient.MoveUp       = false;
            }
        }

        /*
        // Debug prints
        if (GameManager.turn_is_on)
        {
            if (dirZ == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Right");
            if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Left");
            if (dirY == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Backward");
            if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Forward");
        }
        else
        {
            if (dirZ == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Right");
            if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Left");
            if (dirY == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Down");
            if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Up");
        }*/

    }
    

    public GameObject BallPrefab;
    [Command]
    void CmdSpawnBall()
    {
        if (!isServer) { return; }

        // The following code will run only on the server
        Debug.Log("PlayerUnit, CmdSpawnBall");

        GameObject go = Instantiate(BallPrefab);

        // Propagate object to client (and wire up the NetworkIdentity)
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);   // Spawn with client authority
        //NetworkServer.Spawn(go);
    }


}