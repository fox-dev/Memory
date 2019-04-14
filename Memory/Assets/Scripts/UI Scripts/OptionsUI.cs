using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject BeatCounterButton_On;
    [SerializeField]
    private GameObject BeatCounterButton_Off;
    [SerializeField]
    private GameObject BeatSoundButton_On;
    [SerializeField]
    private GameObject BeatSoundButton_Off;


    public void Start()
    {
        SetDefaults();
    }

    public void OnEnable()
    {
        SetDefaults();
    }

    public void GoToMenu()
    {
        GameManager.gm.currentState = GameManager.GameState.menu;
    }

    //Button function
    public void EnableBeatCounter_On()
    {
        Color tempColor_Off = BeatCounterButton_Off.GetComponent<Image>().color;
        Color tempColor_On = BeatCounterButton_Off.GetComponent<Image>().color;
        tempColor_Off.a = 0.1f;
        tempColor_On.a = 1f;
        BeatCounterButton_Off.GetComponent<Image>().color = tempColor_Off;
        BeatCounterButton_On.GetComponent<Image>().color = tempColor_On;

        GameManager.gm.option = GameManager.Options.count_On;


    }

    //Button function
    public void EnableBeatCounter_Off()
    {
        Color tempColor_Off = BeatCounterButton_Off.GetComponent<Image>().color;
        Color tempColor_On = BeatCounterButton_Off.GetComponent<Image>().color;
        tempColor_Off.a = 0.1f;
        tempColor_On.a = 1f;
        BeatCounterButton_Off.GetComponent<Image>().color = tempColor_On;
        BeatCounterButton_On.GetComponent<Image>().color = tempColor_Off;

        GameManager.gm.option = GameManager.Options.count_Off;
    }

    //Button function
    public void EnableBeatSound_On()
    {
        Color tempColor_Off = BeatSoundButton_On.GetComponent<Image>().color;
        Color tempColor_On = BeatSoundButton_Off.GetComponent<Image>().color;
        tempColor_Off.a = 0.1f;
        tempColor_On.a = 1f;
        BeatSoundButton_Off.GetComponent<Image>().color = tempColor_Off;
        BeatSoundButton_On.GetComponent<Image>().color = tempColor_On;

        GameManager.gm.beatSound = GameManager.BeatSound.beatSound_On;
    }

    //Button function
    public void EnableBeatSound_Off()
    {
        Color tempColor_Off = BeatSoundButton_Off.GetComponent<Image>().color;
        Color tempColor_On = BeatSoundButton_Off.GetComponent<Image>().color;
        tempColor_Off.a = 0.1f;
        tempColor_On.a = 1f;
        BeatSoundButton_Off.GetComponent<Image>().color = tempColor_On;
        BeatSoundButton_On.GetComponent<Image>().color = tempColor_Off;

        GameManager.gm.beatSound = GameManager.BeatSound.beatSound_Off;
    }



    public void SetDefaults()
    {
        Color tempColor_Off = BeatCounterButton_On.GetComponent<Image>().color;
        Color tempColor_On = BeatCounterButton_On.GetComponent<Image>().color;

        //Count options
        if (GameManager.gm.option == GameManager.Options.count_Off)
        {
            tempColor_Off.a = 0.1f;
            tempColor_On.a = 1f;
            BeatCounterButton_Off.GetComponent<Image>().color = tempColor_On;
            BeatCounterButton_On.GetComponent<Image>().color = tempColor_Off;
        }
        else
        {
            tempColor_Off.a = 0.1f;
            tempColor_On.a = 1f;
            BeatCounterButton_Off.GetComponent<Image>().color = tempColor_Off;
            BeatCounterButton_On.GetComponent<Image>().color = tempColor_On;
        }

        //Beat Sound Options
        if (GameManager.gm.beatSound == GameManager.BeatSound.beatSound_Off)
        {
            tempColor_Off.a = 0.1f;
            tempColor_On.a = 1f;
            BeatSoundButton_Off.GetComponent<Image>().color = tempColor_On;
            BeatSoundButton_On.GetComponent<Image>().color = tempColor_Off;
        }
        else
        {
            tempColor_Off.a = 0.1f;
            tempColor_On.a = 1f;
            BeatSoundButton_Off.GetComponent<Image>().color = tempColor_Off;
            BeatSoundButton_On.GetComponent<Image>().color = tempColor_On;
            
        }
    }
}
