using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLayerSetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().sortingLayerName = "Background";
        GetComponent<MeshRenderer>().sortingOrder = 3;

    }
	
}
