using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBeam : MonoBehaviour {

    public float range;
    public int damage;
    public float maxWidth;

    //Growing and shrinking the beam/line renderer
    public float growingWidth;
    public float growingWidth2;

    //To reset the width when disabling beam for reuse;
    public float startingWidth;
    public float startingWidth2;

    Ray shootRay;
    RaycastHit shootHit;
    RaycastHit2D hit;
    public LayerMask shootableMask;
    LineRenderer gunLine;

    private GameObject player;
    // Use this for initialization
    void OnEnable()
    {
        CamEvents.camera.ShakeCam(0.15f, 0.25f);

        startingWidth = growingWidth;
        startingWidth2 = growingWidth2;

        //Assign player
        player = GameObject.FindGameObjectWithTag("Player");

        Vector2 point = player.transform.position;

        gunLine = GetComponent<LineRenderer>();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        shootRay.origin = transform.position;
        shootRay.direction = (point - currentPos);

        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
    }

    private void OnDisable()
    {
        growingWidth = startingWidth;
        growingWidth2 = startingWidth2;

    }

    // Update is called once per frame
    void Update()
    {
      

        //gunLine.SetPosition(0, currentPos);



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
}
