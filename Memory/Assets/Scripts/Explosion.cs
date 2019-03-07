using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Explosion : MonoBehaviour {

    public float aliveTime;

    // Use this for initialization
    void OnEnable () {

        //Destroy(this.gameObject, 3f);
        StartCoroutine(disable());
    }

    IEnumerator disable()
    {

        yield return new WaitForSeconds(aliveTime);
        transform.position = Vector3.zero;
        gameObject.SetActive(false);

    }

}
