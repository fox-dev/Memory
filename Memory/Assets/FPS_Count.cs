using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Count : MonoBehaviour 
{

    int fps = 0;
    GameObject textItem;
    Text theText;

    private void Start()
    {
        textItem = transform.gameObject;
        theText = textItem.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        fps = (int)(Time.frameCount / Time.time);
        theText.text = fps.ToString();
    }
}
