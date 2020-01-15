using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;


public class GameManager : MonoBehaviour
{
    public  int PlayerID = 0;  // 0 = None, 1 = Player1, 2 = Player2
    private int P1_Score = 0;
    private int P2_Score = 0;
    public TelloSdkClient _telloClient;
    public  bool _droneMotorsOn;
    public  int Tello_StickDataIntervalMilliseconds = 200;
    public  bool turn_is_on = false;
    /*
    public void Select_PlayerID(int Value_From_Button)
    {
        PlayerID = Value_From_Button;
        Debug.Log("Player Selected: " + PlayerID.ToString());
    }
    */

    public GameObject ball;
    public Text text;

    public float ballDistance = 2;
    public float ballThrowingForce = 10;

    //private int? _fingerId;
    //private Vector3 _touchStartPosition;


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
        //CameraDevice.Instance.SetField("focus-mode", "auto")
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
    
    // Update is called once per frame
    void Update()
    {
        text.text = "Player1: " + P1_Score.ToString() + ",   Player2: " + P2_Score.ToString(); // ball.transform.position.ToString();
    }

    // Turn Button code
    public void set_turn_on()
    {
        turn_is_on = true;
        Debug.Log("Turn is ON");
    }
    public void set_turn_off()
    {
        turn_is_on = false;
        Debug.Log("Turn is OFF");
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        text.text = "Collision!";
    }
    */
}
