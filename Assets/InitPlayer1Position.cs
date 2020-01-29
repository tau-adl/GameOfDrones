using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayer1Position : MonoBehaviour
{
    void Start()
    {
        this.transform.Rotate(new Vector3(0, 180, 0));
        this.transform.Translate(new Vector3(2.5f, 11, 10));
    }
}
