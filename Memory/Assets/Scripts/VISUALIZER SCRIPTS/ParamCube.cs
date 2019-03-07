using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour {

    public int _band;
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

        if (AudioPeer._audioSource.isPlaying)
        {
            if (_useBuffer)
            {
                transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._audioBandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._audioBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
            }
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
        }

        
        
		
	}
}
