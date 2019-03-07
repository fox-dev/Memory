using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler current;
    GameObject objectForPool;
    FloatingText textForPool;
    public int pooledAmount = 20;
    public bool growth = true;

    public List<GameObject> listOfObjects;

    public float[] pooledAmounts;

    public List<GameObject> objectsForPool;

    ///////

    void Awake()
    {
        current = this;

    }

    void Start()
    {
        objectsForPool = new List<GameObject>();
        for (int i = 0; i < listOfObjects.Count; i++)
        {
            objectForPool = listOfObjects[i];
            for (int x = 0; x < pooledAmounts[i]; x++)
            {
                GameObject obj = Instantiate(objectForPool) as GameObject;
                obj.SetActive(false);
                if(obj.tag == "FloatingText" || obj.tag == "SlidingText")
                {
                    GameObject canvas = GameObject.Find("Canvas");
                    obj.transform.SetParent(canvas.transform, false); //false maintains global scale rather than taking on parent's scale

                }
                else
                {
                    obj.transform.parent = transform;
                }
                
                objectsForPool.Add(obj);
            }
        }
      
     
    }

    public GameObject getPooledObject(GameObject o)
    {
        for (int x = 0; x < objectsForPool.Count; x++)
        {
            if ((objectsForPool[x].name.Contains(o.name)) && !objectsForPool[x].gameObject.activeInHierarchy)
            {
                return objectsForPool[x];
            }
        }
        if (growth)
        {
            GameObject obj = Instantiate(o) as GameObject;
            obj.SetActive(false);
            if (obj.tag == "FloatingText" || obj.tag == "SlidingText")
            {
                GameObject canvas = GameObject.Find("Canvas");
                obj.transform.SetParent(canvas.transform, false); //false maintains global scale rather than taking on parent's scale

            }
            else
            {
                obj.transform.parent = transform;
            }
            objectsForPool.Add(obj);
            return obj;
        }
        return null;
    }




}

   