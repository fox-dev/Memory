using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that manages difficulty setting
//Each difficulty has a set of nodes and enemies
//Set elements in inspector
public class Difficulty_Setting : MonoBehaviour {

    //game object nodes corresponding to difficulty path
    public GameObject[] node_Path;

    //game object nodes corresponding to enemies of difficulty
    public GameObject[] enemies;

    //Color selection for testing
    public Color color;

    ////////TESTING///////////
    //GIZMO DRAWING FOR DEBUGGING//
    public float drawDis = 1f; //Size of path objects, used for debugging

    void OnDrawGizmos()
    {
        for (int x = 0; x < node_Path.Length; x++)
        {
            if (node_Path[x] != null)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(node_Path[x].transform.position, drawDis);
            }

        }
    }

}
