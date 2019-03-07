using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillDamageText : MonoBehaviour {

	[SerializeField]
	private float destroyTime;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, this.destroyTime);
	}

	void OnDestroy() {
	}
}
