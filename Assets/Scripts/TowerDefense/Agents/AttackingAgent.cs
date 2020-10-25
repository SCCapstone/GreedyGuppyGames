// <copyright file="AttackingAgent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Health;
using TowerDefense.Affectors;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Agents
{
    /// <summary>
    /// An implementation of Agent that will attack
    /// any Towers that block its path
    /// </summary>
    public class AttackingAgent : Agent
    {
        /// <summary>
        /// Tower to target
        /// </summary>
        protected Tower m_TargetTower;

        /// <summary>
        /// The attached attack affector
        /// </summary>
        protected AttackAffector m_AttackAffector;

        /// <summary>
        /// Is this agent currently engaging a tower?
        /// </summary>
        protected bool m_IsAttacking;

        public override void Initialize()
        {
            base.Initialize();

            // Attack affector
            this.m_AttackAffector.Initialize(this.configuration.alignmentProvider);

            // We don't want agents to attack towers until their path is blocked,
            // so disable m_AttackAffector until it is needed
            this.m_AttackAffector.enabled = false;
        }

        /// <summary>
        /// Unsubscribes from tracked towers removed event
        /// and disables the attached attack affector
        /// </summary>
        public override void Remove()
        {
            base.Remove();
            if (this.m_TargetTower != null)
            {
                this.m_TargetTower.removed -= this.OnTargetTowerDestroyed;
            }

            this.m_AttackAffector.enabled = false;
            this.m_TargetTower = null;
        }

        /// <summary>
        /// Gets the closest tower to the agent
        /// </summary>
        /// <returns>The closest tower</returns>
        protected Tower GetClosestTower()
        {
            var towerController = this.m_AttackAffector.towerTargetter.GetTarget() as Tower;
            return towerController;
        }

        /// <summary>
        /// Caches the Attack Affector if necessary
        /// </summary>
        protected override void LazyLoad()
        {
            base.LazyLoad();
            if (this.m_AttackAffector == null)
            {
                this.m_AttackAffector = this.GetComponent<AttackAffector>();
            }
        }

        /// <summary>
        /// If the tower is destroyed while other agents attack it, ensure it becomes null
        /// </summary>
        /// <param name="tower">The tower that has been destroyed</param>
        protected virtual void OnTargetTowerDestroyed(DamageableBehaviour tower)
        {
            if (this.m_TargetTower == tower)
            {
                this.m_TargetTower.removed -= this.OnTargetTowerDestroyed;
                this.m_TargetTower = null;
            }
        }

        /// <summary>
        /// Peforms the relevant path update for the states <see cref="Agent.State.OnCompletePath"/>,
        /// <see cref="Agent.State.OnPartialPath"/> and <see cref="Agent.State.Attacking"/>
        /// </summary>
        protected override void PathUpdate()
        {
            switch (this.state)
            {
                case State.OnCompletePath:
                    this.OnCompletePathUpdate();
                    break;
                case State.OnPartialPath:
                    this.OnPartialPathUpdate();
                    break;
                case State.Attacking:
                    this.AttackingUpdate();
                    break;
            }
        }

        /// <summary>
        /// Change to <see cref="Agent.State.OnCompletePath" /> when path is no longer blocked or to
        /// <see cref="Agent.State.Attacking" /> when the agent reaches <see cref="AttackingAgent.m_TargetTower" />
        /// </summary>
        protected override void OnPartialPathUpdate()
        {
            if (!this.isPathBlocked)
            {
                this.state = State.OnCompletePath;
                return;
            }

            // Check for closest tower at the end of the partial path
            this.m_AttackAffector.towerTargetter.transform.position = this.m_NavMeshAgent.pathEndPosition;
            Tower tower = this.GetClosestTower();
            if (tower != this.m_TargetTower)
            {
                // if the current target is to be replaced, unsubscribe from removed event
                if (this.m_TargetTower != null)
                {
                    this.m_TargetTower.removed -= this.OnTargetTowerDestroyed;
                }

                // assign target, can be null
                this.m_TargetTower = tower;

                // if new target found subscribe to removed event
                if (this.m_TargetTower != null)
                {
                    this.m_TargetTower.removed += this.OnTargetTowerDestroyed;
                }
            }

            if (this.m_TargetTower == null)
            {
                return;
            }

            float distanceToTower = Vector3.Distance(this.transform.position, this.m_TargetTower.transform.position);
            if (!(distanceToTower < this.m_AttackAffector.towerTargetter.effectRadius))
            {
                return;
            }

            if (!this.m_AttackAffector.enabled)
            {
                this.m_AttackAffector.towerTargetter.transform.position = this.transform.position;
                this.m_AttackAffector.enabled = true;
            }

            this.state = State.Attacking;
            this.m_NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// The agent attacks until the path is available again or it has killed the target tower
        /// </summary>
        protected void AttackingUpdate()
        {
            if (this.m_TargetTower != null)
            {
                return;
            }

            this.MoveToNode();

            // Resume path once blocking has been cleared
            this.m_IsAttacking = false;
            this.m_NavMeshAgent.isStopped = false;
            this.m_AttackAffector.enabled = false;
            this.state = this.isPathBlocked ? State.OnPartialPath : State.OnCompletePath;
            // Move the Targetter back to the agent's position
            this.m_AttackAffector.towerTargetter.transform.position = this.transform.position;
        }
    }
}