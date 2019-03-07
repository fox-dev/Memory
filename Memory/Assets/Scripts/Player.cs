using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject projectile;

    public EnemyRelayManager2 erm; //private reference to erm;

    /// <summary>
    /// MISFIRE THINGS
    /// </summary>
    public int rotateInterval;
    private int numShots = 0;
    private int maxShots;
    public float timeBetweenBullets = 0.15f;
    Quaternion direction;
    float shootRotation;
    private bool misfiring;
    /////////////////////

    public Renderer rend;
	// Use this for initialization
	void Start () {
        //assign main erm reference
        if (erm != null)
        {
            Debug.LogError("more than one erm in scene");

        }
        else
        {
            erm = EnemyRelayManager2.erm;
        }
        rend = GetComponent<Renderer>();
        rend.materials[0].color = Color.red;

        //Incase not set in inspector
        if(rotateInterval == 0)
        {
            Debug.Log("rotateInterval not set in inspector");
            rotateInterval = 72;
        }

        maxShots = 360/rotateInterval;
        misfiring = false;
        direction = Quaternion.Euler(shootRotation, 90, 0);
    }

    void Update()
    {
       // rend.materials[0].color = Color.red;
    }

    public void shoot()
    {
        
        GameObject beam = ObjectPooler.current.getPooledObject(projectile);
        if (beam == null) return;

        beam.transform.position = transform.position;
        beam.transform.rotation = transform.rotation;

        beam.gameObject.SetActive(true);
    }

    public void shoot(Transform o)
    {

        Vector3 position = new Vector3(o.position.x, o.position.y, o.position.z);
        
        GameObject beam = ObjectPooler.current.getPooledObject(projectile);

        if (beam == null) return;

        beam.transform.position = transform.position;
        beam.transform.rotation = transform.rotation;

        beam.GetComponent<LaserBeam>().SetTarget(position);

        beam.gameObject.SetActive(true);

        GameObject explosion = ObjectPooler.current.getPooledObject(Resources.Load("explosion") as GameObject);
        explosion.transform.position = o.position;
        explosion.transform.rotation = o.rotation;
        explosion.SetActive(true);

        o.gameObject.SetActive(false);
        
    }

    public void drawTarget(Transform o)
    {
        GameObject selected = ObjectPooler.current.getPooledObject(Resources.Load("selected") as GameObject);
        selected.transform.position = o.position;
        selected.transform.rotation = o.rotation;
        selected.SetActive(true);
    }

    public void takeDelayedShots(Transform parent)
    { 
        StartCoroutine(delayedShots(parent));
    }

    //Takes in an object with a # of children
    IEnumerator delayedShots(Transform parent)
    {

        foreach(Transform child in parent)
        {
            shoot(child);
            child.gameObject.SetActive(false);
            GameObject explosion = ObjectPooler.current.getPooledObject(Resources.Load("explosion") as GameObject);
            explosion.transform.position = child.position;
            explosion.transform.rotation = child.rotation;
            explosion.SetActive(true);
            yield return new WaitForSeconds(0f);
        }

        parent.gameObject.SetActive(false);

    }



    public void shootTargets(List<GameObject> targets)
    {
        foreach(GameObject o in targets)
        {
            if(o.tag == "Enemy")
            {
                shoot(o.transform);
            }
            else if(o.tag == "Enemy_Cluster")
            {
                takeDelayedShots(o.transform);
            }
        }

        GameObject[] crosshairs = GameObject.FindGameObjectsWithTag("Crosshair");
        foreach(GameObject o in crosshairs)
        {
            o.SetActive(false);
        }
    }

    public void clearTargetVisuals()
    {
        GameObject[] crosshairs = GameObject.FindGameObjectsWithTag("Crosshair");
        foreach (GameObject o in crosshairs)
        {
            o.SetActive(false);
        }
    }

    public void shootMisfire()
    {
        if(!misfiring)
        {
            StartCoroutine(shootRays());
        }
        
    }

    IEnumerator shootRays()
    {
        misfiring = true;

        while (numShots < maxShots)
        {
            GameObject bullet = ObjectPooler.current.getPooledObject(Resources.Load("ShineRay") as GameObject);

            if (bullet == null) break;

            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            direction = Quaternion.Euler(shootRotation, 90, 0);
            bullet.GetComponent<ShineRay>().assignShootDirection(direction);
            shootRotation += rotateInterval;

            numShots++;

            bullet.SetActive(true);

            yield return new WaitForSeconds(timeBetweenBullets);

        }

        numShots = 0;
        direction = Quaternion.Euler(shootRotation, 90, 0);

        misfiring = false;


    }


}
