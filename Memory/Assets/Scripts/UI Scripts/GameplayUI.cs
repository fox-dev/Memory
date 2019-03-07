using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour {

    public static GameplayUI gpUI;

    //score text on gameplay ui, set in inspector
    [SerializeField]
    private RectTransform scoreCard;
    private Text scoreText;
    //combo text on gameplay ui, set in inspector
    [SerializeField]
    private RectTransform comboTextCard;
    private Text comboText;
    [SerializeField]
    private RectTransform comboTextParent; //"COMBO" text card
    //pause button set in inspect
    [SerializeField]
    private Button pauseButton; 

    //Shake effect for combo
    private bool shaking = false;
    Vector2 originalPos;
    Color startColor;

    //Hide combo after not having increased after certain amount of time
    float elapsedTime = 0;
    float hideTime = 2f; //Amount passed before hiding;


    private void Awake()
    {
        gpUI = this;
    }

    // Use this for initialization
    void Start () {
        scoreText = scoreCard.GetComponent<Text>();
        comboText = comboTextCard.GetComponent<Text>();
        originalPos = comboTextCard.anchoredPosition;
        startColor = comboTextParent.GetComponent<Text>().color;

    }
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: "  + GameManager.gm.GetScore().ToString();
        comboText.text = GameManager.gm.GetCombo().ToString();

        elapsedTime += Time.deltaTime;

        if(shaking)
        {
            Vector2 shakePos = new Vector2(comboTextCard.anchoredPosition.x, -60f);

            comboTextCard.anchoredPosition = shakePos;

        }
        else
        {
            comboTextCard.anchoredPosition = Vector2.Lerp(comboTextCard.anchoredPosition, originalPos, 10f * Time.deltaTime);
        }

        if(elapsedTime >= hideTime)
        {
            //comboTextParent.gameObject.SetActive(false);
            comboTextParent.GetComponent<Text>().color = Color.Lerp(comboTextParent.GetComponent<Text>().color, Color.clear, Time.deltaTime * 5f);
            comboText.color = Color.Lerp(comboTextParent.GetComponent<Text>().color, Color.clear, Time.deltaTime * 5f);
            comboTextCard.anchoredPosition = originalPos;
        }

        //Disable pause button during readyState
        if(GameManager.gm.currentState == GameManager.GameState.ready)
        {
            pauseButton.interactable = false;
        }
        else
        {
            pauseButton.interactable = true;
        }



    }

    public void shakeIt()
    {
        StartCoroutine(shake());
    }

    //Shake combo # text
    IEnumerator shake()
    {
        // Vector2 originalPos = comboTextCard.anchoredPosition;
        //comboTextParent.gameObject.SetActive(true);
        comboTextParent.GetComponent<Text>().color = startColor;
        comboText.color = startColor;
        elapsedTime = 0;

        if(!shaking)
        {
            shaking = true;
        }

        yield return new WaitForSeconds(0f);

        shaking = false;
       // comboTextCard.anchoredPosition = originalPos;
        
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        AudioPeer._audioSource.Pause();
        GameManager.gm.previousState = GameManager.gm.currentState;
        GameManager.gm.currentState = GameManager.GameState.pause;
    }
}
