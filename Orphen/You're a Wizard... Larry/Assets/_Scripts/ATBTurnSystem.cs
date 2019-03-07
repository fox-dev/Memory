using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATBTurnSystem : MonoBehaviour {

	private List<UnitStats> unitsStats;
	private UnitStats currentUnitStats;

	private GameObject playerParty;

	private GameObject[] enemyUnits;

	public GameObject enemyEncounter;

	[SerializeField]
	private GameObject actionsMenu;

	private bool takingTurn, battleOver;

	private IEnumerator tickRoutine;

	// Use this for initialization
	void Start () 
	{
		takingTurn = false;
		battleOver = false;

		this.playerParty = GameObject.FindWithTag("PlayerParty");

		unitsStats = new List<UnitStats> ();

		GameObject[] playerUnits = GameObject.FindGameObjectsWithTag ("PlayerUnit");
		foreach (GameObject playerUnit in playerUnits) {
			UnitStats currentUnitStats = playerUnit.GetComponent<UnitStats> ();
			currentUnitStats.turnTime = 100;
			unitsStats.Add (currentUnitStats);
		}
		enemyUnits = GameObject.FindGameObjectsWithTag ("EnemyUnit");

		foreach (GameObject enemyUnit in enemyUnits) {
			UnitStats currentUnitStats = enemyUnit.GetComponent<UnitStats> ();
			currentUnitStats.turnTime = 100;
			unitsStats.Add (currentUnitStats);

			Transform child = enemyUnit.transform.GetChild (1);

			if (child.tag == "HUD") {
				child.gameObject.SetActive (false);
			}
		}
		unitsStats.Sort();

		this.actionsMenu.SetActive (false);

		tickRoutine = tick (.5f);

		StartCoroutine (tickRoutine);

	}

	private IEnumerator tick(float time)
	{
		while (!battleOver) {
			yield return new WaitForSeconds (time);

			updateTurns ();

			UnitStats unit = unitsStats[0];
			while (unit.turnTime <= 0) {
				unitsStats.Remove (unit);

				yield return StartCoroutine (takeTurn (unit));

				unit = unitsStats [0];
			}
		}
			
	}

	private void updateTurns()
	{
		foreach (UnitStats unit in unitsStats) {
			unit.calculateTurnTime ();
		}

		unitsStats.Sort ();
	}

	private IEnumerator takeTurn(UnitStats activeUnit)
	{
		takingTurn = true;
		GameObject currentUnit = activeUnit.gameObject;
		if (!activeUnit.isDead ()) {

			if (currentUnit.tag == "PlayerUnit") {
				this.playerParty.GetComponent<SelectUnit> ().selectCurrentUnit (currentUnit.gameObject);
			} else {
				currentUnit.GetComponent<EnemyUnitAction> ().act ();
			}

			yield return StartCoroutine (listenForEndTurn ());

			activeUnit.resetTurn ();
			unitsStats.Add (activeUnit);
			unitsStats.Sort ();
		} 
	}

	private IEnumerator listenForEndTurn()
	{
		while (takingTurn) {
			yield return null;
		}

		yield return new WaitForSeconds (1.5f);
	}

	public void turnTaken()
	{
		this.takingTurn = false;
	}

	public void activateEnemyMenu()
	{
		foreach (GameObject enemyUnit in enemyUnits) {

			if (enemyUnit.activeSelf) {
				Transform child = enemyUnit.transform.GetChild (1);

				if (child.tag == "HUD") {
					child.gameObject.SetActive (true);
				}
			}
		}
	}

	public void deactivateEnemyMenu()
	{
		foreach (GameObject enemyUnit in enemyUnits) {

			if (enemyUnit.activeSelf) {
				Transform child = enemyUnit.transform.GetChild (1);

				if (child.tag == "HUD") {
					child.gameObject.SetActive (false);
				}
			}
		}
	}
}
