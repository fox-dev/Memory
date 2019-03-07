using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRelayerScript : MonoBehaviour
{

    public Transform[] path; //Enemy path
    public GameObject enemy; //The Enemy
    public Vector3 enemyStartPos; //Starting pos

    public float drawDis = 1f; //Size of path objects, used for debugging

    float ElapsedTime, FinishTime; //Start and End time for moving object, in seconds

    public bool returnToStart; //Flag to return enemies to start pos;

    public bool occupied; //Coroutine check

    [SerializeField]
    private int node; //Selected node to place enemy

    // Use this for initialization
    void Start()
    {
        enemyStartPos = enemy.transform.position;

        ElapsedTime = 0f;
        FinishTime = 0.5f;

        returnToStart = false;

        node = (int)Random.Range(0, path.Length);

    }

    // Update is called once per frame
    void Update()
    {
        //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, path[0].position, Time.deltaTime*5f);

        ElapsedTime += Time.deltaTime;
        if (!returnToStart) //move enemies into position
        {

            enemy.transform.position = Vector3.Lerp(enemyStartPos, path[node].position, ElapsedTime / FinishTime);
            if (enemy.transform.position == path[node].position)
            {
                returnToStart = true;
            }
        }

        else if (returnToStart)
        {
            if (!occupied)
            {
                StartCoroutine(returnPositions());
            }
        }

    }

    public IEnumerator returnPositions()
    {
        occupied = true;
        float timeToStart = Time.time;
        while (enemy.transform.position != enemyStartPos)
        {
            enemy.transform.position = Vector3.Lerp(path[node].position, enemyStartPos, (Time.time - timeToStart));

            yield return null;
        }


        returnToStart = false; //Enemy has returned to startpos
        node = (int)Random.Range(0, 3);
        ElapsedTime = 0; //Reset time flag to repeat cycle

        occupied = false;


    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < path.Length; x++)
        {
            if (path[x] != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(path[x].position, drawDis);
            }

        }
    }
}

