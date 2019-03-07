using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootShineRay : MonoBehaviour
{


    public GameObject projectile;
    public int rotateInterval;

    private int numShots;
    private int maxShots;

    public float timeBetweenBullets = 0.15f;

    Quaternion direction;
    float shootRotation;

    void Start()
    {


        shootRotation = 0;
        direction = Quaternion.Euler(shootRotation, 90, 0); //For fireball, rotate along Y-Axis
        maxShots = 360 / rotateInterval;

     

    }

    void OnEnable()
    {
        numShots = 0;
        maxShots = 5;
        shootRotation = 0;
        direction = Quaternion.Euler(shootRotation, 90, 0); //For fireball, rotate along Y-Axis
        maxShots = 360 / rotateInterval;
        StartCoroutine(shootRays());

    }

    void OnDisable()
    {
        numShots = 0;
        this.gameObject.SetActive(false);
    }


    IEnumerator shootRays()
    { 
    
        while(numShots < maxShots)
        {
            GameObject bullet = ObjectPooler.current.getPooledObject((Resources.Load("ShineRay") as GameObject));

            if (bullet == null) break;

            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            direction = Quaternion.Euler(shootRotation, 90, 0);
           // bullet.GetComponent<ShineRay>().assignShootDirection(direction);
            shootRotation += rotateInterval;

            numShots++;

            bullet.SetActive(true);

            yield return new WaitForSeconds(timeBetweenBullets);

        }

        this.gameObject.SetActive(false);

        
    }


}