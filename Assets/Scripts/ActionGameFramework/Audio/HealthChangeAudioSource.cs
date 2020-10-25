// <copyright file="HealthChangeAudioSource.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Audio
{
    /// <summary>
    /// Health change audio source - a helper for playing sounds on Health Change
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class HealthChangeAudioSource : MonoBehaviour
    {
        /// <summary>
        /// The sound selector. A mechanism of specifying how sounds are selected based on HealthChangeInfo
        /// </summary>
        public HealthChangeSoundSelector soundSelector;

        /// <summary>
        /// The audio source
        /// </summary>
        protected AudioSource m_Source;

        /// <summary>
        /// Assign the required AudioSource reference at runtime
        /// </summary>
        protected virtual void Awake()
        {
            this.m_Source = this.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Play the AudioSource
        /// </summary>
        public virtual void PlaySound()
        {
            this.m_Source.Play();
        }

        /// <summary>
        /// Play a clip when certain health change requirements are met
        /// </summary>
        /// <param name="info">Uses <see cref="HealthChangeInfo"/> to determine what clip to play</param>
        public virtual void PlayHealthChangeSound(HealthChangeInfo info)
        {
            if (this.soundSelector != null && this.soundSelector.isSetUp)
            {
                AudioClip newClip = this.soundSelector.GetClipFromHealthChangeInfo(info);
                if (newClip != null)
                {
                    this.m_Source.clip = newClip;
                }
            }

            this.m_Source.Play();
        }

        /// <summary>
        /// Sorts the <see cref="soundSelector"/> sound list
        /// </summary>
        public void Sort()
        {
            if (this.soundSelector.healthChangeSounds == null || this.soundSelector.healthChangeSounds.Count <= 0)
            {
                return;
            }

            this.soundSelector.healthChangeSounds.Sort(new HealthChangeSoundComparer());
        }
    }

    /// <summary>
    /// Provides a way to compare 2 <see cref="HealthChangeSound"/>s
    /// </summary>
    public class HealthChangeSoundComparer : IComparer<HealthChangeSound>
    {
        /// <summary>
        /// Compares 2 <see cref="HealthChangeSound"/>
        /// </summary>
        public int Compare(HealthChangeSound first, HealthChangeSound second)
        {
            if (first.healthChange == second.healthChange)
            {
                return 0;
            }

            if (first.healthChange < second.healthChange)
            {
                return -1;
            }

            return 1;
        }
    }
}