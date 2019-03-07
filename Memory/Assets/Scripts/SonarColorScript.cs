using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarColorScript : MonoBehaviour
{
    ParticleSystem ps;

    //Color transitions between phases
    Color startColor, endColor;

    Renderer rend;

    // Use this for initialization
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString("#096BF2FF", out endColor); //blueish
        ColorUtility.TryParseHtmlString("#FF0000FF", out startColor); //reddish

        rend.material.SetColor("_TintColor", endColor);
    }

 

    void Update()
    {
        //main particle system module
        var main = ps.main;

        if (GameManager.gm.currentState == GameManager.GameState.memoryRecall)
        {
            //main.startColor = endColor;
           rend.material.SetColor("_TintColor", endColor);
        }
        else if (GameManager.gm.currentState == GameManager.GameState.waiting)
        {
           // main.startColor = startColor;
            rend.material.SetColor("_TintColor", endColor);
        }
        else
        {
            //main.startColor = startColor;
            rend.material.SetColor("_TintColor", startColor);
        }
    }
}
