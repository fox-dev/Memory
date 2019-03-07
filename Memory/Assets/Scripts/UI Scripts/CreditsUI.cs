using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : MonoBehaviour {

	public void GoToMenu()
    {
        GameManager.gm.currentState = GameManager.GameState.menu;
    }

    public void Link_HyperPotions()
    {
        Application.OpenURL("http://www.hyperpotions.com/");
    }

    public void Link_MakaihBeats()
    {
        Application.OpenURL("https://makaihbeats.net/");
    }

    public void Link_SethPower()
    {
        Application.OpenURL("http://sethpowermusic.com/band/");
    }
}
