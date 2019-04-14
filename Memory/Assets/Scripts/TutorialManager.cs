using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages the stats and progression throughout the tutorial song
/// <summary>
/// Tutorial progression is as follows:
// 0. Tell players to memorize the order of enemies;
// 1. Tell player to tap the order enemies come on screen
// 2. Tell player that once all enemies in a chain are tapped, tap the center to shoot
// 3 Tell player that they will receive bonus points for tapping with the beat, when the center square turns red;
// 4. Tell player that matching same the number of beats as the pattern nets bonus points
// 5. Enable failing, if the pattern isn't given in time, player will lose points and be given a new pattern
/// </summary>
public class TutorialManager : MonoBehaviour {

    //For global access and reference
    public static TutorialManager tm;

    //Steps listed above, active step is the current step in the tutorial
    public bool step0, step1, step2, step3, step4, step5;


    [SerializeField]
    private int clearsToStep4 = 1; //Requires 2 board clears during step2 to get to step 4
    [SerializeField]
    private int clearsToStep5 = 1; //Requires 2 board clears during step2 to get to step 5
    [SerializeField]
    private int clearsInStep3 = 0; //Number of clears in step3
    [SerializeField]
    private int clearsInStep4 = 0; //Number of clears in step4

    //Tutorial Message Positions//
    public RectTransform messageObject; //set in inspector
    Text tutorialMessage;
    Vector3 onScreen_position = new Vector3(0, -150, 0);
    Vector3 offScreen_position = new Vector3(0, -400, 0);

    bool message0_Shown, message1_Shown, message2_Shown, message3_Shown, message4_Shown, message5_Shown;

    private void Awake()
    {
        tm = this;
    }
    // Use this for initialization
    void Start () {

        step0 = step1 = step2 = step3 = step4 = step5 = false;
        message0_Shown = message1_Shown = message2_Shown = message3_Shown = message4_Shown = message5_Shown = false;
        tutorialMessage = messageObject.GetComponentInChildren<Text>();

		
	}

    void resetTutorial()
    {
        step0 = step1 = step2 = step3 = step4 = step5 = false;
        message0_Shown = message1_Shown = message2_Shown = message3_Shown = message4_Shown = message5_Shown = false;
        messageObject.localPosition = offScreen_position;
        messageObject.gameObject.SetActive(true);

        clearsInStep3 = 0;
        clearsInStep4 = 0;
    }

    private void Update()
    {
        if(step0 && !message0_Shown) //First message, toggled in gameManager
        {
            ChangeMessage();
        }

        if(step1 && !message1_Shown)
        {
            ChangeMessage();
        }

        if (GameManager.gm.currentState == GameManager.GameState.menu || GameManager.gm.currentState == GameManager.GameState.musicSelect || GameManager.gm.currentState == GameManager.GameState.endingPhase)
        {
            resetTutorial();
        }
    }

    public void incrementClears()
    {
 
        if (step3)
        {
            clearsInStep3 += 1;
        }

        if (step4)
        {
            clearsInStep4 += 1;
        }

        // 3 -> 4
        if (clearsInStep3 == clearsToStep4)
        {
            step4 = true;
            ChangeMessage();
        }

        //4 -> 5
        if (clearsInStep4 == clearsToStep5)
        {
            step5 = true;
        }
    }

    //Tutorial messages
    public void ChangeMessage()
    {
        if (step0 && !message1_Shown)
        {
            tutorialMessage.text = "Memorize the order the enemies come in.";
            message0_Shown = true;
            StartCoroutine(lerpMessage());
        }

        if (step1 && !message1_Shown)
        {
            tutorialMessage.text = "Enemy groups that turned blue all at once are a chain. Tap the order the enemy chain came on-screen.";
            message1_Shown = true;
            StartCoroutine(lerpMessage());
        }

        if(step2 && !message2_Shown)
        {
            message2_Shown = true;
            tutorialMessage.text = "Once all enemies in a chain are tapped, press the center square. Incorrect taps show the next correct order at the cost of points.";
            StartCoroutine(lerpMessage());
        }

        if (step3 && !message3_Shown)
        {
            message3_Shown = true;
            tutorialMessage.text = "Use the beat to tap when the center square is red for bonus points at the end of a pattern.";
            StartCoroutine(lerpMessage());
        }

        if (step4 && !message4_Shown) //Step 4 enabled in Player Visualizer Script
        {
            message4_Shown = true;
            tutorialMessage.text = "Match the number of beats of the given pattern for even more points.";
            StartCoroutine(lerpMessage());
        }

        if (step5 && !message5_Shown)
        {
            message5_Shown = true;
            tutorialMessage.text = "Take too long on a pattern and you'll lose points.";
            StartCoroutine(lerpMessage());
            StartCoroutine(disableMessage());
        }


    }

    public IEnumerator lerpMessage()
    {

        messageObject.localPosition = offScreen_position;
        float startTime = Time.time;
        while (messageObject.localPosition != onScreen_position)
        {

            messageObject.localPosition = Vector3.Lerp(offScreen_position, onScreen_position, (Time.time - startTime) / 0.25f);
            yield return null;
        }

    }

    public IEnumerator disableMessage()
    {
        yield return new WaitForSeconds(12f);
        messageObject.gameObject.SetActive(false);
    }





}
