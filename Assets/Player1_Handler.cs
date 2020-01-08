using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class Player1_Handler : MonoBehaviour
{
    private GameManager GameManager;

    //private Rigidbody rb;
    private float dirX, dirY, dirZ;
    private readonly float moveSpeedX = 3f;
    private readonly float moveSpeedY = 3f;
    private readonly float moveSpeedZ = 3f;
    private Vector3 Player_direction;
    private int duty_cycle;
    private int update_part;
    private int cycle_counter;

    // Drone vars
    private int _frameCount;
    public Text droneLogText;

    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();

        // Tello speed
        GameManager._telloClient.StickDataIntervalMilliseconds = GameManager.Tello_StickDataIntervalMilliseconds;
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            //Debug.Log(GameManager.PlayerID.ToString());
            
            dirX = 0f;
            dirZ = CrossPlatformInputManager.GetAxis("Horizontal");   // Left-Right
            dirY = CrossPlatformInputManager.GetAxis("Vertical");     // Up-Down

            if (GameManager.PlayerID == 1)
            {
                if ((++_frameCount == 60) && (GameManager._telloClient != null))
                {
                    _frameCount = 0;
                    droneLogText.text = string.Format("Drone: online ({0:F0}% battery)", GameManager._telloClient.BatteryPercent);
                }

                Player_direction = new Vector3(dirX * moveSpeedX, dirY * moveSpeedY, dirZ * moveSpeedZ);
                transform.Translate(Player_direction * Time.deltaTime);

                // Send movement to drone code here
                if (GameManager._telloClient != null)
                {
                    if (dirZ!=0 || dirY!=0) { Debug.Log("DirZ=" + dirZ + ", DirY=" + dirY); }
                    if (GameManager.turn_is_on)
                    {
                        GameManager._telloClient.TurnLeft     = (dirZ ==  1);
                        GameManager._telloClient.TurnRight    = (dirZ == -1);
                        GameManager._telloClient.MoveForward  = (dirY ==  1);
                        GameManager._telloClient.MoveBackward = (dirY == -1);
                    }
                    else
                    {
                        GameManager._telloClient.MoveLeft  = (dirZ ==  1);
                        GameManager._telloClient.MoveRight = (dirZ == -1);
                        GameManager._telloClient.MoveUp    = (dirY ==  1);
                        GameManager._telloClient.MoveDown  = (dirY == -1);
                    }
                }

                if (GameManager.turn_is_on)
                {
                    if (dirZ ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Left");
                    if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Turn Right");
                    if (dirY ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Forward");
                    if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Backward");
                }
                else
                {
                    if (dirZ ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Left");
                    if (dirZ == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Right");
                    if (dirY ==  1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Up");
                    if (dirY == -1) Debug.Log("Player " + GameManager.PlayerID.ToString() + " Going Down");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    /*
    public async void ToggleDroneComm()
    {
        try
        {
            if (_telloClient == null)
            {
                _telloClient = new TelloClientNative();
                //droneCommToggle.GetComponentInChildren<Text>().text = CommOnText;
                //droneCommToggle.image.sprite = droneTurnCommOffSprite;
                await _telloClient.StartAsync();
                //droneMotorsToggle.image.sprite = droneTurnMotorsOnSprite;
                //droneMotorsToggle.enabled = true;
                //planeFinder.enabled = false;
            }
            else
            {
                _telloClient.Close();
                _telloClient = null;
                //droneCommToggle.GetComponentInChildren<Text>().text = CommOffText;
                //droneCommToggle.image.sprite = droneTurnCommOnSprite;
                //droneMotorsToggle.image.sprite = buttonDisabledSprite;
                //droneMotorsToggle.enabled = false;
                //foreach (var button in _droneControlButtons)
                //    button.enabled = false;
                droneLogText.text = "Drone: offline";
                //planeFinder.enabled = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            droneLogText.text = "Drone: " + ex.Message;
        }
    }
    */

    public async void ToggleDroneMotors()
    {
        try
        {
            if (GameManager._telloClient != null)
            {
                //droneMotorsToggle.enabled = false;
                if (!GameManager._droneMotorsOn)
                    GameManager._droneMotorsOn = await GameManager._telloClient.TakeOffAsync();
                else
                    GameManager._droneMotorsOn = !await GameManager._telloClient.LandAsync();
                //droneMotorsToggle.enabled = true;
                //droneMotorsToggle.image.sprite = _droneMotorsOn
                // ? droneTurnMotorsOffSprite
                // : droneTurnMotorsOnSprite;
                //foreach (var button in _droneControlButtons)
                //    button.enabled = _droneMotorsOn;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            droneLogText.text = "Drone: " + ex.Message;
        }
    }

}
