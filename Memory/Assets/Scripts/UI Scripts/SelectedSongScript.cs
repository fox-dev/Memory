using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script to animate selection bar in music select
public class SelectedSongScript : MonoBehaviour {

    private Image image;

    float fillStep = 0.0f;
    public float fillTime; //time it takes to fill

	// Use this for initialization
	void Awake () {
        image = GetComponent<Image>();
        image.fillAmount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if(image.fillAmount < 1f)
        {
            fillStep += Time.deltaTime / fillTime;
            image.fillAmount = Mathf.Lerp(0, 1f, fillStep);
        }
	}

    public void resetSelectionBar_FillAmount()
    {
        fillStep = 0;
        image.fillAmount = 0;
    }
}
