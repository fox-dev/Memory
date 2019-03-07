using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateEnemyMenuItems : MonoBehaviour {

	[SerializeField]
	private GameObject targetEnemyUnitPrefab;

	[SerializeField]
	private Vector2 initalPosition, itemDimensions;

	[SerializeField]
	private KillEnemy killEnemyScript;

	void Awake()
	{

		GameObject targetEnemyUnit = Instantiate (this.targetEnemyUnitPrefab, this.gameObject.transform) as GameObject;

		targetEnemyUnit.name = "Target" + this.gameObject.name;
		targetEnemyUnit.transform.localScale = new Vector2 (0.8f, 0.8f);
		targetEnemyUnit.GetComponent<Button> ().onClick.AddListener (() => selectEnemyTarget ());
	}

	public void selectEnemyTarget()
	{
		GameObject partyData = GameObject.FindWithTag ("PlayerParty");
		partyData.GetComponent<SelectUnit> ().attackEnemyTarget (this.gameObject);
	}
}
