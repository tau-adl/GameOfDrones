using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlaying : MonoBehaviour
{
    public bool sound_is_on = false;

    public AudioSource PointFX;
    public AudioSource BounceFX;
    public AudioSource StartFX;

    // Sound ON/OFF
    public void set_sound_on()
    {
        sound_is_on = true;
        Debug.Log("Sound ON");
    }
    public void set_sound_off()
    {
        sound_is_on = false;
        Debug.Log("Sound OFF");
    }

    public void PlayPointFX()
    {
        if (sound_is_on) PointFX.Play();
    }

    public void PlayBounceFX()
    {
        if (sound_is_on) BounceFX.Play();
    }

    public void PlayStartFX()
    {
        if(sound_is_on) StartFX.Play();
    }

}
