using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Song select UI script to handle display
public class SongSelectUI : MonoBehaviour {

    //Scroll Rect of thie song select UI;
    public ScrollRect scrollRect;

    //SongPanel Button to instantiate from prefab
    public GameObject _songPanelPrefab;

    //array of song panel/buttons in music select screen
    public List<GameObject> songs;

    //Selected song image, set in inspector
    public RectTransform selectedSong; //image bar for highlighting selected song/panel
    private int selectedSongIndex; //index
    private Vector3 selectSongUIPos; //selected panel


    //Music info panel for selected song, Set in inspector
    public RectTransform songInfoTextPanel;
    //Lerp positions for info panel
    private Vector2 onScreen = new Vector3(0, -208);
    private Vector2 offScreen = new Vector3(-350, -208);

    public Sprite defaultArt; //Default song art, change these songs later, set in inspector

    //Music Score info panel for selected song, Set in inspector
    public RectTransform playerScoreInfoPanel;

    /// <summary>
    /// PlayerInfoPanel button difficulty selections, set in inspector
    /// Text objects needed to change text color
    /// </summary>
    public RectTransform selectedButton;  //image bar for highlighting selected button
    public List<GameObject> buttons;
    private Text selectedText; //assign text component when tapping;
    private Vector3 selectedButtonPos; //difficulty
    private GameObject difficulty_button;
    private Color unselected;

    //To Disable For tutorial// Set in inspector
    public GameObject hardButton;
    public GameObject normalButton;

    // Use this for initialization
    void Start () {
        ColorUtility.TryParseHtmlString("#FF1E1DFF", out unselected);
        PopulateSongButtons();
        setDefaults();
        CenterToItem();
    }


    private void OnEnable()
    {
 
        ColorUtility.TryParseHtmlString("#FF1E1DFF", out unselected);
        populateInfoPanel();
        populateScoreInfo();
        
        setDefaults();
        CenterToItem();

    }

    public void setDefaults()
    {
     
        selectedSongIndex = AudioPeer.ap.selectClip_index;
        if(songs.Count != 0 )
        {
            selectSongUIPos = songs[selectedSongIndex].GetComponent<RectTransform>().position;
            selectedSong.position = selectSongUIPos;
            selectedSong.SetParent(songs[selectedSongIndex].transform);
            selectedSong.SetSiblingIndex(0);
        }
        selectedButtonPos = buttons[0].GetComponent<RectTransform>().position; //easy button
        GameManager.gm.difficulty = GameManager.Difficulty.easy;

        populateInfoPanel();
        populateScoreInfo();

        if (selectedSongIndex == 0)
        {
            HighlightThisButton(buttons[0]);
        }
        else
        {
            normalButton.SetActive(true);
            hardButton.SetActive(true);
            if(difficulty_button != null)
            {
                HighlightThisButton(difficulty_button);
            }
            else
            {
                HighlightThisButton(buttons[0]);
            }
            
            
            
        }

       
        AudioPeer.ap.changeSong_MusicSelect_Preview(selectedSongIndex);
    }

    public void SelectSong()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != songs[selectedSongIndex])
        {

            //Begin lerping out, then call lerping in
            StartCoroutine(LerpInfoPanel_Off());

            //Play sound effect
            FindObjectOfType<SoundEffectManager>().Play("ChangeSong");

            selectSongUIPos = selected.GetComponent<RectTransform>().position;
            selectedSong.position = selectSongUIPos;
            selectedSong.SetParent(selected.transform);
            selectedSong.SetSiblingIndex(0);

            selectedSongIndex = songs.IndexOf(selected);

            AudioPeer.ap.changeSong_MusicSelect_Preview(selectedSongIndex);

            selectedSong.GetComponent<SelectedSongScript>().resetSelectionBar_FillAmount();

            populateInfoPanel();
            populateScoreInfo();

            
        }

        //Tutorial song selected, disable normal and hard buttons
        if (selectedSongIndex == 0)
        {
            HighlightThisButton(buttons[0]);
            populateInfoPanel();
            populateScoreInfo();
        }
        else
        {
            normalButton.SetActive(true);
            hardButton.SetActive(true);  
        }

        

    }

    //To lerp infoPanel after selecting a song;
    IEnumerator LerpInfoPanel_Off()
    {
        float startTime = Time.time;
        while (songInfoTextPanel.anchoredPosition != offScreen)
        {
            songInfoTextPanel.anchoredPosition = Vector3.Lerp(onScreen, offScreen, (Time.time - startTime) / 0.15f);
            yield return null;
        }
        StartCoroutine(LerpInfoPanel_On()); //Bring in panel
    }
    IEnumerator LerpInfoPanel_On()
    {
        float startTime = Time.time;
        while (songInfoTextPanel.anchoredPosition != onScreen)
        {
            songInfoTextPanel.anchoredPosition = Vector3.Lerp(offScreen, onScreen, (Time.time - startTime) / 0.1f);
            yield return null;
        }
    }
    /////////////////////////////////////////////
   

    public void HighlightThisButton(GameObject button)
    {
        
        selectedText = button.GetComponentInChildren<Text>();
        selectedButtonPos = button.GetComponent<RectTransform>().position;
        selectedButton.position = selectedButtonPos;
        foreach (GameObject b in buttons)
        {
            if (b.name != button.name)
            {
                b.GetComponentInChildren<Text>().color = unselected;
            }
        }
        selectedText.color = Color.white;

        //Functionality, change difficulty
        if (button.tag == "EasyButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.easy;
            EnemyRelayManager.erm.ChangeDifficulty();
        }
        else if (button.tag == "NormalButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.normal;
            EnemyRelayManager.erm.ChangeDifficulty();
        }
        else if (button.tag == "HardButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.hard;
            EnemyRelayManager.erm.ChangeDifficulty();
        }
        //Update score info based on selection
        populateScoreInfo();
    }

    //Choose difficulty
    public void HighlightThisButton()
    {
        //Aesthetics
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        difficulty_button = selected;
        selectedText = selected.GetComponentInChildren<Text>();
        selectedButtonPos = selected.GetComponent<RectTransform>().position;
        selectedButton.position = selectedButtonPos;
        foreach(GameObject b in buttons)
        {
            if(b.name != selected.name)
            {
                b.GetComponentInChildren<Text>().color = unselected;
            }

        }
        selectedText.color = Color.white;

        //Functionality, change difficulty
        if(selected.tag == "EasyButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.easy;
            EnemyRelayManager.erm.ChangeDifficulty();
        }
        else if (selected.tag == "NormalButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.normal;
            EnemyRelayManager.erm.ChangeDifficulty();
        }
        else if (selected.tag == "HardButton")
        {
            GameManager.gm.difficulty = GameManager.Difficulty.hard;
            EnemyRelayManager.erm.ChangeDifficulty();
        }

        //Update score info based on selection
        populateScoreInfo();
    }

    //Fill info in song select, each panel index in songs corresponds to each index in alubs of AudioPeer
    void populateSongSelect()
    {
        foreach(GameObject songPanel in songs)
        {
            foreach(Transform child in songPanel.transform)
            {
                if(child.name == "SongTitle")
                {
                    child.GetComponent<Text>().text = AudioPeer.ap.album[songs.IndexOf(songPanel)].title;   
                }
                if(child.name == "SongArtist")
                {
                    child.GetComponent<Text>().text = AudioPeer.ap.album[songs.IndexOf(songPanel)].artist;
                }
            }
        }
    }

    void populateInfoPanel()
    {
        foreach(RectTransform item in songInfoTextPanel)
        {
            if(item.name == "SongTitle")
            {
                item.GetComponent<Text>().text = AudioPeer.ap.album[selectedSongIndex].title;
            }
            if(item.name == "SongArtist")
            {
                item.GetComponent<Text>().text = AudioPeer.ap.album[selectedSongIndex].artist;
            }
            if(item.name == "SongBPM")
            {
                item.GetComponent<Text>().text = "BPM: " + AudioPeer.ap.album[selectedSongIndex].bpm.ToString();
            }
            if(item.name == "SongImage")
            {
                if(AudioPeer.ap.album[selectedSongIndex].songArt != null)
                {
                    item.GetComponent<Image>().sprite = AudioPeer.ap.album[selectedSongIndex].songArt;
                }
                else
                {
                    item.GetComponent<Image>().sprite = defaultArt;
                }
                
            }
        }
    }

    void populateScoreInfo()
    {
        foreach (RectTransform item in playerScoreInfoPanel)
        {
            if (item.name == "Highscore")
            {
                if(GameManager.gm.difficulty == GameManager.Difficulty.easy)
                {
                    item.GetComponent<Text>().text = "Highscore: " + AudioPeer.ap.album[selectedSongIndex].easy_HighScore.ToString();
                }
                if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
                {
                    item.GetComponent<Text>().text = "Highscore: " + AudioPeer.ap.album[selectedSongIndex].normal_HighScore.ToString();
                }
                if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
                {
                    item.GetComponent<Text>().text = "Highscore: " + AudioPeer.ap.album[selectedSongIndex].hard_HighScore.ToString();
                }

            }
            if (item.name == "BestCombo")
            {
                if (GameManager.gm.difficulty == GameManager.Difficulty.easy)
                {
                    item.GetComponent<Text>().text = "Best Combo: " + AudioPeer.ap.album[selectedSongIndex].easy_MaxCombo.ToString();
                }
                if (GameManager.gm.difficulty == GameManager.Difficulty.normal)
                {
                    item.GetComponent<Text>().text = "Best Combo: " + AudioPeer.ap.album[selectedSongIndex].normal_MaxCombo.ToString();
                }
                if (GameManager.gm.difficulty == GameManager.Difficulty.hard)
                {
                    item.GetComponent<Text>().text = "Best Combo: " + AudioPeer.ap.album[selectedSongIndex].hard_MaxCombo.ToString();
                }
            }
        }
    }


    //Called by on-screen play button
    public void PlayGame()
    {
        AudioPeer.ap.changeSong_MusicSelectTransition(selectedSongIndex);
        GameManager.gm.currentState = GameManager.GameState.ready;
    }

    //Called by on-screen back button
    public void ReturnToMainMenu()
    {
        FindObjectOfType<SoundEffectManager>().Play("ChangeSong");
        GameManager.gm.currentState = GameManager.GameState.menu;
    }

    //Called when opening music select, centers scrollview on selected item
    public void CenterToItem()
    {
        float index = (float)selectedSongIndex;
        float point = 1f - (index / AudioPeer.ap.album.Count);
        scrollRect.verticalNormalizedPosition = point;

    }

    //Instantiates each song item from AudioPeer.ap.album as buttons in the music select list
    public void PopulateSongButtons()
    {
        
        foreach (Song s in AudioPeer.ap.album)
        {
            GameObject _songPanel = (GameObject)Instantiate(_songPanelPrefab);
            _songPanel.transform.SetParent(scrollRect.content, false);
            _songPanel.GetComponent<Button>().onClick.AddListener(SelectSong);
            songs.Add(_songPanel);
            
        }

        //Populate the individual buttons
        populateSongSelect();
    }

   

    



}
