using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRelayManager2 : MonoBehaviour
{
    private GameManager gm; //private reference to gamemanager gm;
    public static EnemyRelayManager2 erm; //static variable for single instance of enemyrelaymanager, for use in GM

    public Transform[] nodes; //Enemy path

    [SerializeField]
    //private int node; //Selected node to place enemy

    public GameObject[] enemies; //Enemy objects'


    [SerializeField]
    private Transform[] relayedPositions; //Nodes assigned to specific enemy element in enemies[];
    public List<Vector3> theOrder; //Ordered in first in first out, patterned positions
    public List<Enemy> enemyOrder;

    [SerializeField]
    public List<Vector3> gpOrder; //Final list containing all memory relay positions including enemies, and shoot/tap instances. Use this array to check input during recallPhase

    public Vector3 enemyStartPos; //Starting pos of enemies
    public Vector3 playerPos;

    public float ElapsedTime, FinishTime; //Start and End time for moving object, in seconds

    //float[] speedTimes = new float[] { 0.1f, 0.25f, 0.5f, 0.75f, 1f};
    float[] speedTimes = new float[] {0.5f};

    //public bool returnToStart; //Flag to return enemies to start pos;

    //public bool memoryPhase;

    //public bool moveIn;

    public bool occupied; //Coroutine check

    public int currentRelay; //Which enemy to move next

    public int gpCurrentRelay;
    public bool rest; //For shoot part of pattern
    //public bool ready;


    //TESTING//
    float circleSize = 5f;

    //GIZMO DRAWING FOR DEBUGGING//
    public float drawDis = 1f; //Size of path objects, used for debugging

    public GameObject explosion;


    // Use this for initialization
    void Awake()
    {

        if (erm != null)
        {
            Debug.LogError("more than one enemyrelaymanager in scene");
        }
        else
        {
            erm = this;
        }

        enemyStartPos = enemies[0].transform.position;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        ElapsedTime = 0f;
        //FinishTime = 1f;
        FinishTime = speedTimes[Random.Range(0, speedTimes.Length)];

        relayedPositions = new Transform[nodes.Length];

        for(int x = 0; x < relayedPositions.Length; x++)
        {
            relayedPositions[x] = nodes[x];
        }

        foreach(Transform o in relayedPositions)
        {
            theOrder.Add(o.position);
        }

        foreach(GameObject e in enemies)
        {
            enemyOrder.Add(e.GetComponent<Enemy>());
        }

        currentRelay = 0;
        gpCurrentRelay = 0;
        //ready = false;
        createGameplayOrder(theOrder);


        //node = (int)Random.Range(0, nodes.Length);

    }

    void Start()
    {
        //assign main game manager reference
        if (GameManager.gm != null)
        {
            gm = GameManager.gm;
        }
        else
        {
            Debug.LogError("gm not instantiated");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        ElapsedTime += Time.deltaTime;

        if (gm.currentState == GameManager.GameState.memoryRelay)
        {

            //currentRelay[0] called at start
            

            
            /*
            float distance = Vector3.Distance(enemies[currentRelay].transform.position, relayedPositions[currentRelay].position);

            print(distance);

            if (distance > 4)
            {
                enemies[currentRelay].transform.position = new Vector3(Mathf.Sin((ElapsedTime / FinishTime) * circleSpeed) * circleSize, Mathf.Cos((ElapsedTime / FinishTime) * circleSpeed) * circleSize);
                circleSize += circleGrowSpeed;
            }
            */
           

            //Move each enemy in enemies[] into relayed position
            if(!rest && currentRelay < enemies.Length && gpCurrentRelay < gpOrder.Count)
            {
                if (enemies[currentRelay].tag == "Enemy")
                {
                    FinishTime = 0.25f;
                }
                else
                {
                    FinishTime = 0.5f;
                }

                enemies[currentRelay].transform.position = Vector3.Lerp(enemyStartPos, relayedPositions[currentRelay].position, ElapsedTime / FinishTime);
            }
            
            if(currentRelay < enemies.Length && gpCurrentRelay < gpOrder.Count)
            {
                if (enemies[currentRelay].transform.position == relayedPositions[currentRelay].position && currentRelay < nodes.Length) //Enemy in position
                {
                    //print(ElapsedTime + " arrived");

                    currentRelay++; //move to next enemy in the enemies[]
                    gpCurrentRelay++;
                    ElapsedTime = 0;
                    //FinishTime = speedTimes[Random.Range(0, speedTimes.Length)]; //Randomize speed for next enemy//FinishTime = AudioPeer._AmplitudeBuffer
                   
                }

                if (gpOrder[gpCurrentRelay] == playerPos)
                {
                    //Pause for animation, then gpCurrentRelay++
                    if (!occupied)
                    {
                        StartCoroutine(enemyShootPause());
                    }
                   
                }

               
            }

            //last enemies has reached its position, return all enemies to starting position first in first out, change state to memoryRelay
            if (gpCurrentRelay == gpOrder.Count && enemies[currentRelay-1].transform.position == relayedPositions[currentRelay-1].position)
            {
                gm.currentState = GameManager.GameState.memoryRecall;

            }
         


        }

        //Player input phase, memoryRelay phase, input in GameManager
        else if (gm.currentState == GameManager.GameState.memoryRecall)
        {
            if (gpOrder.Count == 0 && !enemies[theOrder.ToArray().Length].activeInHierarchy)
            {
                if (!occupied)
                {
                    StartCoroutine(resetPositions());
                }
            }
        }

        //End of memoryRelay phase, return enemies to starting position
        else if (gm.currentState == GameManager.GameState.waiting)
        {
            enemies[currentRelay].transform.position = Vector3.Lerp(relayedPositions[currentRelay].position, enemyStartPos, ElapsedTime / FinishTime);
            // enemies[currentRelay].transform.position = new Vector3(Mathf.Sin((Time.time) * circleSpeed) * circleSize, Mathf.Cos((Time.time) * circleSpeed) * circleSize);
            //circleSize -= circleGrowSpeed;

            if (enemies[currentRelay].transform.position == enemyStartPos && currentRelay < nodes.Length - 1)
            {
                currentRelay++;
                ElapsedTime = 0;
                circleSize = 5f;
            }

            //last enemies has reached its starting position, return all enemies to starting position first in first out animation;
            if (currentRelay == nodes.Length - 1 && enemies[currentRelay].transform.position == enemyStartPos)
            {

                foreach (GameObject o in enemies)
                {
                    if(o.tag == "Enemy_Cluster")
                    {
                        o.SetActive(true);
                        foreach(Transform child in o.transform)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        o.SetActive(true);
                    }
                    
                }

                currentRelay = 0;
                gpCurrentRelay = 0;
                circleSize = 5f;
                ElapsedTime = 0;
                Shuffle(relayedPositions);

                gm.currentState = GameManager.GameState.memoryRelay; //give player new pattern
            }
        }



    }

    //Remove object from theOrder array when player correctly selects an object
    public void correct_popTopOffOrder()
    {
        if(enemyOrder.Count > 0)
        {
            if (enemyOrder[0].transform.position == gpOrder[0])
            {
                enemyOrder.RemoveAt(0);
                theOrder.RemoveAt(0);
            }
        }
        
        gpOrder.RemoveAt(0);
        //print("GOT IT!");


        /*
        GameObject explosion = ObjectPooler.current.getPooledObject(Resources.Load("explosion") as GameObject);
        explosion.transform.position = o.transform.position;
        explosion.transform.rotation = o.transform.rotation;
        explosion.SetActive(true);
        */


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

        foreach (Transform o in relayedPositions)
        {
            theOrder.Add(o.position);
          
        }

        foreach (GameObject e in enemies)
        {
            enemyOrder.Add(e.GetComponent<Enemy>());
        }

        createGameplayOrder(theOrder);
    }

    //memoryRelay phase is done, return enemies to starting position and prepare for pattern change
    public IEnumerator resetPositions()
    {
        occupied = true;

        yield return new WaitForSeconds(1f);


 

        gm.currentState = GameManager.GameState.waiting;

        // returnToStart = true;
        //memoryPhase = false;
        currentRelay = 0;
        gpCurrentRelay = 0;
        ElapsedTime = 0;
        FinishTime = 0.25f;

        occupied = false;
    }

    public IEnumerator enemyShootPause()
    {
        occupied = true;
        rest = true;
        

        print("PAUSE!!!!!");
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject t in enemies)
        {
            foreach (Transform p in relayedPositions)
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

        gpCurrentRelay++;
        ElapsedTime = 0;
        
        rest = false;
        occupied = false;


    }

    public void createGameplayOrder(List<Vector3> copyOrder)
    {
        //Flush gpOrder to make a new list.
        gpOrder.Clear();
        int times = Random.Range(1, 3);
        List<Vector3> list = new List<Vector3>();
        foreach(Vector3 pos in copyOrder)
        {
            list.Add(pos);
        }

        int listSize = list.Count;

        List<int> occurs = new List<int>();

        //Generate a list of unique elements corresponding to the element for the "shoot" to occur in the gpOrder list
        for(int i = 0; i < times; i++)
        {
            int tempLoc = Random.Range(0, list.Count);
            while(occurs.Contains(tempLoc))
            {
                tempLoc = Random.Range(0, list.Count);
            }
            occurs.Add(tempLoc);
            
        }
        occurs.Sort();

        //fill in the gpOrder list array by combinding theOrder[] and occurs[] lists
        for(int j = 0; j < listSize; j++)
        {
            gpOrder.Add(list[0]);
            list.RemoveAt(0);
            if(occurs.Count > 0 && occurs[0] == j)
            {
                gpOrder.Add(playerPos); //add the center tap/shoot
                occurs.RemoveAt(0);
            }
        }
        //Last tap should always be a shoot;
        if (gpOrder[gpOrder.Count - 1] != playerPos )
        {
            gpOrder.Add(playerPos);
        }



    }


    void OnDrawGizmos()
    {
        for (int x = 0; x < nodes.Length; x++)
        {
            if (nodes[x] != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(nodes[x].position, drawDis);
            }

        }
    }
}
