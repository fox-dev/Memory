using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //Gamestates//
    public enum GameState
    {
        test, //testing purposes
        menu, //menu state for main menu
        credits, //credits menu
        options, //options menu
        tutorial, //tutorial music stage
        musicSelect, //music select state;
        ready, //state to last x seconds before moving to normalPlay, for any transition elements
        normalPlay, //main gameplay state, may remove later
        memoryRelay, //phase for enemies to relay pattern to player
        memoryRecall, //phase for player to input the pattern
        waiting, //for any transitions into normal gameplay -> memoryRelay
        endingPhase, //song has ended, maybe transition into another phase from here
        pause, //paused gamestate
        results, //results gameState
        gameOver //gameover
    }

    //Options//
    public enum Options
    {
        count_On, //shows beat count in phases;
        count_Off 
    }

    public enum BeatSound
    {
        beatSound_On, // sound of middle cube beat
        beatSound_Off
    }

    //Difficulty// difficulty sets
    public enum Difficulty
    {
        easy,
        normal,
        hard
    }


    public static GameManager gm; //static variable single instance of gamemanger reference, for use in other classes to change gm states
    public EnemyRelayManager erm; //private reference to erm;

    public GameState currentState; //current gameplay state
    public GameState previousState; //previous state
    public Options option = Options.count_Off; //Default off
    public BeatSound beatSound = BeatSound.beatSound_On; //Default on

    public Difficulty difficulty; //game difficulty

    public Player player;

    float ElapsedTime;// FinishTime; //Start and End time for moving object, in seconds

    public List<GameObject> targets;

    public List<float> clickTimes;
    float clickTime = 0f;
    public int numClicks = 0;

    //Set in inspector, # of beats player can go over before fail given
    public int beatsTillFail = 0;

    //make sure coroutines are called once in update
    bool occupied = false;

    //Screen image when relaying pattern, enabled during relayphase, disabled during, set in inspector
    public GameObject screener;
    //Results after tapping should appear only once after memoryRecall phase
    bool gotResult = false;
    //Ring particle should appear once per phase change
    bool spawnedRing_recall = false;
    bool spawnedRing_relay = false;
    //game has ended and enemies have been sent to starting positions
    bool reset = false;




    //Results of current memoryRecall phase shown
    public bool resultsShown = false;

    //PLAYER DATA//
    [SerializeField]
    public static ScoreData playerData;
    [SerializeField]
    public static SaveData playerSave;
    //Player grade at the end of a song
    [SerializeField]
    private float grade = 0;



    // Use this for initialization
    void Awake () {
        //Clear Save//
        //PlayerPrefs.DeleteAll();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;


        if (gm != null)
        {
            Debug.LogError("More than one GameManager in scene");
        }
        else
        {
            gm = this;
        }

        ElapsedTime = 0;
        //FinishTime = 0;

        playerSave = new SaveData();
        playerData = new ScoreData();
        //reset player_SongData
        playerData.reset();
        //load playerSaveData
        

        //Testing
        Application.runInBackground = true;
        
    }

    void Start()
    {
        
        //assign main erm reference
        if (erm != null)
        {
            Debug.LogError("more than one erm in scene");

        }
        else
        {
            erm = EnemyRelayManager.erm;
        }

        FloatingTextController.Init();
        SlidingTextController.Init();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        targets = new List<GameObject>();
        clickTimes = new List<float>();

        //init playerSave
        playerSave.init();
        //load playerSaveData
        try
        {
            playerData.Load_Json();
        }
        catch(System.Exception e)
        {
            //Something went wrong loading, player prefs did not match current save
            Debug.Log(e.ToString());

            //Clear player prefs
            PlayerPrefs.DeleteAll();

            //Load
            playerData.Load_Json();
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerData.perfectRun > 0)
        {
            grade = (float)((playerData.correctHits - playerData.misses) / playerData.perfectRun);
        }
        
        ElapsedTime += Time.deltaTime; //Start Counting.

        if (GameManager.gm.currentState == GameManager.GameState.ready)
        {
            if (!occupied)
            {
                StartCoroutine(getReady(2f));//Wait 2 seconds before starting gameplay phase + music
            }
        }

        //Tutorial started//
        if(GameManager.gm.currentState == GameManager.GameState.memoryRelay && erm.relayPhaseReady && AudioPeer.ap.tutorial_Playing)
        {
            TutorialManager.tm.step0 = true;
        }

        //Player input phase
        if (currentState == GameState.memoryRecall)
        {
            clickTime += Time.deltaTime;

            //Handle player tapping
            handleInput();

            //Handle player score giving
            if(!AudioPeer.ap.tutorial_Playing)
            {
                checkTimeInRange();
            }
            else
            {
                checkTimeInRange_Tutorial();
            }
            

            screener.SetActive(false);
            if(!spawnedRing_recall)
            {
                FloatingTextController.CreateFloatingText("B E A T", GameObject.Find("Player").transform.position);
                spawnedRing_recall = true;
                GameObject sonarRing = ObjectPooler.current.getPooledObject(Resources.Load("RingSonarParticle") as GameObject);
                if (sonarRing == null) return;
                sonarRing.transform.position = Vector3.zero;
                sonarRing.SetActive(true);
            }
            spawnedRing_relay = false;
        }
        else if (currentState == GameState.waiting)
        {

            resetGameFlags_Waiting();
            
             //for end game flags;
            
        }
        else if (currentState == GameState.memoryRelay && erm.relayPhaseReady)
        {
            screener.SetActive(true);
            if (!spawnedRing_relay)
            {
                FloatingTextController.CreateFloatingText("C H I L L", GameObject.Find("Player").transform.position);
                spawnedRing_relay = true;
                GameObject sonarRing = ObjectPooler.current.getPooledObject(Resources.Load("RingSonarParticle") as GameObject);
                if (sonarRing == null) return;
                sonarRing.transform.position = Vector3.zero;
                sonarRing.SetActive(true);
            }
            spawnedRing_recall = false;


        }
        else if(currentState == GameState.results) //End gameplay phase, reset all flags
        {
            if(!reset)
            {
                reset = true; //Game stats have been reset, only do this once   
                resetGameFlags_GameOver();
            }

            //GameManager.gm.currentState = GameManager.GameState.results;
            if (!occupied)
            {
                StartCoroutine(changeToEndingState());
            }
            

        }

        if(AudioPeer.ap.songEnded() && currentState != GameState.menu && currentState != GameState.musicSelect && !resultsShown)
        {
            GameManager.gm.currentState = GameManager.GameState.results;
            resultsShown = true;
        }

        //reset results shown flag when returning to menu, musicselect, or restarting song
        if(currentState == GameState.menu || currentState == GameState.musicSelect || currentState == GameState.waiting && resultsShown)
        {
            
            /*
            misses = 0; //number of times player tapped incorrectly;
            correctHits = 0; //number of times player tapped correctly;
            perfectRun = 0; //perfect run
            */
            resultsShown = false;
            playerData.reset();
        }
       
    }

    //Quit gameplay phase to return to menu or music select
    public void QuitGameplayPhase(GameState selectState)
    {
        //Stop music
        AudioPeer.ap.QuitPhase_StopMusic();
        resetGameFlags_Quit();
        //State change to be handled by caller, change current state to select state
        currentState = selectState;
    }

    //Game quit, reset flags
    void resetGameFlags_Quit()
    {
        //Visual effect flags set to false
        screener.SetActive(false);
        spawnedRing_recall = false;
        spawnedRing_relay = false;
        gotResult = false;

        //Clear player target visual crosshairs
        player.clearTargetVisuals();
        //Clear player's queued target list
        targets.Clear();

        clickTimes.Clear();
        numClicks = 0;

        //Reset enemyrelaymanager to pre-gameplay
        erm.endGameplayPhase();
    }

    //Game ended, reset flags
    void resetGameFlags_GameOver()
    {
        //Visual effect flags set to false
        screener.SetActive(false);
        spawnedRing_recall = false;
        spawnedRing_relay = false;
        gotResult = false;

        //Clear player target visual crosshairs
        player.clearTargetVisuals();
        //Clear player's queued target list
        targets.Clear();

        clickTimes.Clear();
        numClicks = 0;

        //Reset enemyrelaymanager to pre-gameplay
        erm.endGameplayPhase();

        //Update scores, save data
        SavePlayerData();
       
    }

    //Runs save methods
    void SavePlayerData()
    {
        //Assign highscore to song then reset global score, assign song stats to proper difficulty
        distribDifficultyStats();
        //Save all current song's score difficulty data
        playerData.save_SaveData();
        //Save to player's data in PlayerPrefs
        playerSave.Save_Json();
        //Reset score data
        playerData.reset();
    }

    //For organizaiton, managing difficulty scores
    void distribDifficultyStats()
    {
        //Easy
        if(GameManager.gm.difficulty == GameManager.Difficulty.easy)
        {
            if(playerData.score > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_HighScore)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_HighScore = playerData.score;
            }
            if (playerData.maxCombo > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_MaxCombo)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_MaxCombo = playerData.maxCombo;
            }
        }

        //Normal
        if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
        {
            if (playerData.score > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_HighScore)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_HighScore = playerData.score;
            }
            if (playerData.maxCombo > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_MaxCombo)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_MaxCombo = playerData.maxCombo;
            }
        }

        //Hard
        if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
        {
            if (playerData.score > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_HighScore)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_HighScore = playerData.score;
            }
            if (playerData.maxCombo > AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_MaxCombo)
            {
                AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_MaxCombo = playerData.maxCombo;
            }
        }

    }


    //For waiting state mid-game, reset appropriate flags
    void resetGameFlags_Waiting()
    {
        //Clear player target visual crosshairs
        player.clearTargetVisuals();
        //clear palyer's queued target list
        targets.Clear();

        clickTimes.Clear();
        numClicks = 0;
        
        //Reset visual effect flags 
        screener.SetActive(false);
        gotResult = false;
    }

    void checkTimeInRange()
    {
        if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase == AudioPeer.ap.beatsInRecallPhase && !gotResult) //Perfect
        {
            playerData.perfects += 1;
            FloatingTextController.CreateFloatingText("P E R F E C T I O N", player.transform.position);
            displayScoreChange_Increase(1000, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase < AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 == 0 && !gotResult) //Too slow
        {
            playerData.slows += 1;
            FloatingTextController.CreateFloatingText("S L O W", player.transform.position);
            displayScoreChange_Increase(200, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;


        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase < AudioPeer.ap.beatsInRecallPhase && !gotResult) // Too slow, didnt hit on rhythm
        {
            playerData.slows += 1;
            FloatingTextController.CreateFloatingText("O F F B E A T", player.transform.position);
            displayScoreChange_Increase(125, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase > AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 == 0 && !gotResult) //Fast but on beat
        {
            playerData.fasts += 1;
            FloatingTextController.CreateFloatingText("G R E A T", player.transform.position);
            displayScoreChange_Increase(500, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;


        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase > AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 != 0 && !gotResult) //Early off beat
        {
            playerData.fasts += 1;
            FloatingTextController.CreateFloatingText("E A R L Y", player.transform.position);
            displayScoreChange_Increase(500, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;
        }

        if (AudioPeer.ap.beatsInRecallPhase > AudioPeer.ap.beatsInPhase + beatsTillFail && !gotResult) //No input given, fail, disabled during tutorial song
        {
            playerData.fails += 1;
            if (!occupied)
            {
                StartCoroutine(clearScreen());
            }

            gotResult = true;
        }



    }

    //Custom checkTimeInRange to transition to different setups in the tutorial
    void checkTimeInRange_Tutorial()
    {
        //First thing to enable is step1

        TutorialManager.tm.step1 = true;
        

        if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase == AudioPeer.ap.beatsInRecallPhase && !gotResult) //Perfect
        {
            playerData.perfects += 1;
            FloatingTextController.CreateFloatingText("P E R F E C T I O N", player.transform.position);
            displayScoreChange_Increase(1000, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;
         
            //Enable step3 of tutorial after the first clear
            TutorialManager.tm.step3 = true;
            TutorialManager.tm.incrementClears();
            TutorialManager.tm.ChangeMessage();

        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase < AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 == 0 && !gotResult) //Too slow
        {
            playerData.slows += 1;
            FloatingTextController.CreateFloatingText("S L O W", player.transform.position);
            displayScoreChange_Increase(200, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

            //Enable step3 of tutorial after the first clear
            TutorialManager.tm.step3 = true;
            TutorialManager.tm.incrementClears();
            TutorialManager.tm.ChangeMessage();

        }
        else if(erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase < AudioPeer.ap.beatsInRecallPhase && !gotResult) // Too slow, didnt hit on rhythm
        {
            playerData.slows += 1;
            FloatingTextController.CreateFloatingText("O F F B E A T", player.transform.position);
            displayScoreChange_Increase(125, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

            //Enable step3 of tutorial after the first clear
            TutorialManager.tm.step3 = true;
            TutorialManager.tm.ChangeMessage();



        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase > AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 == 0 && !gotResult) //Fast but on beat
        {
            playerData.fasts += 1;
            FloatingTextController.CreateFloatingText("G R E A T", player.transform.position);
            displayScoreChange_Increase(500, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

            
            //Enable step3 of tutorial after the first clear
            TutorialManager.tm.step3 = true;
            TutorialManager.tm.incrementClears();
            TutorialManager.tm.ChangeMessage();

        }
        else if (erm.gpOrder.Count == 0 && AudioPeer.ap.beatsInPhase > AudioPeer.ap.beatsInRecallPhase && AudioPeer.ap.beats % 4 != 0 && !gotResult) //Early off beat
        {
            playerData.fasts += 1;
            FloatingTextController.CreateFloatingText("E A R L Y", player.transform.position);
            displayScoreChange_Increase(500, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            gotResult = true;

            //Enable step3 of tutorial after the first clear
            TutorialManager.tm.step3 = true;
            TutorialManager.tm.ChangeMessage();

        }

        //Step2, show player fail
        if (TutorialManager.tm.step5)
        {
            if (AudioPeer.ap.beatsInRecallPhase > AudioPeer.ap.beatsInPhase + beatsTillFail && !gotResult) //No input given, fail, disabled during tutorial song
            {
                playerData.fails += 1;
                if (!occupied)
                {
                    StartCoroutine(clearScreen());
                }

                gotResult = true;
            }
        }
        
    }

    private void handleInput()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            

            Ray ray;
            //Vector3 pos_at_z_0;
            //float z_plane_of_2d_game = 0;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //pos_at_z_0 = ray.origin + ray.direction * (z_plane_of_2d_game - ray.origin.z) / ray.direction.z;

            RaycastHit raycastHit;

            //Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 5, false);



            if (Physics.Raycast(ray, out raycastHit)) //if statement to collect tap/click info
            {
                GameObject o = raycastHit.collider.gameObject;

                if (o.tag == "Enemy" || o.tag == "Enemy_Cluster" || o.tag == "Player") //player tapped an enemy, handle input if correct/incorrect pattern
                {
                    
                    

                    //print(AudioPeer.ap.beatsInPhase);
                    if (o.tag == "Player")
                    {
                        if (erm.gpOrder.Count > 0 && o.transform.position == erm.gpOrder[0]) //correct
                        {
                            playerData.correctHits += 1;
                            displayScoreChange_Increase(100, new Vector2(o.transform.position.x, o.transform.position.y -2));
                            Inc_Combo();
                            

                            erm.correct_popTopOffOrder();
                            unloadShots();
                            clickTimes.Add(clickTime);
                            clickTime = 0;
                            numClicks++;
                   

                        }
                        else if (erm.gpOrder.Count > 0 && o.transform.position != erm.gpOrder[0]) //Player tapped center, incorrect
                        {
                
                            makeEnemyShoot();
                            clickTime = 0;
                        }
                    }
                    else if(o.tag == "Enemy_Cluster")
                    {
                       
                        if (erm.gpOrder.Count > 0 && o.transform.position == erm.gpOrder[0]) //correct tap
                        {
                            playerData.correctHits += 1;
                            displayScoreChange_Increase(100, o.transform.position);
                            Inc_Combo();

                            erm.correct_popTopOffOrder();
                            player.drawTarget(o.transform);
                            addTarget(o);

                            clickTimes.Add(clickTime);
                            clickTime = 0;
                            numClicks++;

                            // player.takeDelayedShots(o.transform);

                            //step2 tutorial
                            if (AudioPeer.ap.tutorial_Playing && erm.gpOrder[0] == player.transform.position)
                            {
                                TutorialManager.tm.step2 = true;
                                TutorialManager.tm.ChangeMessage();
                            }

                        }
                        else if (erm.gpOrder.Count > 0 && o.transform.position != erm.gpOrder[0] && erm.gpOrder.Contains(o.transform.position)) //Tapped enemy is not the current order, and position still exists in the order (Prevent retapping correct enemies)
                        {
                            //o.GetComponent<Enemy>().clusterShoot();
                            makeEnemyShoot();
                            clickTime = 0;

                        }
                    }
                    else if(o.tag == "Enemy")
                    {
                        if (erm.gpOrder.Count > 0 && o.transform.position == erm.gpOrder[0]) //correct
                        {
                            playerData.correctHits += 1;
                            displayScoreChange_Increase(100, o.transform.position);
                            Inc_Combo();

                            erm.correct_popTopOffOrder();
                            player.drawTarget(o.transform);
                            addTarget(o);

                            clickTimes.Add(clickTime);
                            clickTime = 0;
                            numClicks++;
                            //player.shoot(o.transform);

                            //step2 tutorial
                            if (AudioPeer.ap.tutorial_Playing && erm.gpOrder[0] == player.transform.position)
                            {
                                TutorialManager.tm.step2 = true;
                                TutorialManager.tm.ChangeMessage();
                            }

                        }
                        else if (erm.gpOrder.Count > 0 && o.transform.position != erm.gpOrder[0] && erm.gpOrder.Contains(o.transform.position)) //incorrect
                        {
                            // o.GetComponent<Enemy>().shoot(o);
                            makeEnemyShoot();
                            clickTime = 0;
                        }
                    }
                    
                }
                


            }
        }
        
    }

    public void addTarget(GameObject o)
    {
        targets.Add(o);
    }

    public void unloadShots()
    {
        player.shootTargets(targets);
        targets.Clear();
    }

 

    public void makeEnemyShoot()
    {

        AudioPeer.ap.Distort(0.1f);
        if (erm.enemyOrder[0].gameObject.tag == "Enemy" && erm.enemyOrder[0].transform.position == erm.gpOrder[0])
        {
            erm.enemyOrder[0].shoot(erm.enemyOrder[0].gameObject);
            displayScoreChange_Decrease(25, erm.enemyOrder[0].transform.position);
        }
        else if (erm.enemyOrder[0].gameObject.tag == "Enemy_Cluster" && erm.enemyOrder[0].transform.position == erm.gpOrder[0])
        {
            erm.enemyOrder[0].clusterShoot();
            displayScoreChange_Decrease(25, erm.enemyOrder[0].transform.position);
        }
        //Player/shoot object is up next, missfire!
        else
        {
            player.shootMisfire();
            displayScoreChange_Decrease(25, player.transform.position);
        }

        playerData.misses += 1;
        Break_Combo();

        


    }

    //After failing, enemies fire then retreat to starting position;
    IEnumerator clearScreen()
    {
        occupied = true;
        yield return new WaitForSeconds(0f);
        AudioPeer.ap.Distort(0.6f);

        if(!AudioPeer.ap.nearingEndOfSong())
        {
            displayScoreChange_Decrease(500, new Vector2(player.transform.position.x, player.transform.position.y + 1f));
            FloatingTextController.CreateFloatingText("F A I L", player.transform.position);
            erm.boardWipe();
            cleanPhase();
        }
        else
        {
            erm.endGameplayPhase();
            cleanPhase();
        }
        

        occupied = false;
    }

    //Extra clean up to make sure phase is clear of previous phase
    public void cleanPhase()
    {
        player.clearTargetVisuals();
        targets.Clear();
    }

    //Wait seconds before starting gameplay phase
    public IEnumerator getReady(float waitTime)
    {
        occupied = true;
        reset = false;
        AudioPeer.ap.startPlayingMusic(waitTime); //Begins music waitTime seconds prior to gameplay phase
        SlidingTextController.CreateSlidingText(AudioPeer.ap.album[AudioPeer.ap.selectClip_index].title, AudioPeer.ap.album[AudioPeer.ap.selectClip_index].artist, GameObject.Find("Player").transform.position);
        yield return new WaitForSeconds(waitTime);
        gm.currentState = GameManager.GameState.waiting;
        occupied = false;
    }

    //Getter function for score, score should not be edited outsite gamemanager
    public int GetScore()
    {
        return playerData.score;
    }//Getter function for combo, combo should not be edited outsite gamemanager

    public int GetCombo()
    {
        return playerData.combo;
    }

    public void displayScoreChange_Increase(int amount, Vector3 position)
    {
        playerData.score += amount;
        FloatingTextController.CreateFloatingText("+" + amount.ToString(), position);
    }

    public void displayScoreChange_Decrease(int amount, Vector3 position)
    {
        playerData.score -= amount;
        FloatingTextController.CreateFloatingText("-" + amount.ToString(), position);
    }

    //Increment combo
    public void Inc_Combo()
    {
        //increment combo
        playerData.combo += 1;
        //shake text;
        GameplayUI.gpUI.shakeIt();
        //check max combo
        if(playerData.combo > playerData.maxCombo)
        {
            playerData.maxCombo = playerData.combo;
        }

    }

    //Combo dropped, display drop
    public void Break_Combo()
    {
        if (playerData.combo > 0)
        {
            FloatingTextController.CreateFloatingText("D R O P", player.transform.position);
            //Check if max combo
            if(playerData.combo > playerData.maxCombo)
            {
                playerData.maxCombo = playerData.combo;
            }
            //reset combo
            playerData.combo = 0;
            //shake text;
            GameplayUI.gpUI.shakeIt();
        }
        

    }

    //Called at the end of memoryRelay in gameManager, max number possible the player can get for a perfect score
    public void countPerfectPattern(int number)
    {
        playerData.perfectRun += number;
    }

    //returns a percentage of how well the player performed
    public float getPlayerPerformance()
    {
        return grade;
    }

    public IEnumerator changeToEndingState()
    {
        occupied = true;

        yield return new WaitForSeconds(1f);

        currentState = GameState.endingPhase;

        occupied = false;

         
    }
}

public class SaveData
{
    //Max combo per song, each index corresponds to song album[] in AudioPeer
    public List<int> easy_HighScores;
    //Max combo per song, each index correspodns to song album[] in AudioPeer
    public List<int> easy_MaxCombos;

    public List<int> normal_HighScores;
    public List<int> normal_MaxCombos;

    public List<int> hard_HighScores;
    public List<int> hard_MaxCombos;




    //If no save file present
    public void init()
    {
        easy_HighScores = new List<int>();
        easy_MaxCombos = new List<int>();

        normal_HighScores = new List<int>();
        normal_MaxCombos = new List<int>();

        hard_HighScores = new List<int>();
        hard_MaxCombos = new List<int>();


        foreach (Song s in AudioPeer.ap.album)
        {
            //set all scores to 0
            easy_HighScores.Add(0);
            easy_MaxCombos.Add(0);

            normal_HighScores.Add(0);
            normal_MaxCombos.Add(0);

            hard_HighScores.Add(0);
            hard_MaxCombos.Add(0);

        }
    }

    public void Save_Json()
    {
        string json = JsonUtility.ToJson(this);
        Debug.Log(json);
        PlayerPrefs.SetString("PlayerSaveData", json);
    }


}

public class ScoreData
{
    /// <summary>
    /// SCORE KEEPING
    /// </summary>
    public int score = 0; //player score
    public int combo = 0; //player combo
    public int maxCombo = 0; //player max combo for song
    /// <summary>
    /// Results for ResultsScreen, perfectRun, misses, correctHits are floats, may use (int) casting instead
    /// </summary>
    public float perfectRun = 0; //best possible score
    public float misses = 0; //number of times player tapped incorrectly;
    public float correctHits = 0; //number of times player tapped correctly;
    public int slows = 0;
    public int fasts = 0;
    public int perfects = 0;
    public int fails = 0;

    //Reset all player data at the end of a song
    public void reset()
    {
        score = combo = maxCombo = slows = fasts = perfects = fails = 0;
        perfectRun = misses = correctHits = 0;
    }

    //Save data for current song
    public void save_SaveData()
    {
        //Easy scores
        GameManager.playerSave.easy_HighScores[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_HighScore;
        GameManager.playerSave.easy_MaxCombos[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].easy_MaxCombo;

        //Normal scores
        GameManager.playerSave.normal_HighScores[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_HighScore;
        GameManager.playerSave.normal_MaxCombos[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].normal_MaxCombo;

        //Hard scores
        GameManager.playerSave.hard_HighScores[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_HighScore;
        GameManager.playerSave.hard_MaxCombos[AudioPeer.ap.selectClip_index] = AudioPeer.ap.album[AudioPeer.ap.selectClip_index].hard_MaxCombo;
    }

    public void Load_Json()
    {

        string p = PlayerPrefs.GetString("PlayerSaveData");
        if (p != null && p.Length > 0)
        {
            GameManager.playerSave = JsonUtility.FromJson<SaveData>(p);

        }

        foreach (Song s in AudioPeer.ap.album)
        {
            //Easy scores
            s.easy_HighScore = GameManager.playerSave.easy_HighScores[AudioPeer.ap.album.IndexOf(s)];
            s.easy_MaxCombo = GameManager.playerSave.easy_MaxCombos[AudioPeer.ap.album.IndexOf(s)];

            //Normal
            s.normal_HighScore = GameManager.playerSave.normal_HighScores[AudioPeer.ap.album.IndexOf(s)];
            s.normal_MaxCombo = GameManager.playerSave.normal_MaxCombos[AudioPeer.ap.album.IndexOf(s)];

            //Hard
            s.hard_HighScore = GameManager.playerSave.hard_HighScores[AudioPeer.ap.album.IndexOf(s)];
            s.hard_MaxCombo = GameManager.playerSave.hard_MaxCombos[AudioPeer.ap.album.IndexOf(s)];
        }


    }
}
