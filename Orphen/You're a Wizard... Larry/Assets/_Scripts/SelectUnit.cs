using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectUnit : MonoBehaviour {

	private GameObject currentUnit;

	private GameObject actionsMenu;

	GameObject system;

	void Awake(){
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Battle") {
			this.actionsMenu = GameObject.Find ("PlayerMenu");
			this.system = GameObject.Find ("TurnSystem");
		}
	}

	public void selectCurrentUnit(GameObject unit){
		this.currentUnit = unit;

		this.actionsMenu.SetActive (true);

		this.currentUnit.GetComponent<PlayerUnitCustomActions> ().updateHUD ();
	}

	public void selectAttack(int attNum){
		this.currentUnit.GetComponent<PlayerUnitCustomActions>().selectAttack(attNum);

		this.actionsMenu.SetActive(false);
		this.system.GetComponent<ATBTurnSystem>().activateEnemyMenu();
	}

	public void attackEnemyTarget(GameObject target){
		this.actionsMenu.SetActive (false);
		this.system.GetComponent<ATBTurnSystem>().deactivateEnemyMenu();

		this.currentUnit.GetComponent<PlayerUnitCustomActions> ().act (target);
	}

	public GameObject getCurrentUnit()
	{
		return currentUnit;
	}
}
