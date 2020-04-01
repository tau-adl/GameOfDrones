using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    private GameManager GameManager;
    private SFXPlaying SFXPlaying;

    public bool menu_is_on = false;

    public Button SoundOn;
    public Button SoundOff;

    public Button LightOn;
    public Button LightOff;

    public Button Antiband_Off;
    public Button Antiband_50Hz;
    public Button Antiband_60Hz;

    public Button FocusMode_Normal;
    public Button FocusMode_TrigAuto;
    public Button FocusMode_ContAuto;
    public Button FocusMode_Infinity;
    public Button FocusMode_Macro;

    public Button VideoMode_Default;
    public Button VideoMode_Speed;
    public Button VideoMode_Quality;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindObjectOfType<GameManager>();
        SFXPlaying = GameObject.FindObjectOfType<SFXPlaying>();
    }


    public void toggle_menu()
    {
        Debug.Log("menu, toggle_menu: menu_is_on="+ menu_is_on);
        if (menu_is_on)
        {
            SoundOn.gameObject.SetActive(false);
            SoundOff.gameObject.SetActive(false);

            LightOn.gameObject.SetActive(false);
            LightOff.gameObject.SetActive(false);

            Antiband_Off.gameObject.SetActive(false);
            Antiband_50Hz.gameObject.SetActive(false);
            Antiband_60Hz.gameObject.SetActive(false);

            FocusMode_Normal.gameObject.SetActive(false);
            FocusMode_TrigAuto.gameObject.SetActive(false);
            FocusMode_ContAuto.gameObject.SetActive(false);
            FocusMode_Infinity.gameObject.SetActive(false);
            FocusMode_Macro.gameObject.SetActive(false);

            VideoMode_Default.gameObject.SetActive(false);
            VideoMode_Speed.gameObject.SetActive(false);
            VideoMode_Quality.gameObject.SetActive(false);
        }
        else
        {
            // show/hide Sound button
            if (SFXPlaying.sound_is_on)  { SoundOn.gameObject.SetActive(true);  }
            else                         { SoundOff.gameObject.SetActive(true); }

            // show/hide Light button
            if (GameManager.light_is_on) { LightOn.gameObject.SetActive(true);  }
            else                         { LightOff.gameObject.SetActive(true); }

            // show/hide Antiband button
            if      (GameManager.antibanding==0) { Antiband_Off.gameObject.SetActive(true);  }
            else if (GameManager.antibanding==1) { Antiband_50Hz.gameObject.SetActive(true); }
            else                                 { Antiband_60Hz.gameObject.SetActive(true); }

            // show/hide FocusMode button
            if      (GameManager.focus_mode == 0) { FocusMode_Normal.gameObject.SetActive(true);   }
            else if (GameManager.focus_mode == 1) { FocusMode_TrigAuto.gameObject.SetActive(true); }
            else if (GameManager.focus_mode == 2) { FocusMode_ContAuto.gameObject.SetActive(true); }
            else if (GameManager.focus_mode == 3) { FocusMode_Infinity.gameObject.SetActive(true); }
            else                                  { FocusMode_Macro.gameObject.SetActive(true);    }

            // show/hide Antiband button
            if      (GameManager.video_mode == 0) { VideoMode_Default.gameObject.SetActive(true); }
            else if (GameManager.video_mode == 1) { VideoMode_Speed.gameObject.SetActive(true);   }
            else                                  { VideoMode_Quality.gameObject.SetActive(true); }
        }

        menu_is_on = !menu_is_on;
    }

}
