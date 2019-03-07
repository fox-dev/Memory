using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512Cubes : MonoBehaviour {

    public GameObject _sampleCubePrefab;

    private List<GameObject> _sampleCube = new List<GameObject>();

    public bool useBuffer;

    public float _maxScale;

    // Use this for initialization
    void Start () {
        //int i = 0; i < 180; i++//int i = 0; i < 512; i++
        /*
        for (int i = 0; i < 180; i++)
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SameplCube" + i;
            _instanceSampleCube.layer = 12;

             this.transform.eulerAngles = new Vector3(0, -2f * i, 0); //360/512 cubes
            //this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0); //360/512 cubes
            _instanceSampleCube.transform.position = Vector3.forward * 100;
            _sampleCube[i] = _instanceSampleCube;
        }
        */

        foreach(Transform child in transform)
        {
            _sampleCube.Add(child.gameObject);
        }

        this.transform.eulerAngles = new Vector3(0, 90, 90);
        this.transform.position = new Vector3(0, 0, 150);
        this.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

    }
	
	// Update is called once per frame
	void Update () {
        //int i = 0; i < 180; i++//int i = 0; i < 512; i++
        for (int i = 0; i < 180; i++)
        {
            if(_sampleCube != null)
            {
                if(useBuffer)
                {
                    _sampleCube[i].transform.localScale = new Vector3(1, (AudioPeer._bufferSamples[i] * _maxScale) + 2, 1);
                }
                else
                {
                    _sampleCube[i].transform.localScale = new Vector3(1, (AudioPeer._samples[i] * _maxScale) + 2, 1);
                }

                if (!AudioPeer._audioSource.isPlaying)
                {
                    _sampleCube[i].transform.localScale = new Vector3(1, 2, 1);
                }
                
            }
        }

       
		
	}


}
