using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


//Class to define each sound effect
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
    //Check if beat has changed, do not play sounds more than once on the same beat
    public int beat = 0;

}

public class SoundEffectManager : MonoBehaviour {

    public Sound[] sounds;

    //Check if beat has changed, do not play sounds more than once on the same beat
    //private bool currentBeatStatus = false;

	// Use this for initialization before start()
	void Awake () {
        //init each sound in sounds[]
		foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
	}

    public void Play(string name)
    {
        //find sound where sound.name == name
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.beat != AudioPeer.ap.beats)
        {
            s.beat = AudioPeer.ap.beats;
            s.source.Play();
        }

           
    }
        
    
}
