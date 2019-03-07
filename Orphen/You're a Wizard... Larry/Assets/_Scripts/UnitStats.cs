using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStats : MonoBehaviour, IComparable {

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject damageTextPrefab;

	[SerializeField]
	private List<GameObject> effects;

	public float health;
	public float maxHealth;
	public float mana;
	public float maxMana;
	public float attack;
	public float magic;
	public float defense;
	public float speed;

	public bool loseTurn = false, takeAction;

	public float turnTime;

	private bool dead = false;

	public void calculateTurnTime() {
		this.turnTime -= this.speed;
	}

	public void resetTurn()
	{
		this.turnTime = 100;
	}

	public int CompareTo(object otherStats) {
		return turnTime.CompareTo (((UnitStats)otherStats).turnTime);
	}

	public bool isDead() {
		return this.dead;
	}

	public void takeDamage(float damage)
	{
		StartCoroutine (receiveDamage (damage, 1f));
	}

	public void takeDamage(float damage, float timeWait)
	{
		StartCoroutine (receiveDamage (damage, timeWait));
	}

	IEnumerator receiveDamage(float damage, float timeWait) {
		yield return new WaitForSeconds (timeWait);

		this.health -= damage;
		animator.Play ("Hit");

		GameObject HUDCanvas = GameObject.Find ("HUDCanvas");
		GameObject damageText = Instantiate (this.damageTextPrefab, HUDCanvas.transform) as GameObject;
		damageText.GetComponent<Text>().text = "" + damage;
		Vector3 position = this.gameObject.transform.position;
		damageText.transform.position = new Vector3 (position.x, position.y + 1.0f, position.z);
		damageText.transform.localScale = new Vector2 (1.0f, 1.0f);

		if (this.health <= 0) {
			this.dead = true;
			this.gameObject.SetActive (false);
		}
	}


}
