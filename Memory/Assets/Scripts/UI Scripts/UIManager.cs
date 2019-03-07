using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script controls which UI are active during each state
public class UIManager : MonoBehaviour {

    [SerializeField]
    private RectTransform songEnd_UI; //set in inspector, ui for end of song
    [SerializeField]
    private RectTransform mainMenu_UI; //set in inspector, ui for main menu
    [SerializeField]
    private RectTransform credits_UI; //set in inspector, credits ui
    [SerializeField]
    private RectTransform musicSelect_UI; //set in inspector, ui for music select
    [SerializeField]
    private RectTransform gameplay_UI; //set in inspector, ui for main gameplay
    [SerializeField]
    private RectTransform pause_UI; //set in inspector, ui for pause ui
    [SerializeField]
    private RectTransform results_UI; //set in inspector, ui for endgame results

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(GameManager.gm.currentState == GameManager.GameState.menu)
        {
            disableAllOtherUI(mainMenu_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.credits)
        {
            disableAllOtherUI(credits_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            disableAllOtherUI(musicSelect_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.endingPhase)
        {
            disableAllOtherUI(songEnd_UI, results_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.pause)
        {
            disableAllOtherUI(pause_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.results)
        {
            disableAllOtherUI(results_UI);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.ready || GameManager.gm.currentState == GameManager.GameState.waiting || 
            GameManager.gm.currentState == GameManager.GameState.memoryRecall || 
            GameManager.gm.currentState == GameManager.GameState.memoryRelay)
        {
            disableAllOtherUI(gameplay_UI);
        }
        else //most UI disabled
        {
            disableAllUI();
        }
	}

    //disables all UI except the one passed in
    void disableAllOtherUI(RectTransform activeMenu)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("UI");

        foreach(GameObject m in menus)
        {
            if(m.name != activeMenu.name)
            {
                m.GetComponentInChildren<CanvasLocator>().Generic_Disable();
            }
        }

        activeMenu.gameObject.SetActive(true);

    }

    //Overloaded function
    //disables all UI except the one passed in
    void disableAllOtherUI(RectTransform activeMenu, RectTransform activeMenu2)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("UI");

        foreach (GameObject m in menus)
        {
            if (m.name != activeMenu.name && m.name != activeMenu2.name)
            {
                m.GetComponentInChildren<CanvasLocator>().Generic_Disable();
            }
        }

        activeMenu.gameObject.SetActive(true);

    }

    void disableAllUI()
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("UI");

        foreach (GameObject m in menus)
        {
            //m.SetActive(false);
            m.GetComponentInChildren<CanvasLocator>().Generic_Disable();
        }
    }

}
