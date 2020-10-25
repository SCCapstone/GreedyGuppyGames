// <copyright file="TimedLevelIntro.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// Basic implementation of intro: a delay
    /// </summary>
    public class TimedLevelIntro : LevelIntro
    {
        /// <summary>
        /// The delay
        /// </summary>
        public float time = 5f;

        /// <summary>
        /// Timer object used to track the delayed
        /// </summary>
        protected Timer m_Timer;

        /// <summary>
        /// Set up the timer and make it fire the SafelyCallIntroCompleted event
        /// </summary>
        protected void Awake()
        {
            this.m_Timer = new Timer(this.time, this.SafelyCallIntroCompleted);
        }

        /// <summary>
        /// Tick the timer and disable it on completion
        /// </summary>
        protected void Update()
        {
            if (this.m_Timer != null)
            {
                if (this.m_Timer.Tick(Time.deltaTime))
                {
                    this.m_Timer = null;
                }
            }
        }
    }
}