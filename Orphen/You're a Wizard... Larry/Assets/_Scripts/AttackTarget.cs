using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour {

	public GameObject owner;

	[SerializeField]
	private string attackAnimation;

	[SerializeField]
	private bool magicAttack;

	[SerializeField]
	private float manaCost;

	[SerializeField]
	private float minAttackMultiplier;

	[SerializeField]
	private float maxAttackMultiplier;

	[SerializeField]
	private float minDefenseMultiplier;

	[SerializeField]
	private float maxDefenseMultiplier;

	public void hit(GameObject target, AttackData dmgInfo) 
	{
		magicAttack = dmgInfo.getAttackType();

		UnitStats ownerStats = this.owner.GetComponent<UnitStats> ();
		UnitStats targetStats = target.GetComponent<UnitStats> ();
		if (ownerStats.mana >= dmgInfo.getManaCost()) 
		{
			float attackMultiplier = (Random.value * (this.maxAttackMultiplier - this.minAttackMultiplier)) + this.minAttackMultiplier;
			float dmgStat = (this.magicAttack) ? ownerStats.magic : ownerStats.attack;

			float damage = (dmgInfo.getDmg () [Values.NORM] + dmgInfo.getDmg () [Values.FIRE] + dmgInfo.getDmg () [Values.WATER] + dmgInfo.getDmg () [Values.ELEC] +
			               dmgInfo.getDmg () [Values.EARTH]) + (attackMultiplier * dmgStat); 

			float defenseMultiplier = (Random.value * (this.maxDefenseMultiplier - this.minDefenseMultiplier)) + this.minDefenseMultiplier;
			damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

			this.owner.GetComponent<Animator> ().Play (dmgInfo.getAttackAnimation());

			targetStats.takeDamage (damage, 1f);

			ownerStats.mana -= this.manaCost;
		}
	}
}
