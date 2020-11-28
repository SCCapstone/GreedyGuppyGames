using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //An array for keep track of audio clips
    public Sounds[] sounds;

    //Reference to the AudioManager itself
    public static AudioManager instance;

    void Awake()
    {
        //If no instance of AudioManager exists yet
        if(instance == null)
        {
            instance = this;
        }
        //If an instace of AudioManager exists destroy it
        else
        {
            Destroy(gameObject);
            return;
        }

        //Allows the AudioManager to transition between scenes
        DontDestroyOnLoad(gameObject);

        //Constructor for internal array for each sound in sounds
        foreach(Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        //Plays game music
        PlayAudio("GameMusic");
    }

    //Plays an audio clip
    public void PlayAudio(string audioName)
    {
        //Searches the array of sounds for the name of the passed in audio clip
        Sounds s = Array.Find(sounds, sound => sound.name == audioName);
        
        //if not found displays a warning
        if(s == null)
        {
            Debug.LogWarning("Sound: " + audioName + " not found");
            return;
        }

        //if found plays the audio
        s.source.Play();
    }
}
