// <copyright file="TimedWave.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// A wave implementation that triggers the waveCompleted event after an elapsed amount of time
    /// </summary>
    public class TimedWave : Wave
    {
        /// <summary>
        /// The time until the next wave is started
        /// </summary>
        [Tooltip("The time until the next wave is started")]
        public float timeToNextWave = 10f;

        /// <summary>
        /// The timer used to start the next wave
        /// </summary>
        protected Timer m_WaveTimer;

        public override float progress
        {
            get { return this.m_WaveTimer == null ? 0 : this.m_WaveTimer.normalizedProgress; }
        }

        /// <summary>
        /// Initializes the Wave
        /// </summary>
        public override void Init()
        {
            base.Init();

            if (this.spawnInstructions.Count > 0)
            {
                this.m_WaveTimer = new Timer(this.timeToNextWave, this.SafelyBroadcastWaveCompletedEvent);
                this.StartTimer(this.m_WaveTimer);
            }
        }

        /// <summary>
        /// Handles spawning the current agent and sets up the next agent for spawning
        /// </summary>
        protected override void SpawnCurrent()
        {
            this.Spawn();
            if (!this.TrySetupNextSpawn())
            {
                this.StopTimer(this.m_SpawnTimer);
            }
        }
    }
}