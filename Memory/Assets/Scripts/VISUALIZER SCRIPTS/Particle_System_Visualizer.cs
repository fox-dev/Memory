using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_System_Visualizer : MonoBehaviour {

    ParticleSystem ps;
    ParticleSystem.VelocityOverLifetimeModule velocityMod;

    Color startColor, endColor;

    Renderer rend;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        velocityMod = ps.velocityOverLifetime;

        ColorUtility.TryParseHtmlString("#09AEF2FF", out endColor); //blueish
        ColorUtility.TryParseHtmlString("#FF0000FF", out startColor); //reddish

        rend = GetComponent<ParticleSystemRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        velocityMod.speedModifierMultiplier = AudioPeer._AmplitudeBuffer*3.5f;

        if(velocityMod.speedModifierMultiplier < 0.2f)
        {
            velocityMod.speedModifierMultiplier = 0.2f;
        }

        //main particle system module
        var main = ps.main;

        if (GameManager.gm.currentState == GameManager.GameState.memoryRecall)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if (GameManager.gm.currentState == GameManager.GameState.waiting)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if (GameManager.gm.currentState == GameManager.GameState.menu || GameManager.gm.currentState == GameManager.GameState.ready)
        {
            rend.material.SetColor("_TintColor", endColor);
        }
        else if (GameManager.gm.currentState == GameManager.GameState.memoryRelay&& EnemyRelayManager.erm.relayPhaseReady)
        {
            rend.material.SetColor("_TintColor", startColor);
        }
        else
        {
            rend.material.SetColor("_TintColor", endColor);
        }
    }
}
