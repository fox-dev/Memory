using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
	[SerializeField]
	GameObject target;

	[SerializeField]
	private float dmg;

	[SerializeField]
	private int duration;

	[SerializeField]
	private float[] statChange = new float[8];

	[SerializeField]
	private bool loseTurn;


	public virtual void resolve()
	{
		target.GetComponent<UnitStats>().takeDamage (dmg, .5f);
		target.GetComponent<UnitStats>().loseTurn = this.loseTurn;
		duration--;
	}


}
