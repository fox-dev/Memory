using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller of sliding text, call init() in gamemanager
public class SlidingTextController : MonoBehaviour {

    //Main UI canvavs
    private static GameObject canvas;

    public static void Init()
    {
        canvas = GameObject.Find("Canvas");
    }

    //Assign text, and screen to world location
    public static void CreateSlidingText(string title, string artist, Vector2 location)
    {
        GameObject temp = ObjectPooler.current.getPooledObject(Resources.Load("SlidingTextParent") as GameObject);
        if (temp == null) return;

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location);
        temp.transform.position = screenPosition;


        temp.GetComponent<SlidingText>().setTitleAndArtist(title, artist);


        temp.gameObject.SetActive(true);
    }
}
