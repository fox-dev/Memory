using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pause menu UI handling
public class PauseUI : MonoBehaviour {

    public void resumeGame()
    {
        GameManager.gm.currentState = GameManager.gm.previousState;
        AudioPeer._audioSource.UnPause();
        Time.timeScale = 1;
    }

    public void returnToMusicSelect()
    {

        GameManager.gm.QuitGameplayPhase(GameManager.GameState.musicSelect);
        Time.timeScale = 1;

    }
}
