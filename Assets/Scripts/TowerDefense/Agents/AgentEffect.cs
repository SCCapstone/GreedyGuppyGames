// <copyright file="AgentEffect.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.Agents
{
    /// <summary>
    /// A component that will apply various effects on an agent
    /// </summary>
    public abstract class AgentEffect : MonoBehaviour
    {
        /// <summary>
        /// Reference to the agent that will be affected
        /// </summary>
        protected Agent m_Agent;

        public virtual void Awake()
        {
            this.LazyLoad();
        }

        /// <summary>
        /// A lazy way to ensure that <see cref="m_Agent"/> will not be null
        /// </summary>
        public virtual void LazyLoad()
        {
            if (this.m_Agent == null)
            {
                this.m_Agent = this.GetComponent<Agent>();
            }
        }
    }
}