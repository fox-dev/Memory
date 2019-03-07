using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Song End button UI//
public class SongEndUI : MonoBehaviour {

    //Buttons// set in inspector
    public Button selectMusic;
    public Button replay;
    public Button menu;

    //OnScreen positions
    private Vector3 selectMusic_onPos = new Vector2(-318, -100);
    private Vector3 replay_onPos = new Vector2(-318, -150);
    private Vector3 menu_onPos = new Vector2(-318, -200);

    //Offscreen positions
    //OnScreen positions
    private Vector3 selectMusic_offPos = new Vector2(-520, -100);
    private Vector3 replay_offPos = new Vector2(-520, -150);
    private Vector3 menu_offPos = new Vector2(-520, -200);


    private void Awake()
    {
        SetDefaults();
    }
    private void OnEnable()
    {
        StartCoroutine(lerpButtons());
    }

    private void OnDisable()
    {
        SetDefaults();
    }

    public void ReplaySong()
    {
        GameManager.gm.currentState = GameManager.GameState.ready;
    }

    public void GoToMusicSelect()
    {
        GameManager.gm.currentState = GameManager.GameState.musicSelect;
    }

    public void GoToMainMenu()
    {
        GameManager.gm.currentState = GameManager.GameState.menu;
    }

    //Lerping animations per button// each button has a different finishTime
    IEnumerator lerpButtons()
    {
        float startTime = Time.time;

        while (menu.GetComponent<RectTransform>().localPosition != menu_onPos)
        {
            //select music button 1st
            selectMusic.GetComponent<RectTransform>().localPosition = Vector3.Lerp(selectMusic_offPos, selectMusic_onPos, (Time.time - startTime) / 0.4f);
            //Replay button 2nd
            replay.GetComponent<RectTransform>().localPosition = Vector3.Lerp(replay_offPos, replay_onPos, (Time.time - startTime) / 0.5f);
            //Menu last
            menu.GetComponent<RectTransform>().localPosition = Vector3.Lerp(menu_offPos, menu_onPos, (Time.time - startTime) / 0.6f);

            yield return null;

        }
        
    }

    public void SetDefaults()
    {
        //Default button positions are off-screen
        selectMusic.GetComponent<RectTransform>().localPosition = selectMusic_offPos;
        replay.GetComponent<RectTransform>().localPosition = replay_offPos;
        menu.GetComponent<RectTransform>().localPosition = menu_offPos;
    }


}
