using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//song class for each song file
[System.Serializable]
public class Song
{
    public string title;
    public string artist;
    public float bpm;
    public AudioClip clip;
    public Sprite songArt; //image art for song

    //Player Related info;
    //easy
    public int easy_MaxCombo;
    public int easy_HighScore;

    //normal
    public int normal_MaxCombo;
    public int normal_HighScore;

    //hard
    //Player Related info;
    public int hard_MaxCombo;
    public int hard_HighScore;

}


[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{

    public static AudioPeer ap;
    public static AudioSource _audioSource;
    public static float[] _samples = new float[512];
    public static float[] _bufferSamples = new float[512];
    float[] _sampleBufferDecrease = new float[512];

    //All songs
    public List<Song> album;
    //public AudioClip[] album; //0 element is main menu song
    //public int[] bpms; //0 element is main menu song, array of bpms that correspond to albums array indexes
    public int selectClip_index; //index for album, user selected 1 by default

    public static float[] _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8]; //decrease radical changes in the _freqBands
    float[] _bufferDecrease = new float[8];


    //detecting range for usable values
    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    //average of the bands
    public static float _Amplitude, _AmplitudeBuffer;
    float _AmplitudeHighest;

    public static float timePlaying;

    /// <summary>
    /// These bools are to make sure events only happen once when a beat is reached
    /// </summary>
    public bool sameBeat; //Beat is on the same beat
    public bool changed; //Beat has changed
    public int prevBeat; //Prev beat value # before beats changed
    public bool chain; //Chain of enemies before player pattern
    public int beats, beatsInSong = 1; //Number of beats in the song
    public bool occupied; //For update coroutine calls
    public float lastTime, deltaTime, timer; //Increments a timer based on current song playtime
    public float bpm; //BPM set in inspect, must calculate this by hand/ear

    public int beatsInPhase, beatsInRecallPhase;
    public bool counting = false;

    //Fading in bools
    private static bool keepFadingIn;
    private static bool keepFadingOut;

    //song ended flag
    [SerializeField]
    private bool donePlayingSong = false;

    //Testing
    public float clipTime;
    public float clipLength;


    //Tutorial song is playing; first index of album[] is the tutorial song
    public bool tutorial_Playing = false;


    // Use this for initialization
    void Awake()
    {

        ap = this;
        _audioSource = GetComponent<AudioSource>();
        timePlaying = _audioSource.time;

        //Select random song, not tutorial 0 index
        selectClip_index = Random.Range(1, album.Count);
        _audioSource.clip = album[selectClip_index].clip;
        bpm = album[selectClip_index].bpm; //menu song

        _audioSource.Play(); //plays menu song
        
    }

    private void Start()
    {
        //album = album.OrderBy(x => x.artist).ToList();
   
    }
 


    // Update is called once per frame
    void Update()
    {
        
        //Tutorial song is playing, first index of album[]
        if(selectClip_index == 0)
        {
            tutorial_Playing = true;
           // _audioSource.pitch = 1f;
        }
        else
        {
            tutorial_Playing = false;
          //  _audioSource.pitch = 1f;

        }
        

        if (GameManager.gm.currentState == GameManager.GameState.menu || GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            bpm = album[selectClip_index].bpm; //menu
        }
        else
        {
            bpm = album[selectClip_index].bpm; //move later
        }

        if (_audioSource.isPlaying)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            Samples_BandBuffer();
            BandBuffer();
            CreateAudioBands();
            GetAmplitude();
            timePlaying = _audioSource.time;
            //Debug.Log(timePlaying);  
        }

        beat();//BPM basis for beat mechanics

        

        //loop songs on menu/music select screen
        if ((GameManager.gm.currentState == GameManager.GameState.menu || GameManager.gm.currentState == GameManager.GameState.musicSelect || GameManager.gm.currentState == GameManager.GameState.credits) && songEnded())
        {
            if (!_audioSource.isPlaying)
            {
                if (!occupied)
                {
                    StartCoroutine(replaySong());
                }
            }

        }

        //Check for end of song
        if(_audioSource.time >= (_audioSource.clip.length - 1f)) //-1 for buffer, song clip not always accurate to length
        {
            donePlayingSong = true;
        }

        clipTime = _audioSource.time;
        clipLength = _audioSource.clip.length;


    }

    
    public void QuitPhase_StopMusic()
    {
        _audioSource.Stop();
        timePlaying = _audioSource.time;
        timer = 0f;
        lastTime = 0f;
        deltaTime = 0f;
        beats = 0;
        beatsInSong = 0;
        beatsInPhase = 0;
        prevBeat = 0;
        sameBeat = changed = false;
    }
    

    //Song almost over boolean function
    public bool nearingEndOfSong()
    {
        return ((_audioSource.clip.length - _audioSource.time) <= 10f);
    }

    //Based of song playtime, true if reached end of song or player quit
    public bool songEnded()
    {
        return donePlayingSong;
    }


    //Called in game manager when transitioning into waitingState from readyState (beginning of gameplay states)
    public void startPlayingMusic(float waitTime)
    {
        if (!_audioSource.isPlaying)
        {
            if (!occupied)
            {
                StartCoroutine(startPlayMusic(waitTime));
            }
        }
    }

    
    //Called by coroutines below, resets all necessary audio variables
    public void resetAudioPeerFlags()
    {
        donePlayingSong = false;
        timePlaying = _audioSource.time;
        timer = 0f;
        lastTime = 0f;
        deltaTime = 0f;
        beats = 0;
        beatsInSong = 0;
        beatsInPhase = 0;
        prevBeat = 0;
        sameBeat = changed = false;
    }
    

    //Start song from music select
    IEnumerator startPlayMusic(float waitTime)
    {
        occupied = true;
        resetAudioPeerFlags();
        _audioSource.Play();
        //print("stats " + deltaTime + " " + lastTime + " " + timer);
        yield return new WaitForSeconds(waitTime);
        occupied = false;
    }

    //Replay song
    IEnumerator replaySong()
    {
        occupied = true;
        yield return new WaitForSeconds(0f);
        resetAudioPeerFlags();
        _audioSource.Play();
        //print("stats " + deltaTime + " " + lastTime + " " + timer);
        occupied = false;
    }


    //Call for musicSelect play buttons, change song to selected clip when tapping play
    public void changeSong_MusicSelectTransition(int selectSong)
    {
        if (_audioSource.isPlaying && GameManager.gm.currentState == GameManager.GameState.menu)
        {
            selectClip_index = selectSong;
            _audioSource.Stop();
            _audioSource.clip = album[selectSong].clip;
            bpm = album[selectSong].bpm;
            _audioSource.time = 0f;
            timer = 0f;

        }

        if (_audioSource.isPlaying && GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            selectClip_index = selectSong;
            _audioSource.Stop();
            _audioSource.clip = album[selectSong].clip;
            bpm = album[selectSong].bpm;
            _audioSource.time = 0f;
            timer = 0f;

        }

        if (!_audioSource.isPlaying && GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            selectClip_index = selectSong;
            _audioSource.Stop();
            _audioSource.clip = album[selectSong].clip;
            bpm = album[selectSong].bpm;
            _audioSource.time = 0f;
            timer = 0f;

        }
    }

    //music select preview songs
    public void changeSong_MusicSelect_Preview(int selectSong)
    {
        if (_audioSource.isPlaying && GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            selectClip_index = selectSong;
            // int r = Random.Range(0, album.Length);
            _audioSource.Stop();
            // selectClip_index = r;
            _audioSource.clip = album[selectSong].clip;

            //Set song properties for beat
            bpm = album[selectSong].bpm;
            _audioSource.time = 20;
            timePlaying = 20;
            lastTime = 20;
            deltaTime = 0;
            _audioSource.Play();

            FadeInCaller(1, 0.1f, 1f);
          


        }

        if (!_audioSource.isPlaying && GameManager.gm.currentState == GameManager.GameState.musicSelect)
        {
            selectClip_index = selectSong;
            // int r = Random.Range(0, album.Length);
            _audioSource.Stop();
            // selectClip_index = r;
            _audioSource.clip = album[selectSong].clip;

            //Set song properties for beat
            bpm = album[selectSong].bpm;
            _audioSource.time = 20;
            timePlaying = 20;
            lastTime = 20;
            deltaTime = 0;
            _audioSource.Play();

            FadeInCaller(1, 0.1f, 1f);
         

        }
    }

    //Increments the beat value based off bpm and song time
    void beat()
    {

        deltaTime = AudioPeer.timePlaying - lastTime;
        timer += deltaTime;



        if (timer >= (60f / bpm))
        {


            beats++;
            beatsInSong++;

            if (EnemyRelayManager.erm.counting)
            {
                beatsInPhase++;
            }

            if (GameManager.gm.currentState == GameManager.GameState.memoryRecall)
            {
                beatsInRecallPhase++;
            }

            timer -= (60f / bpm);

        }

        //Ever beat is every 4 beats, every 4 beats an action occurs
        //when a 4th beat is reached, and it has not been previously changed, set the beat
        if (beats % 4 == 0 && !changed)
        {
            sameBeat = !sameBeat; //toggle beat on off
            prevBeat = beats; //get number of song beats at the moment the beat is changed
            changed = true; //beat has been changed at this beat
        }

        //if the prevBeat is not the same as the current beat 
        if (prevBeat != beats)
        {
            changed = false; //Change to false, we can now toggle sameBeat once a new 4 beat count is reached
        }

        //Reset beats in phase for memoryRelay
        if (GameManager.gm.currentState == GameManager.GameState.waiting)
        {
            beatsInPhase = 0;
        }

        //Reset beats in phase for the player's memoryRecall phase
        if (GameManager.gm.currentState == GameManager.GameState.memoryRelay)
        {
            beatsInRecallPhase = 0;
        }

        counting = EnemyRelayManager.erm.counting;
        lastTime = AudioPeer.timePlaying;
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (_CurrentAmplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAmplitude;
        }

        _Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }

            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void Samples_BandBuffer()
    {
        for (int g = 0; g < 512; g++)
        {
            if (_samples[g] > _bufferSamples[g])
            {
                _bufferSamples[g] = _samples[g];
                _sampleBufferDecrease[g] = 0.0005f;
            }

            if (_samples[g] < _bufferSamples[g])
            {
                _bufferSamples[g] -= _sampleBufferDecrease[g];
                _sampleBufferDecrease[g] *= 1.2f;


            }
        }
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }

            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;


            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 22050/512 = 43 hertz per sample
         * 
         * 20 - 60hertz
         * 60 -  250 hertz
         * 500 - 2000 hertz
         * 2000 - 4000 hertz
         * 4000 - 6000 hertz
         * 6000 - 20000 hertz
         * 
         * 0 - 2 = 86 hertz
         * 1 - 4 = 172 hertz - 87-258
         * 2 - 8 = 344 hertz - 259-602
         * 3 - 16 = 688 hertz - 603-1290
         * 4 - 32 = 1376 hertz - 1291-2666
         * 5 - 64 = 2752 hertz - 2667-5418
         * 6 - 128 = 5504 hertz - 5419-10922
         * 7 - 256 = 11008 hertz - 10923 -21930
         * 510
         * */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }
    }

    //For PlayerVisualizer pulse
    //Getter for timer float on beats;
    public float getTimer()
    {
        return timer;
    }

    //Callers for this static script //Max volume is 1f
    public static void FadeInCaller(int track, float speed, float maxVolume)
    {
        ap.StartCoroutine(FadeIn(track, speed, maxVolume));
    }
    public static void FadeOutCaller(int track, float speed)
    {
        ap.StartCoroutine(FadeOut(track, speed));
    }
    //Fade coroutines
    static IEnumerator FadeIn(int track, float speed, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        _audioSource.volume = 0;
        float audioVolume = _audioSource.volume;

        while(_audioSource.volume < maxVolume && keepFadingIn)
        {
            audioVolume += speed;
            _audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }

    }

    static IEnumerator FadeOut(int track, float speed)
    {
        keepFadingIn = false;
        keepFadingOut = true;
        float audioVolume = _audioSource.volume;

        while (_audioSource.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            _audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }


    //
    public void Distort(float time)
    {
        StartCoroutine(changePitch(time));
    }
    //When enemies fire
    IEnumerator changePitch(float time)
    {
        _audioSource.pitch = 0.5f;
        yield return new WaitForSeconds(time);
        _audioSource.pitch = 1f;
    }
}


