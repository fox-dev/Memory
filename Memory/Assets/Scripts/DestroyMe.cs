using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour {

    public float aliveTime;

    void OnEnable()
    {

        //Destroy(gameObject, aliveTime);
        StartCoroutine(disable());
    }

    IEnumerator disable()
    {
        
        yield return new WaitForSeconds(aliveTime);
        transform.position = Vector3.zero;
        gameObject.SetActive(false);

    }

}
