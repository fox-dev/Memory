using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//For floating text above objects
public class FloatingText : MonoBehaviour {

    //Set in inspector
    public Animator animator;
    private Text textString;
    AnimatorClipInfo[] clipInfo;

    void OnEnable()
    {
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Destroy(gameObject, clipInfo[0].clip.length);

        textString = animator.GetComponent<Text>();

        StartCoroutine(hide());
    }

    public void SetText(string t)
    {
        textString.text = t;
    }

    IEnumerator hide()
    {

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }

}


