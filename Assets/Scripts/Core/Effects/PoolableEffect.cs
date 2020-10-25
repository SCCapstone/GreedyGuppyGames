// <copyright file="PoolableEffect.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Effects
{
    /// <summary>
    /// Simple effect support script to reset trails and particles on enable, and also
    /// stops and starts reused emitters (to prevent them emitting when moving after being repooled)
    /// </summary>
    public class PoolableEffect : Poolable
    {
        protected List<ParticleSystem> m_Systems;
        protected List<TrailRenderer> m_Trails;
        private bool m_EffectsEnabled;

        /// <summary>
        /// Stop emitting all particles
        /// </summary>
        public void StopAll()
        {
            foreach (var particleSystem in this.m_Systems)
            {
                particleSystem.Stop();
            }
        }

        /// <summary>
        /// Turn off all known systems
        /// </summary>
        public void TurnOffAllSystems()
        {
            if (!this.m_EffectsEnabled)
            {
                return;
            }

            // Reset all systems and trails
            foreach (var particleSystem in this.m_Systems)
            {
                particleSystem.Clear();
                var emission = particleSystem.emission;
                emission.enabled = false;
            }

            foreach (var trailRenderer in this.m_Trails)
            {
                trailRenderer.Clear();
                trailRenderer.enabled = false;
            }

            this.m_EffectsEnabled = false;
        }

        /// <summary>
        /// Turn on all known systems
        /// </summary>
        public void TurnOnAllSystems()
        {
            if (this.m_EffectsEnabled)
            {
                return;
            }

            // Re-enable all systems and trails
            foreach (var particleSystem in this.m_Systems)
            {
                particleSystem.Clear();
                var emission = particleSystem.emission;
                emission.enabled = true;
            }

            foreach (var trailRenderer in this.m_Trails)
            {
                trailRenderer.Clear();
                trailRenderer.enabled = true;
            }

            this.m_EffectsEnabled = true;
        }

        protected override void Repool()
        {
            base.Repool();
            this.TurnOffAllSystems();
        }

        protected virtual void Awake()
        {
            this.m_EffectsEnabled = true;

            // Cache systems and trails, but only active and emitting ones
            this.m_Systems = new List<ParticleSystem>();
            this.m_Trails = new List<TrailRenderer>();

            foreach (var system in this.GetComponentsInChildren<ParticleSystem>())
            {
                if (system.emission.enabled && system.gameObject.activeSelf)
                {
                    this.m_Systems.Add(system);
                }
            }

            foreach (var trail in this.GetComponentsInChildren<TrailRenderer>())
            {
                if (trail.enabled && trail.gameObject.activeSelf)
                {
                    this.m_Trails.Add(trail);
                }
            }

            this.TurnOffAllSystems();
        }
    }
}