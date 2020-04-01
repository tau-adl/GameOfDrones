using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour
{
    private int board_y = 40;

    // Unit variables
    //----------------------
    private GameManager GameManager;
    private GameObject TelloTargetPosition;

    private float dirX, dirY, dirZ;
    private readonly float moveSpeedX = 3f;
    private readonly float moveSpeedY = 3f;
    private readonly float moveSpeedZ = 3f;

    // last_known_position
    public bool Player_position_updated_from_vuforia = false;
    Vector3     Player_last_known_position; // Last known position (not as a drone)

    //Vector3 Initial_target_position = new Vector3(-50f, 0f, 0f); // initial position of target (until target found this will be the position)
    bool    Drone_found_first_time = false;
    bool    Drone_at_required_position = false;
    Vector3 Drone_required_position;    // the position we want the drone to initially be

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
        TelloTargetPosition = GameObject.Find("TelloTargetPosition");

        Debug.Log("board_y=" + board_y);

        if (GameManager.PlayerID == 1)
        {
            Debug.Log("PlayerUnit, Start: Player1 was spawn");
            this.transform.position    = new Vector3(2.5f, board_y+1, 10);
            Player_last_known_position = new Vector3(2.5f, board_y+1, 10);
            Drone_required_position    = new Vector3(2.5f, board_y+1, 10); // the position we want the drone to initially be
            CmdSpawnBall();                         // Request server to spawn an object
        }
        else if (GameManager.PlayerID == 2)
        {
            Debug.Log("PlayerUnit, Start: Player2 was spawn");
            this.transform.position    = new Vector3(17.5f, board_y+1, 10);
            Player_last_known_position = new Vector3(17.5f, board_y+1, 10);
            Drone_required_position    = new Vector3(17.5f, board_y+1, 10); // the position we want the drone to initially be
            //this.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else { Debug.Log("PlayerUnit, Start: ERROR. PlayerID != 1 or 2x"); }
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        GameManager = GameObject.FindObjectOfType<GameManager>();

        // Debug Zone
        //Debug.Log("PlayerUnit, Update: GameManager.Target_is_aquired=" + GameManager.Target_is_aquired);

        // Movement logic here
        dirX = CrossPlatformInputManager.GetAxis("Forward_Backward");
        dirZ = CrossPlatformInputManager.GetAxis("Left_Right");
        dirY = CrossPlatformInputManager.GetAxis("Up_Down");

        if ((++_frameCount == 60) && (GameManager._telloClient != null))
        {
            _frameCount = 0;
            droneLogText.text = string.Format("Drone: online ({0:F0}% battery)", GameManager._telloClient.BatteryPercent);
        }

        // Drone throttling
        _drone_update = !drone_throttling_enable || (_cycle_counter <= _cycle_update_threshold);
        if ((++_cycle_counter == _cycle_size) && (GameManager._telloClient != null)) { _cycle_counter = 1; }

        // update player position when dis-connecting from Tello
        if (!GameManager.ConnectedToTello && Player_position_updated_from_vuforia)
        {
            Debug.Log("Updating Player_position to be last known position");
            transform.position = Player_last_known_position;
            Player_position_updated_from_vuforia = false;
        }

        // Update Player position
        if (!GameManager.ConnectedToTello && (dirZ != 0 || dirY != 0 || dirX != 0))
        {
            // update player position according to inputs from CrossPlatformInputManager
            Vector3 Player_direction;
            Player_last_known_position = transform.position;  // save last known position for dis-connect (place player at this position)
            if (GameManager.PlayerID == 1) { Player_direction = new Vector3( dirX * moveSpeedX, dirY * moveSpeedY, -dirZ * moveSpeedZ); }
            else                           { Player_direction = new Vector3(-dirX * moveSpeedX, dirY * moveSpeedY,  dirZ * moveSpeedZ); }
            Vector3 Player_new_position = transform.position + (Player_direction * Time.deltaTime);
            transform.position = Player_new_position;
        }
        else if (GameManager.ConnectedToTello)
        {
            Player_position_updated_from_vuforia = true;  // position updated according to vuforia (Player_last_known_position can be used when dis-connecting from Tello)

            // update player position according to Vuforia object
            Vector3 Target_position = TelloTargetPosition.transform.position;
            Debug.Log("PlayerUnit (ConnectedToTello): Tello position=(" + Target_position.x + ", " + Target_position.y + ", " + Target_position.z + ")");
            //Quaternion Player_new_rotation = TelloTargetPosition.transform.rotation;
            //Debug.Log("PlayerUnit (ConnectedToTello): Tello rotation=(" + Player_new_rotation.w + ", " + Player_new_rotation.x + ", " + Player_new_rotation.y + ", " + Player_new_rotation.z + ")");

                if (!Drone_found_first_time && GameManager.Target_is_aquired)
            {
                Debug.Log("PlayerUnit, Update: Drone_found_first_time. Tello position=(" + Target_position.x + ", " + Target_position.y + ", " + Target_position.z + ")");
                Drone_found_first_time = true;
            }
            transform.position = Target_position;

            /*
            // Send movement to drone code here
            if (GameManager._droneMotorsOn && Drone_found_first_time && !Drone_at_required_position)
            {
                // This will move the drone to the required initial position
                // Until this position is reached, the inputs will be ignored
                Vector3 diff_to_required_position = (Target_position - Drone_required_position);
            }
            else */ 
            if (GameManager._telloClient != null)
            {
                if (dirZ != 0 || dirY != 0 || dirX != 0) { Debug.Log("PlayerObject, Update: DirZ=" + dirZ + ", DirY=" + dirY + ", DirX=" + dirX); }
                if (_drone_update && GameManager.turn_is_on)
                {
                    GameManager._telloClient.TurnLeft     = (dirZ == -1);
                    GameManager._telloClient.TurnRight    = (dirZ == 1);
                    GameManager._telloClient.MoveDown     = (dirY == -1);
                    GameManager._telloClient.MoveUp       = (dirY == 1);
                    GameManager._telloClient.MoveBackward = (dirX == -1);
                    GameManager._telloClient.MoveForward  = (dirX == 1);
                }
                else if (_drone_update && !GameManager.turn_is_on)
                {
                    GameManager._telloClient.MoveLeft     = (dirZ == -1);
                    GameManager._telloClient.MoveRight    = (dirZ == 1);
                    GameManager._telloClient.MoveDown     = (dirY == -1);
                    GameManager._telloClient.MoveUp       = (dirY == 1);
                    GameManager._telloClient.MoveBackward = (dirX == -1);
                    GameManager._telloClient.MoveForward  = (dirX == 1);
                }
                else
                {
                    GameManager._telloClient.TurnRight = false;
                    GameManager._telloClient.TurnLeft = false;
                    GameManager._telloClient.MoveBackward = false;
                    GameManager._telloClient.MoveForward = false;
                    GameManager._telloClient.MoveRight = false;
                    GameManager._telloClient.MoveLeft = false;
                    GameManager._telloClient.MoveDown = false;
                    GameManager._telloClient.MoveUp = false;
                }
            }

        }


        /*
        // Debug prints
        if (_drone_update && GameManager.turn_is_on)
        {
            if (dirZ == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Right");
            if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Left");
            if (dirY == 1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Backward");
            if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Forward");
        }
        else if (_drone_update && !GameManager.turn_is_on)
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