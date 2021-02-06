using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //An array for keep track of audio clips
    public Sounds[] musicSounds;
    public Sounds[] gameSFXSounds;
    public Sounds[] menuSFXSounds;

    public Slider musicSlider;
    public Slider sfxSlider;

    //Reference to the AudioManager itself
    public static AudioManager instance;
    public AudioMixer audioMixer;
    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;
    public string MusicMixerGroupName;
    public string SFXMixerGroupName;
    public string gameMusicName;


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

        /**
         * Constructor for internal array for each sound in sounds
         * adds the audio clip, volume, pitch, loop, and mixer group to each
         * music group depends on if it is a sfx or music for the sliders
         * */
        foreach(Sounds s in musicSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = MusicMixerGroup;
        }

        foreach (Sounds s in gameSFXSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = SFXMixerGroup;
        }

        foreach (Sounds s in menuSFXSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = SFXMixerGroup;
        }
    }

    void Start()
    {
        // Plays game music
        PlayAudio(gameMusicName);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    //Plays an audio clip
    public void PlayAudio(string audioName)
    {
        //Searches the array of sounds for the name of the passed in audio clip
        Sounds s = Array.Find(musicSounds, sound => sound.name == audioName);
        
        //if not found displays a warning
        if(s != null)
        {
            //if found plays the audio
            s.source.Play();
            return;
        }

        s = Array.Find(gameSFXSounds, sound => sound.name == audioName);

        //if not found displays a warning
        if (s != null)
        {
            //if found plays the audio
            s.source.Play();
            return;
        }

        s = Array.Find(menuSFXSounds, sound => sound.name == audioName);

        //if not found displays a warning
        if (s != null)
        {
            //if found plays the audio
            s.source.Play();
            return;
        }
        Debug.Log("Sound not found");
    }

    // sets music volume for slider using logarithmic scale
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MusicMixerGroupName, Mathf.Log10(volume) * 20);
    }

    // sets sfx volume for slider using logarithmic scale
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFXMixerGroupName, Mathf.Log10(volume) * 20);
    }
}
