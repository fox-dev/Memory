using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Gameplay results UI script
public class ResultsUI : MonoBehaviour {

    int progressBarValue = 0; //Percentage of fill amount

    //Canvas locator object, set in inspector
    public GameObject canvasLocator;

    [SerializeField] //song title on results screen object set in insepctor
    RectTransform songTitleCard;
    Text songTitleText;

    [SerializeField] //artist on results screen object set in insepctor
    RectTransform artistCard;
    Text artistText;

    [SerializeField]
    RectTransform dropsCard; //drops/misses object, assign in insepctor
    Text dropsText;
    float drops, dropsStart = 0; //Get from game manager


    [SerializeField]
    RectTransform bestComboCard; //best combo for song object, assign in inspector
    Text bestComboText;
    float bestCombo, bestComboStart = 0; //Get from game manager

    [SerializeField]
    RectTransform beatsHitCard; //number of correct patterns hit object, assign in inspector
    Text beatsHitText;
    float beatsHit, beatsHitStart = 0; //Get from game manager

    [SerializeField]
    RectTransform scoreCard; //score object, assign in inspector
    Text scoreText;
    float score, scoreStart = 0; //Get from game manager

    [SerializeField] //Ring that fills to show player result
    RectTransform resultRing;
    Image resultImg;

    [SerializeField] //center black ring
    RectTransform centerRing;
    Image centerImg;

    [SerializeField]  //Testing//
    RectTransform percentCard; //Percentage fill
    Text percentText;

    //Elapsed times/Finish time to lerp resultImg fillamount in seconds for lerp animations
    float elapsedTime_resultRing = 0;
    float elapsedTime_centerRing = 0;
    float elapsedTime_global = 0; //All number text animate at same time;
    float finishTime_resultRing = 2f; //finish in time seconds
    float finishTime_centerRing = 0.5f; //finish in time seconds
    float finishTime_global = 2f; //global finish animate time in seconds
    float slowFactor = 0;

	// Use this for initialization
	void Awake () {
        //Assign text components//
        percentText = percentCard.GetComponent<Text>();
        resultImg = resultRing.GetComponent<Image>();
        resultImg.fillAmount = 0;
        centerImg = centerRing.GetComponent<Image>();
        centerImg.fillAmount = 0;

        songTitleText = songTitleCard.GetComponent<Text>();
        artistText = artistCard.GetComponent<Text>();
        dropsText = dropsCard.GetComponent<Text>();
        bestComboText = bestComboCard.GetComponent<Text>();
        beatsHitText = beatsHitCard.GetComponent<Text>();
        scoreText = scoreCard.GetComponent<Text>();

        //////////////////////////

        progressBarValue = 0;
    }

    private void Start()
    {
        percentText.text = "0%";
        dropsText.text = bestComboText.text = beatsHitText.text = scoreText.text = "0";
    }

    private void OnDisable()
    {
        //Reset fields
        percentText.text = "0%";

        dropsText.text = bestComboText.text = beatsHitText.text = scoreText.text = "0";
        drops = bestCombo = beatsHit = score = 0;
        dropsStart = bestComboStart = beatsHitStart = scoreStart = 0; //starting values for the lerp

        elapsedTime_centerRing = 0;
        elapsedTime_resultRing = 0;
        elapsedTime_global = 0;
        resultImg.fillAmount = 0;
        centerImg.fillAmount = 0;

        //Reset animation lerp scales
        foreach (RectTransform item in canvasLocator.transform)
        {
            if (item.tag == "Results_Card")
            {
                item.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            }
        }

    }

    private void OnEnable()
    {
        drops = (int)GameManager.playerData.misses;
        bestCombo = GameManager.playerData.maxCombo;
        beatsHit = (int)GameManager.playerData.correctHits;
        score = GameManager.gm.GetScore();

        songTitleText.text = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].title;
        artistText.text = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].artist;

    }



    // Update is called once per frame
    void Update () {
        if (GameManager.gm.currentState == GameManager.GameState.results)
        {
            lerpToMax_CenterRing();
        }

        if(centerImg.fillAmount >= 1f)
        {
            lerpToMax_ResultRing();
            lerpNumbers();
        }
        
        //customGrow(0.005f, 0.001f, 50);
            

	}

    //Linear interpolation from 0 to 100
    void lerpToMax_ResultRing()
    {
        elapsedTime_resultRing += Time.deltaTime;
        resultImg.fillAmount = Mathf.Lerp(0f, GameManager.gm.getPlayerPerformance(), elapsedTime_resultRing / finishTime_resultRing);
        percentText.text = (resultImg.fillAmount * 100).ToString("F0") + "%";

        //Lerp result cards in
        foreach(RectTransform item in canvasLocator.transform)
        {
            if(item.tag == "Results_Card")
            {
                item.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(0, 1, 1), new Vector3(1, 1, 1), elapsedTime_resultRing / 0.25f);
            }
        }
    }
    //Linear interpolation from 0 to 100
    void lerpToMax_CenterRing()
    {
        elapsedTime_centerRing += Time.deltaTime;
        centerImg.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime_centerRing / finishTime_centerRing);
    }

    //Linear interpolation for drops/misses, bestcombo, beatshit, score
    void lerpNumbers()
    {
        elapsedTime_global += Time.deltaTime;
        dropsStart = Mathf.Lerp(0, drops, elapsedTime_global / finishTime_global);
        bestComboStart = Mathf.Lerp(0, bestCombo, elapsedTime_global / finishTime_global);
        beatsHitStart = Mathf.Lerp(0, beatsHit, elapsedTime_global / finishTime_global);
        scoreStart = Mathf.Lerp(0, score, elapsedTime_global / finishTime_global);

        dropsText.text = ((int)dropsStart).ToString();
        bestComboText.text = ((int)bestComboStart).ToString();
        beatsHitText.text = ((int)beatsHitStart).ToString();
        scoreText.text = ((int)scoreStart).ToString();
    }





    void customGrow(float startGrowthSpd, float slowGrowthSpd,int percentToSlowAt)
    {
        elapsedTime_resultRing += Time.deltaTime;

        float percent = percentToSlowAt / 100f;

        if(resultImg.fillAmount < 0.8f)
        {
            if (resultImg.fillAmount <= percent)
            {
                resultImg.fillAmount += startGrowthSpd;

            }
            else
            {
                slowFactor = Mathf.Lerp(startGrowthSpd, slowGrowthSpd, elapsedTime_resultRing / (2f * 1/4));
                resultImg.fillAmount += slowFactor;
            }
        }

        percentText.text = (resultImg.fillAmount * 100).ToString("F0") + "%";

    }

}
