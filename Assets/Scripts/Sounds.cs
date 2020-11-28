using UnityEngine;
using UnityEngine.Audio;

//Allows other scripts to reference it
[System.Serializable]

public class Sounds
{
    [Header("Attributes")]
    public string name;
    public AudioClip audioClip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
