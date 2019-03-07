using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour {

    public GameObject projectile;

    private bool occupied; //for coroutine use;

    public Vector3 targetPosition;

    public float ElapsedTime, FinishTime; //Start and End time for moving object, in seconds 

    private Transform myTransform;

    private Vector3 startPos;

    Color startColor, endColor;

    Renderer rend;

    private int currentBeat = 0;

    //beat visualizer for enemy is enabled
    bool beating;

    //Return bool on player fail;
    bool retreat;

    //fireing boolean for clusters, firing not yet finished
    bool firing;


    // Use this for initialization
    void Start () {

        beating = false;
        retreat = false;
        firing = false;

        occupied = false;
        
        rend = GetComponent<Renderer>();
        myTransform = transform;

        if (this.tag == "Enemy")
        {
            //GetComponent<Renderer>().material.color = Color.blue;
            GetComponent<EnemyBeatVisualizer>().enabled = false;
        }
        else
        {
            foreach (Transform child in transform)
            {
                //child.GetComponent<Renderer>().material.color = Color.blue;
                if (child.tag != "Crosshair")
                {
                    child.GetComponent<EnemyBeatVisualizer>().enabled = false;
                }

            }
        }
    
        ElapsedTime = 0;
        if (this.tag == "Enemy")
        {
            FinishTime = 0.25f;
        }
        else
        {
            FinishTime = 0.25f;
        }
        
        startPos = transform.position;

        endColor = new Color();
        startColor = new Color();

        ColorUtility.TryParseHtmlString("#00FFD580", out endColor); //blueish
        ColorUtility.TryParseHtmlString("#FF0000FF", out startColor); //reddish

        if (this.tag == "Enemy" || this.tag == "PartOfCluster")
        {
            //GetComponent<Renderer>().material.color = Color.blue;
            GetComponent<EnemyBeatVisualizer>().enabled = false;
            beating = false;
            rend.material.SetColor("_TintColor", startColor);
        }

    }


    void FixedUpdate()
    {
        ElapsedTime += Time.deltaTime;
        if(myTransform.tag != "PartOfCluster" && !retreat)
        {
            myTransform.position = Vector3.Lerp(startPos, targetPosition, ElapsedTime / FinishTime);
        }
        else if(myTransform.tag != "PartOfCluster" && retreat && !firing)
        {
            myTransform.position = Vector3.Lerp(targetPosition, startPos, ElapsedTime / FinishTime);
        }

        if(beating)
        {
            //rend.material.shader = Shader.Find("Custom/Additive");
            rend.material.SetColor("_TintColor", endColor);
            

        }

        //Retreat successful, disable
        if(retreat && !firing && myTransform.position == startPos)
        {
            gameObject.SetActive(false);
        }
        
    }


    void OnEnable()
    {
        if(targetPosition == null)
        {
            Debug.Log("NO TARGET POSITION SET FOR " + myTransform.name);
        }
        if(this.tag == "Enemy_Cluster")
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);

            }
        }

        currentBeat = AudioPeer.ap.beats;

    }


    void OnDisable()
    {
        if(this.tag == "Enemy" || this.tag == "PartOfCluster")
        {
            //GetComponent<Renderer>().material.color = Color.blue;
            GetComponent<EnemyBeatVisualizer>().enabled = false;
            beating = false;
            rend.material.SetColor("_TintColor", startColor);
        }
        else
        {
            foreach (Transform child in transform)
            {
                //child.GetComponent<Renderer>().material.color = Color.blue;
                child.GetComponent<EnemyBeatVisualizer>().enabled = false;
                beating = false;
                
            }
        }

        if(this.tag == "Enemy" || this.tag == "Enemy_Cluster")
        {
            ///Reset elapsed time
            ElapsedTime = 0;
            //Move back to start pos
            myTransform.position = startPos;
        }
        //default scale
        transform.localScale = new Vector3(1, 1, 1);

        //Make sure to disable
        retreat = false;
        firing = false;
        
    }

    public void fallBack()
    {
        StartCoroutine(retreatFire());
    }

    public void GameEnded()
    {
        StartCoroutine(returnToStart());
    }

    IEnumerator retreatFire()
    {
        if (gameObject.tag == "Enemy")
        {
            shoot(gameObject); //origin is self
        }
        else if(gameObject.tag == "Enemy_Cluster")
        {
            clusterShoot();
        }

        yield return new WaitForSeconds(0.5f);

        ElapsedTime = 0;
        retreat = true;
        
    }

    IEnumerator returnToStart()
    {
        yield return new WaitForSeconds(0.5f);

        ElapsedTime = 0;
        retreat = true;
    }

    //Origin is this enemy's current position
    public void shoot(GameObject origin)
    {
        //Instantiate(projectile);
        GameObject beam = ObjectPooler.current.getPooledObject(projectile);
        if (beam == null) return;

        beam.transform.position = origin.transform.position;
        beam.transform.rotation = origin.transform.rotation;

        beam.gameObject.SetActive(true);

        //if retreating, reset elapsed time before moving
        if (retreat)
        {
            ElapsedTime = 0f;
        }

    }

    //enemy lasers automatically lock onto player, no need to specify target
    public void clusterShoot()
    {
        if(transform.tag != "Enemy_Cluster")
        {
            Debug.Log("Trying to fire non-cluster enemy");
        }
        else
        {
            if(!occupied)
            {
                StartCoroutine(delayedShots());
            }
            
        }


    }

    IEnumerator delayedShots() //For enemy clusters
    {
        occupied = true;
        firing = true;

        foreach(Transform child in transform)
        {
            if(child.tag == "PartOfCluster")
            {
                shoot(child.gameObject); //specify origin
            }
            yield return new WaitForSeconds(0.2f);
        }

        //if retreating, reset elapsed time before moving
        if(retreat)
        {
            ElapsedTime = 0f;
        }

        firing = false;
        occupied = false;

        /*
        GameObject explosion = ObjectPooler.current.getPooledObject(Resources.Load("explosion") as GameObject);
        explosion.transform.position = child.position;
        explosion.transform.rotation = child.rotation;
        explosion.SetActive(true);
        */
    }

    //Called by relay manager in assignPositions()
    public void AssignPosition(Transform pos)
    {
        targetPosition = pos.position;
    }


    public bool InPosition()
    {
        if(myTransform == null)
        {
            return transform.position == targetPosition;
        }
        else
        {
            return myTransform.position == targetPosition;
        }
    }

    public bool InStartPosition()
    {
        if (myTransform == null)
        {
            return transform.position == startPos;
        }
        else
        {
            return myTransform.position == startPos;
        }
    }

    //Beating has been turned on, call this in enemybeatvisualizer script
    public void setBeating()
    {
        beating = true;
    }
    


}
