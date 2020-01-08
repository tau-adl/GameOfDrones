using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    private GameManager     GameManager;
    private Player1_Handler Player1_Handler;
    private Player2_Handler Player2_Handler;
    private SFXPlaying      SFXPlaying;

    public bool Ball_is_moving = false;
    Vector3 Curr_Position;
    Vector3 direction;
    float radius;

    // Constants
    public  float speed      =  5f;
    private float Lo_Bound_X =  0f;
    private float Lo_Bound_Y =  10f;
    private float Lo_Bound_Z =  0f;
    private float Hi_Bound_X =  20f;
    private float Hi_Bound_Y =  15f;
    private float Hi_Bound_Z =  20f;

    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision detected");

        // Play SFX
        SFXPlaying.PlayBounceFX();

        Vector3 ColPoint = other.ClosestPoint(transform.position);  // collision point
        Vector3 P1Pos    = Player1_Handler.transform.position;
        Vector3 P2Pos    = Player2_Handler.transform.position;
        Vector3 P_Size   = Player1_Handler.transform.localScale;
        Vector3 BallPos  = transform.position;

        bool    Ball_hit_Player1 = other.gameObject.name == "Player1";
        bool    Ball_hit_Player2 = other.gameObject.name == "Player2";
        //bool    Ball_direction_to_Player1 = (direction.x < 0);
        //bool    Ball_direction_to_Player2 = (direction.x > 0);

        float   P1_upperZ = (P1Pos.z + P_Size.z / 2) + radius;
        float   P1_lowerZ = (P1Pos.z - P_Size.z / 2) - radius;
        float   P2_upperZ = (P2Pos.z + P_Size.z / 2) + radius;
        float   P2_lowerZ = (P2Pos.z - P_Size.z / 2) - radius;

        Debug.Log("Object name: " + other.gameObject.name + "   Collision point: " + ColPoint + "   P1_pos: " + P1Pos + "   Ball_pos: " + BallPos + "   Direction.x: " + direction.x);

        //if (Ball_direction_to_Player1 && (BallPos.z <= P1_upperZ) && (BallPos.z >= P1_lowerZ)) direction.x = -direction.x;  // Ball hit P1
        //if (Ball_direction_to_Player2 && (BallPos.z <= P2_upperZ) && (BallPos.z >= P2_lowerZ)) direction.x = -direction.x;  // Ball hit P2
        if (Ball_hit_Player1 && (BallPos.z <= P1_upperZ) && (BallPos.z >= P1_lowerZ)) direction.x = -direction.x;  // Ball hit P1
        if (Ball_hit_Player2 && (BallPos.z <= P2_upperZ) && (BallPos.z >= P2_lowerZ)) direction.x = -direction.x;  // Ball hit P2
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
        Player1_Handler = GameObject.FindObjectOfType<Player1_Handler>();
        Player2_Handler = GameObject.FindObjectOfType<Player2_Handler>();
        SFXPlaying = GameObject.FindObjectOfType<SFXPlaying>();

        float initial_vel_x = Random.Range(3.0f, 5.0f);
        float initial_vel_y = 0;  //Random.Range(3.0f, 5.0f); //
        float initial_vel_z = Random.Range(3.0f, 5.0f);
        direction = new Vector3(initial_vel_x, initial_vel_y, initial_vel_z).normalized;
        //direction = Vector3.one.normalized;  // direction is (1,1,1) normalized
        //direction = new Vector3(1.0,0,1.0).normalized;
        radius = transform.localScale.x / 2; // half the ball width
    }

    // Update is called once per frame
    void Update()
    {
        if (Ball_is_moving == true)
        {
            // move Ball to next position
            transform.Translate(direction * speed * Time.deltaTime);
            // Inc score when hitting player side + PointFX
            if (transform.position.x <= Lo_Bound_X + radius && direction.x < 0) { GameManager.IncPlayerScore(2); SFXPlaying.PlayPointFX(); }
            if (transform.position.x >= Hi_Bound_X - radius && direction.x > 0) { GameManager.IncPlayerScore(1); SFXPlaying.PlayPointFX(); }

            // reverse velocity if boundary reached
            if (transform.position.x <= Lo_Bound_X + radius && direction.x < 0) direction.x = -direction.x;
            if (transform.position.y <= Lo_Bound_Y + radius && direction.y < 0) direction.y = -direction.y;
            if (transform.position.z <= Lo_Bound_Z + radius && direction.z < 0) direction.z = -direction.z;
            if (transform.position.x >= Hi_Bound_X - radius && direction.x > 0) direction.x = -direction.x;
            if (transform.position.y >= Hi_Bound_Y - radius && direction.y > 0) direction.y = -direction.y;
            if (transform.position.z >= Hi_Bound_Z - radius && direction.z > 0) direction.z = -direction.z;
        }
    }

    /*
    private void OnCollisionEnter(Collision col)  // this function is called in any collision
    {
        Debug.Log("Collision");
    }
    */
}
