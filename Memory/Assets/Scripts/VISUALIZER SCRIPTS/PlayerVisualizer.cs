using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualizer : MonoBehaviour
{

    public bool beatPulse;


    private Enemy thisEnemy;

    private Transform myTransform;

    public float _startScale, _maxScale;
    public bool _useBuffer;

    private TrailRenderer tr;

    public float min, max, speed;

    Vector3 maxSize, minSize, beatSize;


    public float FinishTime;

    public int beats = 1;

    bool occupied;

    Color startBeat, endBeat;
    Renderer rend;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        myTransform = transform;

        //For default enemy values
        //_startScale = 0.25f;
        //_maxScale = 1.2f;

        maxSize = new Vector3(2f, 2f, 2f);
        minSize = new Vector3(1f, 1f, 1f);
        beatSize = new Vector3(3f, 3f, 3f);

        rend = GetComponent<Renderer>();

        startBeat = Color.white;
        endBeat = Color.red;

        

    }

    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pulsing();

        // pulse();
        if (beatPulse)
        {
            transform.localScale = new Vector3((AudioPeer._AmplitudeBuffer * _maxScale) + _startScale, (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale, (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale);

            tr.startWidth = (AudioPeer._AmplitudeBuffer * _maxScale) + _startScale;
        }
    }

    void pulse()
    {
        float range = max - min;
        transform.localScale = new Vector3(min + Mathf.PingPong(Time.time * speed, range), min + Mathf.PingPong(Time.time * speed, range), 1f);
    }

    void pulsing()
    {
        
        if (AudioPeer.ap.tutorial_Playing && beats != AudioPeer.ap.beats)
        {
            beats = AudioPeer.ap.beats;
            int count = 0;
            if(TutorialManager.tm.step3 && !TutorialManager.tm.step4)
            {
                count = (AudioPeer.ap.beats % 4);
                if (count == 0) { count = 4; }
            }
            

            if(GameManager.gm.currentState == GameManager.GameState.memoryRelay && TutorialManager.tm.step4)
            {
                count = AudioPeer.ap.beatsInPhase;
            }
            else if(GameManager.gm.currentState == GameManager.GameState.memoryRecall && TutorialManager.tm.step4)
            {
                count = AudioPeer.ap.beatsInRecallPhase;
            }

            if((EnemyRelayManager.erm.counting || GameManager.gm.currentState == GameManager.GameState.memoryRecall) && TutorialManager.tm.step3)
            {
                Vector2 placement = new Vector3(transform.position.x, transform.position.y + 1);
                FloatingTextController.CreateFloatingText(count.ToString(), placement);
            }
            
        }

        if (AudioPeer.ap.beats % 4 == 0)
        {
            //Only blink red during gameplay phases
            if (GameManager.gm.currentState == GameManager.GameState.menu  || GameManager.gm.currentState == GameManager.GameState.musicSelect || GameManager.gm.currentState == GameManager.GameState.credits)
            {
                //stay white
            }
            else
            {
                FindObjectOfType<SoundEffectManager>().Play("Beat");
                //FindObjectOfType<SoundEffectManager>().Play("Snare");
                rend.material.SetColor("_Color", endBeat);
            }

            transform.localScale = Vector3.Lerp(beatSize, minSize, (AudioPeer.ap.getTimer()/FinishTime));
        }
        else if (AudioPeer.ap.beats % 4 != 0)
        {
            if (GameManager.gm.currentState != GameManager.GameState.menu && GameManager.gm.currentState != GameManager.GameState.musicSelect && GameManager.gm.currentState != GameManager.GameState.credits)
            {
                FindObjectOfType<SoundEffectManager>().Play("Snare");
            }
            
            rend.material.SetColor("_Color", startBeat);
            transform.localScale = Vector3.Lerp(maxSize, minSize, AudioPeer.ap.getTimer() / FinishTime);
        }

        if (transform.localScale.x < minSize.x)
        {
            transform.localScale = minSize;
        }

    }

    public IEnumerator growing()
    {
        occupied = true;
        float timeToStart = Time.time;
        
        while(transform.localScale.x != beatSize.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, beatSize, (Time.time - timeToStart) * speed);
            yield return null;
        }
        
     

        occupied = false;


    }

    public IEnumerator shrink()
    {
        
        yield return null;
    }



}
