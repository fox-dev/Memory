using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//For sliding text animation at start of gameplay phase
public class SlidingText : MonoBehaviour {

    public Animator animator;
    public RectTransform musicTitle, artist; //objects holding corresponding text components
    Text titleText, artistText;
    AnimatorClipInfo[] clipInfo;

    void OnEnable()
    {
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        titleText = musicTitle.GetComponent<Text>();
        artistText = artist.GetComponent<Text>();
        //Destroy(gameObject, clipInfo[0].clip.length);
        StartCoroutine(hide());
    }

    public void setTitleAndArtist(string title, string artist)
    {
        titleText.text = title;
        artistText.text = artist;
    }


    IEnumerator hide()
    {

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }
}
