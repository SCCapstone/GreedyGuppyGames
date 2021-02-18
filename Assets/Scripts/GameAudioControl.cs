using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameAudioControl
{
    public static float musicVolume;
    public static float sfxVolume;
    public static AudioManager audioManager;

    public static void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        audioManager.SetMusicVolume(musicVolume);
    }

    public static void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioManager.SetSFXVolume(sfxVolume);
    }
}
