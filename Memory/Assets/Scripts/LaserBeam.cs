using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

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
    RaycastHit2D hit;
    public LayerMask shootableMask;
    LineRenderer gunLine;

    private bool targeting = false;
    private Vector3 target = Vector3.zero;


    // Use this for initialization
    void OnEnable()
    {
        CamEvents.camera.ShakeCam(0.1f, 0.15f);
        startingWidth = growingWidth;
        startingWidth2 = growingWidth2;


        Vector2 point;

        if (!targeting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float z_plane_of_2d_game = 2;
            Vector3 pos_at_z_0 = ray.origin + ray.direction * (z_plane_of_2d_game - ray.origin.z) / ray.direction.z;
            point = new Vector2(pos_at_z_0.x, pos_at_z_0.y);
        }
        else
        {
            point = new Vector2(target.x, target.y);
        }


        gunLine = GetComponent<LineRenderer>();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        shootRay.origin = transform.position;
        shootRay.direction = (point - currentPos);

        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);

        
    }


    public void SetTarget(Vector3 obj)
    {
        targeting = true;
        target = obj;
    }

    private void OnDisable()
    {

        //reset parameters//
        growingWidth = startingWidth;
        growingWidth2 = startingWidth2;

        targeting = false;
        target = Vector3.zero;

    }

    // Update is called once per frame
    void Update () {
       



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
