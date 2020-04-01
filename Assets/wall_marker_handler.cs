using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_marker_handler : MonoBehaviour
{
    float player_height = 2;     // from PlayerUnit->Graphics
    private int board_y = 40;    // +1 for board height (so board_y is the upper side of the board)

    PlayerUnit PlayerUnit;
    BallMovement BallMovement;
    public GameObject wall_marker;
    Renderer MarkerRenderer;

    Vector3 player_position;
    float   player_top, player_bottom;
    Vector3 ball_position;
    bool player_above_board, player_below_board, player_above_ball;
    float distance_from_board;
    Vector3 position_in_board_plane;


    private void Start()
    {
        //PlayerUnit = GameObject.FindObjectOfType<PlayerUnit>();
        //BallMovement = GameObject.FindObjectOfType<BallMovement>();

        //Get the Renderer component from the new cube
        //var MarkerRenderer = wall_marker.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerUnit = GameObject.FindObjectOfType<PlayerUnit>();
        BallMovement = GameObject.FindObjectOfType<BallMovement>();
        MarkerRenderer = wall_marker.GetComponent<Renderer>();

        if (PlayerUnit != null)
        {
            //Debug.Log("wall_marker_handler, Update: PlayerUnit found");
            player_position = PlayerUnit.transform.position;
            player_top    = player_position.y + (player_height / 2);
            player_bottom = player_position.y - (player_height / 2); // get bottom point of player
            ball_position = BallMovement.transform.position;

            // indications
            player_above_board = (player_bottom > board_y + 0.0001);
            player_below_board = (player_top < board_y);
            player_above_ball  = (player_bottom > ball_position.y + BallMovement.radius);
            
            /*
            // Debug Zone
            Debug.Log("wall_marker_handler, Update: player position=(" + player_position.x + ", " + player_position.y + ", " + player_position.z + ")");
            Debug.Log("wall_marker_handler, Update: ball   position=(" + ball_position.x   + ", " + ball_position.y   + ", " + ball_position.z   + ")");
            if (player_above_board) { Debug.Log("^^^^^ ABOVE BOARD ^^^^^"); }
            else if (player_below_board) { Debug.Log("||||| BELOW BOARD |||||"); }
            if      (player_above_ball)  { Debug.Log("^^^^^ ABOVE BALL ^^^^^"); }
            */

            // calculate distance from board
            if (player_above_board) { distance_from_board = player_bottom - board_y; }
            else                    { distance_from_board = board_y - player_top;    }

            // change marker size
            if      (player_above_ball)  { wall_marker.transform.localScale = new Vector3(1f, 0.1f, 1f);  }
            else if (player_below_board) { wall_marker.transform.localScale = new Vector3(1f, 0.1f, 1f);  }
            else                         { wall_marker.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f); }

            // change marker color according to player position (above/below board)
            if      (player_above_ball)  { MarkerRenderer.material.SetColor("_Color", Color.blue);  }
            else if (player_below_board) { MarkerRenderer.material.SetColor("_Color", Color.red);   }
            else                         { MarkerRenderer.material.SetColor("_Color", Color.green); }

            // update marker position
            position_in_board_plane = player_position;
            position_in_board_plane.y = board_y +0.001f;
            wall_marker.transform.position = position_in_board_plane;
        }
    }
}
