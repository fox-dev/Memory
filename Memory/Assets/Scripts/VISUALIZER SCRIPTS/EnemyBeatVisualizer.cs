using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeatVisualizer : MonoBehaviour {

    public bool beatPulse;

    private Enemy thisEnemy;

    private Transform myTransform;

    public float _startScale, _maxScale;
    public bool _useBuffer;

    private TrailRenderer tr;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        thisEnemy = GetComponent<Enemy>();
    }

    // Use this for initialization
    void Start () {
        myTransform = transform;

        //For default enemy values
        //_startScale = 0.25f;
        //_maxScale = 1.2f;

    }

    void OnEnable()
    {
        if(thisEnemy != null)
        {
            if (thisEnemy.tag != "Enemy_Cluster")
            {
                //Beating has been enabled
                thisEnemy.setBeating();
            }
        }
        else
        {
            //Debug.Log("ENEMY SCRIPT NOT YET INSTANCED FOR " + transform.name);
        }
    }

    // Update is called once per frame
    void Update () {
        if (beatPulse)
        {
            transform.localScale = new Vector3((AudioPeer._AmplitudeBuffer * _maxScale) + _startScale, (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale, (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale);

            tr.startWidth = (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale;

        }
    }
}
