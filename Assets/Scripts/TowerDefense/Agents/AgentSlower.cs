// <copyright file="AgentSlower.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Core.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Agents
{
    /// <summary>
    /// This effect will get attached to an agent that is within range of the SlowAffector radius
    /// </summary>
    public class AgentSlower : AgentEffect
    {
        protected GameObject m_SlowFx;

        protected List<float> m_CurrentEffects = new List<float>();

        /// <summary>
        /// Initializes the slower with the parameters configured in the SlowAffector
        /// </summary>
        /// <param name="slowFactor">Normalized float that represents the % slowdown applied to the agent</param>
        /// <param name="slowfxPrefab">The instantiated object to visualize the slow effect</param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public void Initialize(float slowFactor, GameObject slowfxPrefab = null,
                               Vector3 position = default(Vector3),
                               float scale = 1)
        {
            this.LazyLoad();
            this.m_CurrentEffects.Add(slowFactor);

            // find greatest slow effect
            float min = slowFactor;
            foreach (float item in this.m_CurrentEffects)
            {
                min = Mathf.Min(min, item);
            }

            float originalSpeed = this.m_Agent.originalMovementSpeed;
            float newSpeed = originalSpeed * min;
            this.m_Agent.navMeshNavMeshAgent.speed = newSpeed;

            if (this.m_SlowFx == null && slowfxPrefab != null)
            {
                this.m_SlowFx = Poolable.TryGetPoolable(slowfxPrefab);
                this.m_SlowFx.transform.parent = this.transform;
                this.m_SlowFx.transform.localPosition = position;
                this.m_SlowFx.transform.localScale *= scale;
            }

            this.m_Agent.removed += this.OnRemoved;
        }

        /// <summary>
        /// Resets the agent's speed
        /// </summary>
        public void RemoveSlow(float slowFactor)
        {
            this.m_Agent.removed -= this.OnRemoved;

            this.m_CurrentEffects.Remove(slowFactor);
            if (this.m_CurrentEffects.Count != 0)
            {
                return;
            }

            // No more slow effects
            this.ResetAgent();
        }

        /// <summary>
        /// Agent has died, remove affect
        /// </summary>
        private void OnRemoved(DamageableBehaviour targetable)
        {
            this.m_Agent.removed -= this.OnRemoved;
            this.ResetAgent();
        }

        private void ResetAgent()
        {
            if (this.m_Agent != null)
            {
                this.m_Agent.navMeshNavMeshAgent.speed = this.m_Agent.originalMovementSpeed;
            }

            if (this.m_SlowFx != null)
            {
                Poolable.TryPool(this.m_SlowFx);
                this.m_SlowFx.transform.localScale = Vector3.one;
                this.m_SlowFx = null;
            }

            Destroy(this);
        }
    }
}