﻿using UnityEngine;
using UnityEngine.Networking;

public class BallMovement : NetworkBehaviour
{
    private int board_y = 40;

    private GameManager     GameManager;
    public  PlayerUnit      Player1;
    //private Player2_Handler Player2_Handler;
    private SFXPlaying      SFXPlaying;

    public Vector3 direction;
    public float radius;
    //float min_ball_x = 10;  // TBD: remove this temporary var
    Vector3 last_ball_position;// = new Vector3(10f, 50.5f, 10f);
    Vector3 PlayerSize;// = new Vector3(1.0f, 2.0f, 3.0f);
    Vector3 InitPosition;// = new Vector3(10.0f, 50.5f, 10.0f);

    // Constants
    public  float speed      =  0.7f;
    private float Lo_Bound_X =  0f;
    private float Lo_Bound_Y;// =  50f;
    private float Lo_Bound_Z =  0f;
    private float Hi_Bound_X =  20f;
    private float Hi_Bound_Y;// =  55f;
    private float Hi_Bound_Z =  20f;
    float epsilon = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BallMovement, Start");

        GameManager = GameObject.FindObjectOfType<GameManager>();
        SFXPlaying = GameObject.FindObjectOfType<SFXPlaying>();

        last_ball_position = new Vector3(10f, 50.5f, 10f);
        PlayerSize = new Vector3(1.0f, 2.0f, 3.0f);
        InitPosition = new Vector3(10.0f, 50.5f, 10.0f);
        Lo_Bound_Y = 50f;
        Hi_Bound_Y = 55f;

        bool  initial_x_dir = (Random.value > 0.5f);
        float initial_vel_x = Random.Range(3.0f, 5.0f);  // 4.0f; // 
        float initial_vel_y = 0.0f; // Random.Range(3.0f, 5.0f);  // 
        float initial_vel_z = Random.Range(3.0f, 5.0f);  // 3.0f; // 
        if (!initial_x_dir) { initial_vel_x = -initial_vel_x; }  // 50% move backwards

        direction = new Vector3(initial_vel_x, initial_vel_y, initial_vel_z).normalized;//float initial_vel_x = Random.Range(3.0f, 5.0f);

        radius = transform.localScale.x / 2; // half the ball width
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.PlayerID != 1) { return; }   // Only Host update the balls position

        Vector3 PlayerCenter = other.transform.position;
        Vector3 BallCenter   = transform.position;
        Vector3 ColPoint     = other.ClosestPoint(transform.position);  // collision point

        // Deubg zone
        Debug.Log("Collision detected");
        Debug.Log("player center=(" + PlayerCenter.x + ", " + PlayerCenter.y + ", " + PlayerCenter.z + ")");
        Debug.Log("collision point=(" + ColPoint.x + ", " + ColPoint.y + ", " + ColPoint.z + ")");
        Debug.Log("ball center=(" + BallCenter.x + ", " + BallCenter.y + ", " + BallCenter.z + ")");

        // Play SFX
        SFXPlaying.PlayBounceFX();

        float P_upperZ = (PlayerCenter.z + PlayerSize.z / 2) + radius;
        float P_lowerZ = (PlayerCenter.z - PlayerSize.z / 2) - radius;
        /*
        
        float   P1_upperZ = (P1Pos.z + P_Size.z / 2) + radius;
        float   P1_lowerZ = (P1Pos.z - P_Size.z / 2) - radius;
        float   P2_upperZ = (P2Pos.z + P_Size.z / 2) + radius;
        float   P2_lowerZ = (P2Pos.z - P_Size.z / 2) - radius;

        Debug.Log("Object name: " + other.gameObject.name + "   Collision point: " + ColPoint + "   P1_pos: " + P1Pos + "   Ball_pos: " + BallPos + "   Direction.x: " + direction.x);
        */

        // Ball direction update
        float diff_x = Mathf.Abs(BallCenter.x - ColPoint.x);
        float diff_y = Mathf.Abs(BallCenter.y - ColPoint.y);
        float diff_z = Mathf.Abs(BallCenter.z - ColPoint.z);
        if (diff_x > epsilon)  direction.x = -direction.x;  // Change X direction
        if (diff_y > epsilon)  direction.y = -direction.y;  // Change Y direction
        if (diff_z > epsilon)  direction.z = -direction.z;  // Change Z direction
    }

    // Update is called once per frame
    void Update()
    {
        // Inc score when hitting player side + PointFX (according to next position)
        // DEBUG ONLY
        /*
        if (transform.position.x < min_ball_x)
        {
            min_ball_x = transform.position.x;
            Debug.Log("BallMovement, Update: Min ball position x=" + min_ball_x);
        }
        //Debug.Log("BallMovement, Update: Ball position=(" + transform.position.x + "," + transform.position.y + "," + transform.position.z + ")");
        */
        if (transform.position.x <= (Lo_Bound_X + radius + 2*epsilon) &&
            last_ball_position.x >  (Lo_Bound_X + radius + 2*epsilon)) { GameManager.IncPlayerScore(2); SFXPlaying.PlayPointFX(); }
        if (transform.position.x >= (Hi_Bound_X - radius - 2*epsilon) &&
            last_ball_position.x <  (Hi_Bound_X - radius - 2*epsilon)) { GameManager.IncPlayerScore(1); SFXPlaying.PlayPointFX(); }
        last_ball_position = transform.position;

        if (GameManager.PlayerID != 1) { return; }   // Only Host update the balls position

        if (GameManager.restart_pressed)  // init ball position & randomize new ball direction
        {
            // Update ball position to init position
            GameManager.restart_pressed = false;
            transform.position = InitPosition;

            // Randomize new direction
            float initial_vel_x = Random.Range(3.0f, 5.0f);
            float initial_vel_y = 0.0f;
            float initial_vel_z = Random.Range(3.0f, 5.0f);
            direction = new Vector3(initial_vel_x, initial_vel_y, initial_vel_z).normalized;
            last_ball_position = new Vector3(10f, board_y+0.5f, 10f);
        }

        Player1 = GameObject.FindObjectOfType<PlayerUnit>();

        if (Player1 != null)
        {
            if (GameManager.ball_is_moving)
            {
                // Debug Zone
                //Debug.Log("BallMovement: ball_is_moving is set!!!!");
                //Debug.Log("BallMovement: Ball direction=" + direction);
                //Debug.Log("BallMovement, Update: Ball position=(" + transform.position.x + "," + transform.position.y + "," + transform.position.z +")");

                // move Ball to next position
                transform.Translate(direction * speed * Time.deltaTime);
                //if (transform.position.x <= Lo_Bound_X + radius && direction.x < 0) { GameManager.IncPlayerScore(2); SFXPlaying.PlayPointFX(); }
                //if (transform.position.x >= Hi_Bound_X - radius && direction.x > 0) { GameManager.IncPlayerScore(1); SFXPlaying.PlayPointFX(); }

                // reverse velocity if boundary reached
                if (transform.position.x <= Lo_Bound_X + radius && direction.x < 0) direction.x = -direction.x;
                if (transform.position.y <= Lo_Bound_Y + radius && direction.y < 0) direction.y = -direction.y;
                if (transform.position.z <= Lo_Bound_Z + radius && direction.z < 0) direction.z = -direction.z;
                if (transform.position.x >= Hi_Bound_X - radius && direction.x > 0) direction.x = -direction.x;
                if (transform.position.y >= Hi_Bound_Y - radius && direction.y > 0) direction.y = -direction.y;
                if (transform.position.z >= Hi_Bound_Z - radius && direction.z > 0) direction.z = -direction.z;
            }
        }
    }



}
