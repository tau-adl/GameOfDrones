using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;


public class GameManager : MonoBehaviour
{
    public int PlayerID = 0;  // 0 = None, 1 = Player1, 2 = Player2
    private int P1_Score = 0;
    private int P2_Score = 0;
    public TelloSdkClient _telloClient;
    public bool _droneMotorsOn;
    public float MoveSpeedFactor = 0.25f;
    public int Tello_StickDataIntervalMilliseconds = 200;
    public bool ConnectedToTello = false;
    public bool turn_is_on = false;  // turn left/right or move left/right
    public bool Target_is_aquired = false;

    public GameObject ball;
    public Text text;
    public bool ball_is_moving = false;
    public bool restart_pressed = false;


    // Turn flash light on/off
    public bool light_is_on;
    public void turn_light_on()
    {
        Debug.Log("GameManager, turn_light_on");
        CameraDevice.Instance.SetFlashTorchMode(true);
        light_is_on = true;
    }
    public void turn_light_off()
    {
        Debug.Log("GameManager, turn_light_off");
        CameraDevice.Instance.SetFlashTorchMode(false);
        light_is_on = false;
    }


    // Change Antiband (0=Off, 1=50Hz, 2=60Hz)
    public int antibanding;
    public void change_to_antiband_off()
    {
        Debug.Log("GameManager, change_to_antiband_off");
        CameraDevice.Instance.SetField("antibanding", "off");
        antibanding = 0;
    }
    public void change_to_antiband_50Hz()
    {
        Debug.Log("GameManager, change_to_antiband_50Hz");
        CameraDevice.Instance.SetField("antibanding", "on");
        CameraDevice.Instance.SetField("antibanding-values", "50hz");
        antibanding = 1;
    }
    public void change_to_antiband_60Hz()
    {
        Debug.Log("GameManager, change_to_antiband_60Hz");
        CameraDevice.Instance.SetField("antibanding", "on");
        CameraDevice.Instance.SetField("antibanding-values", "60hz");
        antibanding = 2;
    }


    // Change Focus mode (0=Normal, 1=TrigAuto, 2=ContAuto, 3=Inifinity, 4=Macro)
    public int focus_mode;
    public void change_to_focus_Normal()
    {
        Debug.Log("GameManager, change_to_focus_Normal");
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        focus_mode = 0;
    }
    public void change_to_focus_TrigAuto()
    {
        Debug.Log("GameManager, change_to_focus_TrigAuto");
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
        focus_mode = 1;
    }
    public void change_to_focus_ContAuto()
    {
        Debug.Log("GameManager, change_to_focus_ContAuto");
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        focus_mode = 2;
    }
    public void change_to_focus_Infinity()
    {
        Debug.Log("GameManager, change_to_focus_Infinity");
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_INFINITY);
        antibanding = 3;
    }
    public void change_to_focus_Macro()
    {
        Debug.Log("GameManager, change_to_focus_Macro");
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_MACRO);
        antibanding = 4;
    }


    // Change video_mode (0=Default, 1=Speed, 2=Quality)
    public int video_mode;
    public void change_to_VideoMode_Default()
    {
        Debug.Log("GameManager, change_to_VideoMode_Default");
        CameraDevice.Instance.SelectVideoMode(Vuforia.CameraDevice.CameraDeviceMode.MODE_DEFAULT);
        video_mode = 0;
    }
    public void change_to_VideoMode_Speed()
    {
        Debug.Log("GameManager, change_to_VideoMode_Speed");
        CameraDevice.Instance.SelectVideoMode(Vuforia.CameraDevice.CameraDeviceMode.MODE_OPTIMIZE_SPEED);
        video_mode = 1;
    }
    public void change_to_VideoMode_Quality()
    {
        Debug.Log("GameManager, change_to_VideoMode_Quality");
        CameraDevice.Instance.SelectVideoMode(Vuforia.CameraDevice.CameraDeviceMode.MODE_OPTIMIZE_QUALITY);
        video_mode = 2;
    }

    // Inc score functions
    public void IncPlayerScore(int PlayerID)
    {
        if (PlayerID == 1) P1_Score++;
        if (PlayerID == 2) P2_Score++;
    }


    // Start is called before the first frame update
    void Start()
    {
        var vuforia = VuforiaARController.Instance;
        vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);

        CameraDevice.Instance.SetField("exposure-time", "auto");
        CameraDevice.Instance.SetField("iso", "auto");
        CameraDevice.Instance.SetField("antibanding", "off");
        //CameraDevice.Instance.SetField("antibanding", "on");
        //CameraDevice.Instance.SetField("antibanding-values", "50hz");
        //CameraDevice.Instance.SelectVideoMode("MODE_OPTIMIZE_QUALITY");
        //CameraDevice.Instance.SelectVideoMode("MODE_OPTIMIZE_SPEED");
        //CameraDevice.Instance.SelectVideoMode("");
        //CameraDevice.Instance.SetField("VideoModeData.frameRate", "");
        //CameraDevice.Instance.SetField("focus-mode", "auto")
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnVuforiaStarted()
    {
        // Set default Light setting
        light_is_on = false;
        CameraDevice.Instance.SetFlashTorchMode(false);

        // Set default antibanding mode setting
        antibanding = 1;
        CameraDevice.Instance.SetField("antibanding", "on");
        CameraDevice.Instance.SetField("antibanding-values", "50hz");

        // Set default FocusMode setting
        focus_mode = 2;
        CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        //focus_mode = 0;
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);  // Default value (can be changed in menu)

        // Set default VideoMode setting
        //video_mode = 2;
        //CameraDevice.Instance.SelectVideoMode(Vuforia.CameraDevice.CameraDeviceMode.MODE_OPTIMIZE_QUALITY);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Player1: " + P1_Score.ToString() + ",   Player2: " + P2_Score.ToString(); // ball.transform.position.ToString();
    }

    // Ball movement code
    public void toggle_ball_is_moving()
    {
        if (PlayerID == 1)
        {
            if (ball_is_moving) { Debug.Log("GameManager,ball_is_moving = false"); }
            else                { Debug.Log("GameManager,ball_is_moving = true"); }
            ball_is_moving = !ball_is_moving;
        }
    }
    public void set_ball_is_moving()
    {
        if (PlayerID == 1)
        {
            Debug.Log("GameManager, set_ball_is_moving");
            ball_is_moving = true;
        }
    }
    public void clear_ball_is_moving()
    {
        if (PlayerID == 1)
        {
            Debug.Log("GameManager, clear_ball_is_moving");
            ball_is_moving = false;
        }
    }

    // restart game (move ball to center + choose new random initial direction
    public void restart_game()
    {
        //if (PlayerID == 1)
        //{
            Debug.Log("GameManager, restart_game");
            ball_is_moving = false;  // stop ball from moving
            restart_pressed = true;  // ball should return to original location
        //}
    }

    public void set_turn_on()
    {
        if (PlayerID == 1)
        {
            Debug.Log("Turn is ON");
            turn_is_on = true;   // turn left/right or move left/right
        }
    }
    public void set_turn_off()
    {
        Debug.Log("Turn is OFF");
        turn_is_on = false;      // turn left/right or move left/right
    }

}
