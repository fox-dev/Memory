using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitAction : MonoBehaviour {

	[SerializeField]
	private GameObject attackHandler;

	[SerializeField]
	private string targetsTag;

	[SerializeField]
	private GameObject[] attacks = new GameObject[2];

	private GameObject gm;

	void Awake () {
		gm = GameObject.FindWithTag ("TurnSystem");

		this.attackHandler = Instantiate (this.attackHandler);

		this.attackHandler.GetComponent<AttackTarget> ().owner = this.gameObject;
	}

	GameObject findRandomTarget() {
		GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag (targetsTag);

		if (possibleTargets.Length > 0) {
			int targetIndex = Random.Range (0, possibleTargets.Length);
			GameObject target = possibleTargets [targetIndex];

			return target;
		}

		return null;
	}

	public void act() 
	{
		GameObject target = findRandomTarget ();

		AttackData attData = attacks[0].GetComponent<AttackData> ();
		this.attackHandler.GetComponent<AttackTarget> ().hit (target, attData);

		gm.GetComponent<ATBTurnSystem> ().turnTaken ();
	}
}
