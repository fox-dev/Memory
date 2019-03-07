using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller of floating text, call init() in gamemanager
public class FloatingTextController : MonoBehaviour {
    //Main UI canvavs
    private static GameObject canvas;

    public static void Init()
    {
        canvas = GameObject.Find("Canvas");
    }

    //Assign text, and screen to world location
    public static void CreateFloatingText(string text, Vector2 location)
    {
        GameObject temp = ObjectPooler.current.getPooledObject(Resources.Load("FloatingTextParent") as GameObject);
        if (temp == null) return;

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location);
        temp.transform.position = screenPosition;


        temp.GetComponent<FloatingText>().SetText(text);

        temp.gameObject.SetActive(true);
    }
}
