using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Updates loading bar while loading next scene
public class SceneLoader : MonoBehaviour {

    public RectTransform loadingObject; //set in inspector
    public RectTransform loadingObject_Outline; //set in inspector
    private Image loadingImage;
    private Image loadingImage_Outline;
    public Text loadingText; //set in inspector

    public bool flip = false;
    float startTime = 0;

    private void Awake()
    {
        loadingImage = loadingObject.GetComponent<Image>();
        loadingImage_Outline = loadingObject_Outline.GetComponent<Image>();
    }

    private void Start()
    {
        LoadScene(1); //Load main scene
    }

    void animate()
    {

        if (loadingImage_Outline.fillAmount == 1.0f && !flip)
        {
            startTime = Time.time;
            flip = true;
           
        }

        if (loadingImage_Outline.fillAmount == 0f && flip)
        {
            startTime = Time.time;
            flip = false;
        }

        if (flip)
        {
            loadingObject_Outline.rotation = Quaternion.Euler(0, 180, 0);
            loadingImage_Outline.fillAmount = Mathf.Lerp(1, 0, (Time.time - startTime) / 2f);

        }
        else
        {
            loadingObject_Outline.rotation = Quaternion.Euler(0, 0, 0);
            loadingImage_Outline.fillAmount = Mathf.Lerp(0, 1, (Time.time - startTime) / 2f);

        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsycnhronously(sceneIndex));
    }

    IEnumerator LoadAsycnhronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingImage.fillAmount = progress;
            animate();
            loadingText.text = (progress * 100f).ToString("F0") + "%";
            //Debug.Log(progress);

            yield return null;
        }
    }

    IEnumerator tester()
    {
        startTime = Time.time;
        while(true)
        {
            animate();
            yield return null;
        }
    }

}
