using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePlayer : MonoBehaviour {

	[SerializeField]
	private Transform partyPosition;
	[SerializeField]
	private GameObject playerPartyPrefab;

	public void instantiatePlayer()
	{
		GameObject playerParty = Instantiate (this.playerPartyPrefab) as GameObject;

		playerParty.transform.position = partyPosition.position;
	}
}
