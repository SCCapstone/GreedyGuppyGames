// <copyright file="Sounds.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.Audio;

//Allows other scripts to reference it
[System.Serializable]

// Attributes for every sound in the game
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
