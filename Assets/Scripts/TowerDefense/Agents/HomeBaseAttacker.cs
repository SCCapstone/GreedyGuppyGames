// <copyright file="HomeBaseAttacker.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Health;
using Core.Health;
using Core.Utilities;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Agents
{
    /// <summary>
    /// A component that attacks a home base when an agent reaches it
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class HomeBaseAttacker : MonoBehaviour
    {
        /// <summary>
        /// How long the agent charges for before it attacks
        /// the home base
        /// </summary>
        public float homeBaseAttackChargeTime = 0.5f;

        /// <summary>
        /// Timer used to stall attack to the home base
        /// </summary>
        protected Timer m_HomeBaseAttackTimer;

        /// <summary>
        /// If the agent has reached the Player Home Base and is charging an attack
        /// </summary>
        protected bool m_IsChargingHomeBaseAttack;

        /// <summary>
        /// The DamageableBehaviour on the home base
        /// </summary>
        protected DamageableBehaviour m_FinalDestinationDamageableBehaviour;

        /// <summary>
        /// The agent component attached to this gameObject
        /// </summary>
        public Agent agent { get; protected set; }

        /// <summary>
        /// Fired on completion of <see cref="m_HomeBaseAttackTimer"/>
        /// Applies damage to the homebase
        /// </summary>
        protected void AttackHomeBase()
        {
            this.m_IsChargingHomeBaseAttack = false;
            var damager = this.GetComponent<Damager>();
            if (damager != null)
            {
                this.m_FinalDestinationDamageableBehaviour.TakeDamage(damager.damage, this.transform.position, this.agent.configuration.alignmentProvider);
            }

            this.agent.Remove();
        }

        /// <summary>
        /// Ticks the attack timer
        /// </summary>
        protected virtual void Update()
        {
            // Update HomeBaseAttack Timer
            if (this.m_IsChargingHomeBaseAttack)
            {
                this.m_HomeBaseAttackTimer.Tick(Time.deltaTime);
            }
        }

        /// <summary>
        /// Caches the attached Agent and subscribes to the destinationReached event
        /// </summary>
        protected virtual void Awake()
        {
            this.agent = this.GetComponent<Agent>();
            this.agent.destinationReached += this.OnDestinationReached;
            this.agent.died += this.OnDied;
        }

        /// <summary>
        /// Unsubscribes from the destinationReached event
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (this.agent != null)
            {
                this.agent.destinationReached -= this.OnDestinationReached;
                this.agent.died -= this.OnDied;
            }
        }

        /// <summary>
        /// Stops the attack on the home base
        /// </summary>
        private void OnDied(DamageableBehaviour damageableBehaviour)
        {
            this.m_IsChargingHomeBaseAttack = false;
        }

        /// <summary>
        /// Fired then the agent reached its final node,
        /// Starts the attack timer
        /// </summary>
        /// <param name="homeBase"></param>
        private void OnDestinationReached(Node homeBase)
        {
            this.m_FinalDestinationDamageableBehaviour = homeBase.GetComponent<DamageableBehaviour>();
            // start timer
            if (this.m_HomeBaseAttackTimer == null)
            {
                this.m_HomeBaseAttackTimer = new Timer(this.homeBaseAttackChargeTime, this.AttackHomeBase);
            }
            else
            {
                this.m_HomeBaseAttackTimer.Reset();
            }

            this.m_IsChargingHomeBaseAttack = true;
        }
    }
}
