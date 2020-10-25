// <copyright file="IntervalParticleSystemPlayer.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace TowerDefense.UI
{
    /// <summary>
    /// A simple component that plays a given particle system on a given regular interval
    /// </summary>
    public class IntervalParticleSystemPlayer : MonoBehaviour
    {
        public ParticleSystem particleSystemToPlay;

        public float interval;

        protected DateTime m_NextPlayTime;

        private void Start()
        {
            this.m_NextPlayTime = DateTime.Now.AddSeconds(this.interval);
        }

        private void Update()
        {
            if (this.particleSystemToPlay != null && this.m_NextPlayTime <= DateTime.Now)
            {
                this.particleSystemToPlay.Play();
                this.m_NextPlayTime = DateTime.Now.AddSeconds(this.interval);
            }
        }
    }
}
