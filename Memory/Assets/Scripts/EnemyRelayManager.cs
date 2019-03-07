using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRelayManager : MonoBehaviour {

    private GameManager gm; //private reference to gamemanager gm;

    public static EnemyRelayManager erm; //static variable for single instance of enemyrelaymanager, for use in GM

    /// <summary>
    /// Enemy and placement lists
    /// </summary>
    //Elements set in inspector//
    [SerializeField]
    private Transform[] nodes; //Enemy location nodes//Assigned in inspector
    [SerializeField]
    private GameObject[] enemies; //enemy objects//Assigned in inspector
    //Elements are initialized from 
    [SerializeField]
    private Transform[] relayedEnemyPositions; //Nodes assigned to specific enemy element in enemies[]
    [SerializeField]
    public List<Vector3> gpOrder; //Final list containing all memory relay positions including enemies, and shoot/tap instances. Use this array to check input during recallPhase
    [SerializeField]
    public List<Enemy> enemyOrder; //List order for enemies only
    [SerializeField]
    public List<float> arrivalTimes;
    //////////////////////////

    //Difficulty_Setting//
    [SerializeField]
    Difficulty_Setting easy_Setting; //set in inspector
    [SerializeField]
    Difficulty_Setting normal_Setting; //set in inspector
    [SerializeField]
    Difficulty_Setting hard_Setting; //set in inspector

    public Vector3 playerPos; //Players position

    float ElapsedTime, FinishTime, TimeArrival = 0;

    public int currentEnemyRelay; //Which enemy to move next;

    public int gpCurrentRelay; //Global playlist relay order, includes player position

    public bool rest; //For shoot part of pattern

    public int beatsAtMoment = 0; //current beats at momment of action, used to prevent next phase from occurring on same exact beat;

    /// <summary>
    /// These bools are to make sure events only happen once when a beat is reached
    /// </summary>
   // public bool sameBeat; //Beat is on the same beat
    ///public bool changed; //Beat has changed
   // public int prevBeat; //Prev beat before beats changed
    public bool chain = false; //Chain of enemies before player pattern
    public bool occupied = false; //For update coroutine calls

    //Begin counting the beats in the currentPhase
    public bool counting = false;

    //Current status of sameBeat in AudioPeer; Follows current beats
    public bool currentBeatStatus = false;

    //board is clear and ready to reset positions, needed to avoid enemies popping into their new position
    public bool clear = false;

    //Retreat called
    public bool retreat = false;

    //Phase timing boolean, beat started on relay phase, used because relay phase does not start exactly when memoryRelay state, waits for next 4th beat before pattern is given
    public bool relayPhaseReady = false;

    //Inactive enemies for reset
    public bool endingGameplayPhase = false;


    


    ////////TESTING///////////
    //GIZMO DRAWING FOR DEBUGGING//
    public float drawDis = 1f; //Size of path objects, used for debugging

    private void Awake()
    {
        if (erm != null)
        {
            Debug.LogError("more than one enemyrelaymanager in scene");
        }
        else
        {
            erm = this;
        }

        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position; //Assign player position


        if (GameManager.gm.difficulty == GameManager.Difficulty.easy)
        {
            AssignDifficultySetting(easy_Setting);
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
        {
            AssignDifficultySetting(normal_Setting);
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
        {
            AssignDifficultySetting(hard_Setting);
        }

        arrivalTimes = new List<float>();

    }

    // Use this for initialization
    void Start () {
        //assign main game manager reference
        if (GameManager.gm != null)
        {
            gm = GameManager.gm;
        }
        else
        {
            Debug.LogError("gm not instantiated");
        }


        relayedEnemyPositions = new Transform[nodes.Length];
        //Default, corresponding nodes = enemy positions 1:1
        for (int x = 0; x < nodes.Length; x++)
        {
            relayedEnemyPositions[x] = nodes[x];
        }
        //Default, corresponding nodes = enemy positions 1:1
        foreach (GameObject e in enemies)
        {
            enemyOrder.Add(e.GetComponent<Enemy>());
        }
        //Populates gpOrder
        createGameplayOrder(relayedEnemyPositions);
        //Give enemy objects their target positions
        AssignPosition(relayedEnemyPositions);
    }

    //Re-initialize arrays when difficulty changes
    public void ChangeDifficulty()
    {
        if (GameManager.gm.difficulty == GameManager.Difficulty.easy)
        {
            AssignDifficultySetting(easy_Setting);
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
        {
            AssignDifficultySetting(normal_Setting);
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
        {
            AssignDifficultySetting(hard_Setting);
        }

        //Create new enemy positions based of difficulty
        relayedEnemyPositions = new Transform[nodes.Length];
        //Default, corresponding nodes = enemy positions 1:1
        for (int x = 0; x < nodes.Length; x++)
        {
            relayedEnemyPositions[x] = nodes[x];
        }
        //Re-allocate order arrays;
        endGame_Reset();
    }

    //Gets a difficulty setting of nodes,enemies from Difficulty_Setting, assigns them to nodes and enemies arrays for EnemyRelayManager class
    public void AssignDifficultySetting(Difficulty_Setting setting)
    {
        nodes = new Transform[setting.node_Path.Length];
        enemies = new GameObject[setting.node_Path.Length];

        for (int x = 0; x < nodes.Length; x++)
        {
            nodes[x] = setting.node_Path[x].transform;
            enemies[x] = setting.enemies[x];
        }
    }

    public void endGame_Reset()
    {
        //Reset endgameplayphase flag
        endingGameplayPhase = false;
        //Reset time based variables
        ElapsedTime = FinishTime = TimeArrival = 0;
        //Reset enemy and gp array indexes
        currentEnemyRelay = gpCurrentRelay = 0;

        //Clear enemy and gp array orders
        enemyOrder.Clear();
        gpOrder.Clear();

        arrivalTimes.Clear();

        //Reset appropriate variables
        rest = false; //not resting for start of gameplay phase
        beatsAtMoment = 0; //no beats at momemnt assigned
        chain = false; //chain has not started
        counting = false; //counting for beats in phase has not begun
        currentBeatStatus = false; //Beat status at start of game is false
        retreat = false; //enemy retreat flag set to false by default
        relayPhaseReady = false; //relay phase set to false by default at beginning of gameplay phase

        Shuffle(relayedEnemyPositions);
    }

    public bool AllEnemiesInactive()
    {
        bool allInactive = true;

        for(int x = 0; x < enemies.Length; x++)
        {
            if(enemies[x].activeInHierarchy)
            {
                allInactive = false;
                break;
            }
        }

        return allInactive;
    }



    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;
        TimeArrival += Time.deltaTime;
        memoryGameLoop();
    }

    void memoryGameLoop()
    {
        if (gm.currentState == GameManager.GameState.memoryRelay)
        {
            if(AudioPeer.ap.beats % 4 == 0 && !counting) //Begin counting AudioPeer's beatsInPhase after the first 4th beat of the starting phase of memoryRelay
            {
                counting = true;
                relayPhaseReady = true;

            }
            
            //Start of the memoryRelay phase, pattern given out, don't give out pattern 
            //Move each enemy in enemies[] into relayed position while the end of the memoryRelay phase has not yet ended, and rest/waiting period not active
            if (!rest && currentEnemyRelay < enemies.Length && gpCurrentRelay < gpOrder.Count && (AudioPeer.ap.beats - beatsAtMoment) >= 4)
            {
                
                //Enable first enemy in list; 
                if (AudioPeer.ap.beatsInPhase == 4 && currentEnemyRelay == 0)
                {
                    currentBeatStatus = AudioPeer.ap.sameBeat;
                    enemies[currentEnemyRelay].SetActive(true);
           
                    chain = true; //begin chain

                    //print("something happens at " + AudioPeer.ap.beatsInPhase);
               
                }
                //First enemy object has been enabled, beat is 4, currentEnemyRelay has not reached its position yet, but the previous currentEnemyRelay has reached its position and the gpCurrentRelay is not a player, therefore activate the currentenemyRelay
               if(currentEnemyRelay > 0  && AudioPeer.ap.beatsInPhase % 4 == 0 && !enemies[currentEnemyRelay].GetComponent<Enemy>().InPosition() && enemies[currentEnemyRelay - 1].GetComponent<Enemy>().InPosition() && gpOrder[gpCurrentRelay] != playerPos)
                {
                    //("something happens at " + AudioPeer.ap.beatsInPhase);
                    currentBeatStatus = AudioPeer.ap.sameBeat;
                    enemies[currentEnemyRelay].SetActive(true);
                   
                    chain = true; //begin chain

  
                }

               //Chain the rest of the enemies, enable them without next beat
               if(chain)
               {
                    enemies[currentEnemyRelay].SetActive(true);
               }

                //current enemy has reached its selected position, and not all enemies have been assigned/reached their relayed positions
                if (enemies[currentEnemyRelay].GetComponent<Enemy>().InPosition() && currentEnemyRelay < relayedEnemyPositions.Length) //Enemy in position
                {
                    //print(TimeArrival);
                    arrivalTimes.Add(TimeArrival);
                    currentEnemyRelay++; //move to next enemy in the enemies[]
                    gpCurrentRelay++;
                    TimeArrival = 0f;
                   

                }
              

            }

            //current order in the list is the player/shoot position.  Pause before moving to next set of enemies
            if (gpCurrentRelay < gpOrder.Count && gpOrder[gpCurrentRelay] == playerPos)
            {
                //Pause for animation, then gpCurrentRelay++
                beatPause();
                

            }

            //last enemies has reached its position, return all enemies to starting position first in first out, change state to memoryRelay
            if (gpCurrentRelay == gpOrder.Count && enemies[currentEnemyRelay - 1].GetComponent<Enemy>().InPosition())
            {

                GameManager.gm.countPerfectPattern(gpOrder.Count); //Count number of patterns assigned
                gm.currentState = GameManager.GameState.memoryRecall;
                relayPhaseReady = false;
                counting = false;

            }
        }
        //Player input phase, memoryRecall phase, input in GameManager
        else if (gm.currentState == GameManager.GameState.memoryRecall)
        {
            bool active = false;
            //All enemies destroyed/inactive
            if (gpOrder.Count == 0 && !retreat)
            {
                foreach (GameObject e in enemies)
                {
                    if (e.gameObject.activeInHierarchy)
                    {
                        active = true;
                        return;
                    }
                }

                if (!active && !occupied)
                {
                    StartCoroutine(resetPositions());
                }

            }

            //Force enemies on screen back to starting position
            if(retreat)
            {
                foreach (GameObject e in enemies)
                {
                    if (e.gameObject.activeInHierarchy)
                    {
                        active = true;
                        return;
                    }
                }

                if (!active && !occupied)
                {
                    StartCoroutine(resetPositions());
                }
            }


        }
       

        //End of memoryRecall phase, return enemies to starting position
        else if (gm.currentState == GameManager.GameState.waiting)
        {
            //(int)AudioPeer._audioSource.time % 4 == 0
            //Make sure the beat has changed since inPhaseBeatStatus was taken
            if (AudioPeer.ap.sameBeat != currentBeatStatus && !AudioPeer.ap.songEnded())
            {
                
                //Don't give new pattern if song is almost done
                if(!AudioPeer.ap.nearingEndOfSong())
                {
                    gm.currentState = GameManager.GameState.memoryRelay; //give player new pattern
                }
               
                counting = false;

            }
        }

        //Gameplay phase ending
        if(endingGameplayPhase)
        {
            if(AllEnemiesInactive())
            {
                endGame_Reset();
            }
            
        }

       
    }


    //When gpCurrentRelay has reached a player position int he pattern, pause and activate enemybeatvisualizer for chain of enemies, pause, then increment to next gpCurrentRelay
    void beatPause()
    {
        chain = false;
        
        if (AudioPeer.ap.beatsInPhase % 4 == 0 && currentBeatStatus != AudioPeer.ap.sameBeat)
        {
            foreach (GameObject t in enemies)
            {
                foreach (Transform p in relayedEnemyPositions)
                {
                    if (t.transform.position == p.position)
                    {
                        //print("OK");

                        if (t.tag == "Enemy_Cluster")
                        {
                            foreach (Transform child in t.transform)
                            {
                                //child.GetComponent<Renderer>().material.color = Color.red;
                                child.GetComponent<EnemyBeatVisualizer>().enabled = true;

                            }
                        }
                        else
                        {
                            t.GetComponent<EnemyBeatVisualizer>().enabled = true;
                        }

                    }
                }
            }
        }
        
        //if the beat has changed, wait and move to next chain of enemies;
        if(!occupied && currentBeatStatus != AudioPeer.ap.sameBeat)
        {
            StartCoroutine(pause());
        }
        
    }

    //Pause before incrementing gpCurrentRelay and activating next chain of enemies
    IEnumerator pause()
    {
        occupied = true;
        
        //Wait for next beat to avoid spawning enemies during the same beat;
        yield return new WaitForSeconds(0f);
        
        gpCurrentRelay++;
        rest = false;
        occupied = false;
        
    }
    //Remove object from theOrder array when player correctly selects an object
    public void correct_popTopOffOrder()
    {
        //If enemyOrder still has undestroyed enemies, and the current gpOrder is that same enemy tapped
        if (enemyOrder.Count > 0 && enemyOrder[0].transform.position == gpOrder[0])
        {
            enemyOrder.RemoveAt(0);
        }
        gpOrder.RemoveAt(0);
    }

    //Clear everything for new Pattern
    public void boardWipe()
    {

        foreach(GameObject o in enemies)
        {
            if(o.activeInHierarchy)
            {
                o.GetComponent<Enemy>().fallBack();
            }   
        }

        retreat = true;

    }

    //Song ended, end gameplay phase
    public void endGameplayPhase()
    {
        foreach (GameObject o in enemies)
        {
            if (o.activeInHierarchy)
            {
                o.GetComponent<Enemy>().GameEnded();  //tell all enemies to retreat
            }
        }
        
        //Toggle endgame flag
        endingGameplayPhase = true;
    }

   
    //memoryRelay phase is done, return enemies to starting position and prepare for pattern change
    public IEnumerator resetPositions()
    {
        occupied = true;
        
        yield return new WaitForSeconds(0f);

        gm.currentState = GameManager.GameState.waiting;

        //Checking Safety Measures//
        currentBeatStatus = AudioPeer.ap.sameBeat;
        beatsAtMoment = AudioPeer.ap.beats;
        ////////////////////////////

        //clear orders make sure they are empty
        enemyOrder.Clear();
        gpOrder.Clear();

        currentEnemyRelay = 0;
        gpCurrentRelay = 0;
        Shuffle(relayedEnemyPositions);
        arrivalTimes.Clear();


        chain = false;
        clear = false;
        retreat = false;

        occupied = false;
    }

    

    //shuffle pattern
    void Shuffle(Transform[] array)
    {
        int l = array.Length;
        for (int x = l - 1; x > 0; x--)
        {
            int r = Random.Range(0, x);
            Transform o = array[r];
            array[r] = array[x];
            array[x] = o;

        }
        //Populates gpOrder
        createGameplayOrder(array);
        //Give enemy objects their target positions
        AssignPosition(relayedEnemyPositions);
        //Readd enemies to enemyOrder
        foreach (GameObject e in enemies)
        {
            enemyOrder.Add(e.GetComponent<Enemy>());
        }
    }

    //Assign individual enemy objects their targetPos in enemy script;
    public void AssignPosition(Transform[] relayedPositions)
    {
        for(int x = 0; x < relayedPositions.Length; x++)
        {
            enemies[x].GetComponent<Enemy>().AssignPosition(relayedPositions[x]);
        }
    }


    public void createGameplayOrder(Transform[] targetPositions)
    {
        List<Vector3> copyOrder = new List<Vector3>();
        foreach(Transform pos in targetPositions)
        {
            copyOrder.Add(pos.position);
        }
        //Flush gpOrder to make a new list.
        gpOrder.Clear();
        //# of times shoot is put into the order, +1 if no shoot at the end of list
        //int times = Random.Range(1, 3);
        //Set number of chains based on difficulty setting
        int times = 0;
        if (GameManager.gm.difficulty == GameManager.Difficulty.easy)
        {
            times = 0;
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
        {
            times = 1;
        }
        else if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
        {
            times = 2;
        }

        List<Vector3> list = new List<Vector3>();
        foreach (Vector3 pos in copyOrder)
        {
            list.Add(pos);
        }

        int listSize = list.Count;

        List<int> occurs = new List<int>();

        //Generate a list of unique elements corresponding to the element for the "shoot" to occur in the gpOrder list
        for (int i = 0; i < times; i++)
        {
            int tempLoc = Random.Range(0, list.Count);
            while (occurs.Contains(tempLoc))
            {
                tempLoc = Random.Range(0, list.Count);
            }
            occurs.Add(tempLoc);

        }
        occurs.Sort();

        //fill in the gpOrder list array by combinding theOrder[] and occurs[] lists
        for (int j = 0; j < listSize; j++)
        {
            gpOrder.Add(list[0]);
            list.RemoveAt(0);
            if (occurs.Count > 0 && occurs[0] == j)
            {
                gpOrder.Add(playerPos); //add the center tap/shoot
                occurs.RemoveAt(0);
            }
        }
        //Last tap should always be a shoot;
        if (gpOrder[gpOrder.Count - 1] != playerPos)
        {
            gpOrder.Add(playerPos);
        }



    }
}
