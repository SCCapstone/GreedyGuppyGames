// <copyright file="MobileParticleSelector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.Effects
{
    /// <summary>
    /// Simple class to switch between two separate game objects depending on whether we're
    /// on a mobile platform or not
    /// </summary>
    public class MobileParticleSelector : MonoBehaviour
    {
        /// <summary>
        /// System to use on non-mobile platforms
        /// </summary>
        public ParticleSystem defaultParticles;
        /// <summary>
        /// System to use on mobile platforms
        /// </summary>
        public ParticleSystem mobileParticles;

        protected virtual void Awake()
        {
            ParticleSystem selectedSystem;

#if UNITY_STANDALONE
            selectedSystem = this.defaultParticles;
#else
			selectedSystem = mobileParticles;
#endif

            this.defaultParticles.gameObject.SetActive(selectedSystem == this.defaultParticles);
            this.mobileParticles.gameObject.SetActive(selectedSystem == this.mobileParticles);
        }
    }
}