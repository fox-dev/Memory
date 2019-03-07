using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script used on individual parametric cubes, used for changing colors between phases
public class ParametricCubeScript : MonoBehaviour {

    //Color transitions between phases
    Color startColor, endColor;

    Renderer rend;

    // Use this for initialization
    void Start () {

        rend = GetComponent<Renderer>();
        ColorUtility.TryParseHtmlString("#08587280", out endColor); //blueish
        ColorUtility.TryParseHtmlString("#FF0000FF", out startColor); //reddish
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.gm.currentState == GameManager.GameState.memoryRecall)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if (GameManager.gm.currentState == GameManager.GameState.waiting)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.menu || GameManager.gm.currentState == GameManager.GameState.ready)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if(GameManager.gm.currentState == GameManager.GameState.memoryRelay && EnemyRelayManager.erm.relayPhaseReady)
        {
            rend.material.SetColor("_TintColor", startColor);
        }
        else
        {
            rend.material.SetColor("_TintColor", endColor);
        }
    }

}


