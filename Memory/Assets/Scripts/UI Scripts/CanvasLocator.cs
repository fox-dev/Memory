using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script attached to UI CanvasLocator objects for lerping them or performing UI animations
public class CanvasLocator : MonoBehaviour
{

    //CanvasLocator's transform
    private RectTransform myTransform;

    //Screen positions
    private Vector3 offPos = new Vector3(0, 500, 0); //onScreen
    private Vector3 onPos = Vector3.zero; //offScreen

    private void Awake()
    {
        myTransform = this.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (myTransform.parent.name == "MusicSelect_UI" || myTransform.parent.name == "Credits_UI" || myTransform.parent.name == "Options_UI")
        {
            MusicSelectUI_lerpOn();
        }
    }

    public void Generic_Disable()
    {
        if (myTransform.parent.name != "MusicSelect_UI" && myTransform.parent.name != "Credits_UI" && myTransform.parent.name != "Options_UI")
        {
            myTransform.parent.gameObject.SetActive(false);
        }
        else
        {
            MusicSelectUI_lerpOff();
        }
 
    }

    //MUSIC SELECT METHODS//
    IEnumerator MusicSelectUI_lons()
    {
        float startTime = Time.time;

        while (myTransform.GetComponent<RectTransform>().localPosition != onPos)
        {
            myTransform.GetComponent<RectTransform>().localPosition = Vector3.Lerp(offPos, onPos, (Time.time - startTime) / 0.2f);
            yield return null;
        }
    }

    IEnumerator MusicSelectUI_loffs()
    {
        float startTime = Time.time;

        while (myTransform.GetComponent<RectTransform>().localPosition != offPos)
        {
            myTransform.GetComponent<RectTransform>().localPosition = Vector3.Lerp(onPos, offPos, (Time.time - startTime) / 0.2f);
            yield return null;
        }

        myTransform.parent.gameObject.SetActive(false);
    }

    public void MusicSelectUI_lerpOff()
    {
        StartCoroutine(MusicSelectUI_loffs());
    }

    public void MusicSelectUI_lerpOn()
    {
        StartCoroutine(MusicSelectUI_lons());
    }
    ////////////////////////
}
