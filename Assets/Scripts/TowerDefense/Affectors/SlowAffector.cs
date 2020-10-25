// <copyright file="SlowAffector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Health;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// Uses a trigger to attach and remove <see cref="AgentSlower" /> components to agents
    /// </summary>
    public class SlowAffector : PassiveAffector
    {
        /// <summary>
        /// A normalized value to slow agents by
        /// </summary>
        [Range(0, 1)]
        public float slowFactor;

        /// <summary>
        /// The slow factor for displaying to the UI
        /// </summary>
        public string slowFactorFormat = "<b>Slow Factor:</b> {0}";

        /// <summary>
        /// The particle system that plays when an entity enters the sphere
        /// </summary>
        public ParticleSystem enterParticleSystem;

        public GameObject slowFxPrefab;

        /// <summary>
        /// The audio source that plays when an entity enters the sphere
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Subsribes to the relevant targetter events
        /// </summary>
        protected void Awake()
        {
            this.towerTargetter.targetEntersRange += this.OnTargetEntersRange;
            this.towerTargetter.targetExitsRange += this.OnTargetExitsRange;
        }

        /// <summary>
        /// Unsubsribes from the relevant targetter events
        /// </summary>
        private void OnDestroy()
        {
            this.towerTargetter.targetEntersRange -= this.OnTargetEntersRange;
            this.towerTargetter.targetExitsRange -= this.OnTargetExitsRange;
        }

        /// <summary>
        /// Attaches a <see cref="AgentSlower" /> to the agent
        /// </summary>
        /// <param name="target">The agent to attach the slower to</param>
        protected void AttachSlowComponent(Agent target)
        {
            var slower = target.GetComponent<AgentSlower>();
            if (slower == null)
            {
                slower = target.gameObject.AddComponent<AgentSlower>();
            }

            slower.Initialize(this.slowFactor, this.slowFxPrefab, target.appliedEffectOffset, target.appliedEffectScale);

            if (this.enterParticleSystem != null)
            {
                this.enterParticleSystem.Play();
            }

            if (this.audioSource != null)
            {
                this.audioSource.Play();
            }
        }

        /// <summary>
        /// Removes the <see cref="AgentSlower" /> from the agent once it leaves the area
        /// </summary>
        /// <param name="target">The agent to remove the slower from</param>
        protected void RemoveSlowComponent(Agent target)
        {
            if (target == null)
            {
                return;
            }

            var slowComponent = target.gameObject.GetComponent<AgentSlower>();
            if (slowComponent != null)
            {
                slowComponent.RemoveSlow(this.slowFactor);
            }
        }

        /// <summary>
        /// Fired when the targetter aquires a new targetable
        /// </summary>
        protected void OnTargetEntersRange(Targetable other)
        {
            var agent = other as Agent;
            if (agent == null)
            {
                return;
            }

            this.AttachSlowComponent(agent);
        }

        /// <summary>
        /// Fired when the targetter aquires loses a targetable
        /// </summary>
        protected void OnTargetExitsRange(Targetable other)
        {
            var searchable = other as Agent;
            if (searchable == null)
            {
                return;
            }

            this.RemoveSlowComponent(searchable);
        }
    }
}