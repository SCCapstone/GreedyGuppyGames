// <copyright file="WaveManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Core.Extensions;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// WaveManager - handles wave initialisation and completion
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        /// <summary>
        /// Current wave being used
        /// </summary>
        protected int m_CurrentIndex;

        /// <summary>
        /// Whether the WaveManager starts waves on Awake - defaulted to null since the LevelManager should call this function
        /// </summary>
        public bool startWavesOnAwake;

        /// <summary>
        /// The waves to run in order
        /// </summary>
        [Tooltip("Specify this list in order")]
        public List<Wave> waves = new List<Wave>();

        /// <summary>
        /// The current wave number
        /// </summary>
        public int waveNumber
        {
            get { return this.m_CurrentIndex + 1; }
        }

        /// <summary>
        /// The total number of waves
        /// </summary>
        public int totalWaves
        {
            get { return this.waves.Count; }
        }

        public float waveProgress
        {
            get
            {
                if (this.waves == null || this.waves.Count <= this.m_CurrentIndex)
                {
                    return 0;
                }

                return this.waves[this.m_CurrentIndex].progress;
            }
        }

        /// <summary>
        /// Called when a wave begins
        /// </summary>
        public event Action waveChanged;

        /// <summary>
        /// Called when all waves are finished
        /// </summary>
        public event Action spawningCompleted;

        /// <summary>
        /// Starts the waves
        /// </summary>
        public virtual void StartWaves()
        {
            if (this.waves.Count > 0)
            {
                this.InitCurrentWave();
            }
            else
            {
                Debug.LogWarning("[LEVEL] No Waves on wave manager. Calling spawningCompleted");
                this.SafelyCallSpawningCompleted();
            }
        }

        /// <summary>
        /// Inits the first wave
        /// </summary>
        protected virtual void Awake()
        {
            if (this.startWavesOnAwake)
            {
                this.StartWaves();
            }
        }

        /// <summary>
        /// Sets up the next wave
        /// </summary>
        protected virtual void NextWave()
        {
            this.waves[this.m_CurrentIndex].waveCompleted -= this.NextWave;
            if (this.waves.Next(ref this.m_CurrentIndex))
            {
                this.InitCurrentWave();
            }
            else
            {
                this.SafelyCallSpawningCompleted();
            }
        }

        /// <summary>
        /// Initialize the current wave
        /// </summary>
        protected virtual void InitCurrentWave()
        {
            Wave wave = this.waves[this.m_CurrentIndex];
            wave.waveCompleted += this.NextWave;
            wave.Init();
            if (this.waveChanged != null)
            {
                this.waveChanged();
            }
        }

        /// <summary>
        /// Calls spawningCompleted event
        /// </summary>
        protected virtual void SafelyCallSpawningCompleted()
        {
            if (this.spawningCompleted != null)
            {
                this.spawningCompleted();
            }
        }
    }
}