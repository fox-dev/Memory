using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUnitMana : ShowUnitStat {

	override protected float newStatValue(){
		return unit.GetComponent<UnitStats> ().mana;
	}

	override protected float statMax(){
		return unit.GetComponent<UnitStats> ().maxMana;
	}
}
