// <copyright file="PlayerHomeBase.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using ActionGameFramework.Audio;
using Core.Health;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// A class representing the home base that players must defend
    /// </summary>
    public class PlayerHomeBase : DamageableBehaviour
    {
        /// <summary>
        /// The particle system when an attack is charging
        /// </summary>
        public ParticleSystem chargePfx;

        /// <summary>
        /// Sound to play when charge effect starts
        /// </summary>
        public RandomAudioSource chargeSound;

        /// <summary>
        /// The particle system for an attack
        /// </summary>
        public ParticleSystem attackPfx;

        /// <summary>
        /// Sound to play when attack effect starts
        /// </summary>
        public RandomAudioSource attackSound;

        /// <summary>
        /// The current Agents within the home base attack zone
        /// </summary>
        protected List<Agent> m_CurrentAgentsInside = new List<Agent>();

        /// <summary>
        /// Subscribes to damaged event
        /// </summary>
        protected virtual void Start()
        {
            this.configuration.damaged += this.OnDamaged;
        }

        /// <summary>
        /// Unsubscribes to damaged event
        /// </summary>
        protected virtual void OnDestroy()
        {
            this.configuration.damaged -= this.OnDamaged;
        }

        /// <summary>
        /// Plays <see cref="attackPfx"/> if assigned
        /// </summary>
        protected virtual void OnDamaged(HealthChangeInfo obj)
        {
            if (this.attackPfx != null)
            {
                this.attackPfx.Play();
            }

            if (this.attackSound != null)
            {
                this.attackSound.PlayRandomClip();
            }
        }

        /// <summary>
        /// Adds triggered Agent to tracked Agents, subscribes to Agent's
        /// removed event and plays pfx
        /// </summary>
        /// <param name="other">Triggered collider</param>
        private void OnTriggerEnter(Collider other)
        {
            var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
            if (homeBaseAttacker == null)
            {
                return;
            }

            this.m_CurrentAgentsInside.Add(homeBaseAttacker.agent);
            homeBaseAttacker.agent.removed += this.OnAgentRemoved;
            if (this.chargePfx != null)
            {
                this.chargePfx.Play();
            }

            if (this.chargeSound != null)
            {
                this.chargeSound.PlayRandomClip();
            }
        }

        /// <summary>
        /// If the entity that has entered the collider
        /// has an <see cref="Agent"/> component on it
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
            if (homeBaseAttacker == null)
            {
                return;
            }

            this.RemoveTarget(homeBaseAttacker.agent);
        }

        /// <summary>
        /// Removes Agent from tracked <see cref="m_CurrentAgentsInside"/>
        /// </summary>
        private void OnAgentRemoved(DamageableBehaviour targetable)
        {
            targetable.removed -= this.OnAgentRemoved;
            Agent attackingAgent = targetable as Agent;
            this.RemoveTarget(attackingAgent);
        }

        /// <summary>
        /// Removes <paramref name="agent"/> from <see cref="m_CurrentAgentsInside"/> and stops pfx
        /// if there are no more <see cref="Agent"/>s
        /// </summary>
        /// <param name="agent">
        /// The agent to remove
        /// </param>
        private void RemoveTarget(Agent agent)
        {
            if (agent == null)
            {
                return;
            }

            this.m_CurrentAgentsInside.Remove(agent);
            if (this.m_CurrentAgentsInside.Count == 0 && this.chargePfx != null)
            {
                this.chargePfx.Stop();
            }
        }
    }
}