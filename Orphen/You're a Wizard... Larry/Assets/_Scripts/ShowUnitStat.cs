using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShowUnitStat : MonoBehaviour {

	[SerializeField]
	protected GameObject unit;

	public float newValue = 0;
	public float newScale = 0;

	public Vector2 initialScale;

	// Use this for initialization
	void Start () {
		this.initialScale = this.gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {

		if (this.unit) {
			newValue = this.newStatValue ();
			newScale = (this.initialScale.x * newValue) / this.statMax();
			this.gameObject.transform.localScale = new Vector2 (newScale, this.initialScale.y);
		}
	}

	public void changeUnit(GameObject newUnit){
		this.unit = newUnit;
	}

	abstract protected float newStatValue();

	abstract protected float statMax ();
}
