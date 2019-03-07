using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour{

	[SerializeField]
	private Sprite attackIcon;

	[SerializeField]
	private float[] damageType = new float[5];

	[SerializeField]
	private List<StatusEffect> effects;

	[SerializeField]
	private float attackDelay, manaCost;

	[SerializeField]
	private string attackAnimation;

	[SerializeField]
	private bool magicAttack;

	

	public float[] getDmg()
	{
		return damageType;
	}

	public List<StatusEffect> getEffects()
	{
		return effects;
	}

	public float getDelay()
	{
		return attackDelay;
	}

	public float getManaCost()
	{
		return manaCost;
	}

	public bool getAttackType()
	{
		return magicAttack;
	}

	public string getAttackAnimation()
	{
		return attackAnimation;
	}
}
