using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUnitHealth : ShowUnitStat {

	override protected float newStatValue(){
		return unit.GetComponent<UnitStats>().health;
	}

	override protected float statMax(){
		return unit.GetComponent<UnitStats> ().maxHealth;
	}
}
