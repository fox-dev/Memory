using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

    
    public void PlayGame()
    {
        AudioPeer.ap.changeSong_MusicSelectTransition(2);
        GameManager.gm.currentState = GameManager.GameState.ready;
    }

    public void GoToMusicSelect()
    {
        GameManager.gm.currentState = GameManager.GameState.musicSelect;
    }

    public void StartTutorial()
    {
        AudioPeer.ap.changeSong_MusicSelectTransition(0); //0 index is the tutorial song
        GameManager.gm.currentState = GameManager.GameState.ready;
    }

    public void GoToCredits()
    {
        GameManager.gm.currentState = GameManager.GameState.credits;
    }
}
