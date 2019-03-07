using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUnitCustomActions : MonoBehaviour {

	[SerializeField]
	private Sprite faceSprite;

	[SerializeField]
	private GameObject attackHandler;

	[SerializeField]
	private GameObject[] attacks = new GameObject[4];

	private GameObject playerUnitFace, playerUnitHealthBar, playerUnitManaBar;

	private GameObject currentAttack;

	private GameObject gm;

	void Awake () 
	{
		SceneManager.sceneLoaded += OnSceneLoaded;

		this.attackHandler = Instantiate (this.attackHandler, this.transform) as GameObject;

		this.attackHandler.GetComponent<AttackTarget> ().owner = this.gameObject;

		instantiateAttacks ();

	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Battle") {
			gm = GameObject.FindWithTag ("TurnSystem");

			playerUnitFace = GameObject.Find ("PlayerUnitFace") as GameObject;
			playerUnitHealthBar = GameObject.Find ("Health") as GameObject;
			playerUnitManaBar = GameObject.Find ("Mana") as GameObject;
			updateHUD();
		}
	}

	public void act(GameObject target) {
		AttackData attData = currentAttack.GetComponent<AttackData> ();
		this.attackHandler.GetComponent<AttackTarget>().hit (target, attData);
		gm.GetComponent<ATBTurnSystem> ().turnTaken ();
	}

	public void selectAttack(int attNum)
	{
		this.currentAttack = attacks [attNum];
	}

	public void updateHUD(){

		playerUnitFace.GetComponent<Image> ().sprite = this.faceSprite;

		playerUnitHealthBar.GetComponent<ShowUnitHealth> ().changeUnit (this.gameObject);

		playerUnitManaBar.GetComponent<ShowUnitMana> ().changeUnit (this.gameObject);
	}

	public void instantiateAttacks()
	{
		for (int i = 0; i < attacks.Length; i++) {
			attacks [i] = Instantiate (attacks [i], this.gameObject.transform) as GameObject;
		}
	}
}
