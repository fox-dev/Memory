using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineRay : MonoBehaviour
{

    public float range;
    public float maxWidth;

    //Growing and shrinking the beam/line renderer
    public float growingWidth;
    public float growingWidth2;

    //To reset the width when disabling beam for reuse;
    public float startingWidth;
    public float startingWidth2;

    Ray shootRay;
    RaycastHit shootHit;

    public LayerMask shootableMask;
    LineRenderer gunLine;

    Quaternion shootDirection;

    void Start()
    {
        shootDirection = Quaternion.Euler(0, 90, 0);
    }
    void OnEnable()
    {
        CamEvents.camera.ShakeCam(0.1f, 0.15f);

        startingWidth = growingWidth;
        startingWidth2 = growingWidth2;


        //Vector3 pos_at_z_0 = crosshair.transform.position;
        Vector3 pos_at_z_0 = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        ////////////////

        gunLine = GetComponent<LineRenderer>();

        shootRay.origin = transform.position;

        shootRay.direction = shootDirection * Vector3.forward;

        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);

    }

    void OnDisable()
    {
        //gunLine.SetWidth(startWidth, 0.025f);
        growingWidth = startingWidth;
        growingWidth2 = startingWidth2;

  

    }


    // Update is called once per frame
    void Update()
    {

        if (growingWidth <= maxWidth - 1f)
        {
            growingWidth = Mathf.Lerp(growingWidth, maxWidth, Time.deltaTime * 7f);
            gunLine.startWidth = growingWidth;
        }
        else if (growingWidth >= maxWidth - 1f)
        {
            growingWidth2 = Mathf.Lerp(growingWidth2, 0, Time.deltaTime * 10f);
            gunLine.startWidth = growingWidth2;
        }



    }

    public void assignShootDirection(Quaternion q)
    {

        shootDirection = q;
    }


}
