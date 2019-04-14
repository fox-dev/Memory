using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEvents : MonoBehaviour {

    //For external script use;
    public static CamEvents camera;

    //Shake strength, duration
    private float shakeAmount, shakeTimer;

    private Transform myTransform;

    float ElapsedTime;// FinishTime;

    public float startOrthoSize, targetOrthoSize;

    // Use this for initialization
    void Start () {
        myTransform = transform;

        camera = this;

        startOrthoSize = Camera.main.orthographicSize;

        ElapsedTime = 0f;
        //FinishTime = 1.5f;
    }
	
	// Update is called once per frame
	void Update () {

        ElapsedTime += Time.deltaTime;

        // Camera.main.orthographicSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, ElapsedTime / FinishTime);
        if (GameManager.gm.currentState != GameManager.GameState.menu && GameManager.gm.currentState != GameManager.GameState.credits && GameManager.gm.currentState != GameManager.GameState.options)
        {
            if (shakeTimer >= 0)
            {
                Vector2 shakePos = Random.insideUnitCircle * shakeAmount;

                myTransform.position = new Vector3(myTransform.position.x + shakePos.x, myTransform.position.y + shakePos.y, myTransform.position.z);

                shakeTimer -= Time.deltaTime;
            }
            else
            {
                myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, new Vector3(0, 0, -12.5f), 10f * Time.deltaTime);
            }
        }
        


          

    }

    public void ShakeCam (float shakeStr, float duration)
    {
        shakeAmount = shakeStr;
        shakeTimer = duration;
    }
}
