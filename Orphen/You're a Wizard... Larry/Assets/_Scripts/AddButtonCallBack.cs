using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButtonCallBack : MonoBehaviour {

	[SerializeField]
	private bool att1, att2, att3, att4;

	private int attackNumber;

	private GameObject playerParty;

	// Use this for initialization
	void Start () {
		playerParty = GameObject.FindWithTag("PlayerParty");
		this.gameObject.GetComponent<Button> ().onClick.AddListener (() => addCallback ());
	}

	private void addCallback()
	{
		if (att1)
			attackNumber = 0;
		if (att2)
			attackNumber = 1;
		if (att3)
			attackNumber = 2;
		if (att4)
			attackNumber = 3;
		
		playerParty.GetComponent<SelectUnit> ().selectAttack (attackNumber);
	}


}
