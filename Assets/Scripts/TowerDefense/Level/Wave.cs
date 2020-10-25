// <copyright file="Wave.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Utilities;
using TowerDefense.Agents;
using TowerDefense.Agents.Data;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// A Wave is a TimedBehaviour, that uses the RepeatingTimer to spawn enemies
    /// </summary>
    public class Wave : TimedBehaviour
    {
        /// <summary>
        /// A list of instructions on how to spawn enemies
        /// </summary>
        public List<SpawnInstruction> spawnInstructions;

        /// <summary>
        /// The index of the current enemy to spawn
        /// </summary>
        protected int m_CurrentIndex;

        /// <summary>
        /// The RepeatingTimer used to spawn enemies
        /// </summary>
        protected RepeatingTimer m_SpawnTimer;

        /// <summary>
        /// The event that is fired when a Wave is completed
        /// </summary>
        public event Action waveCompleted;

        public virtual float progress
        {
            get { return (float)(this.m_CurrentIndex) / this.spawnInstructions.Count; }
        }

        /// <summary>
        /// Initializes the Wave
        /// </summary>
        public virtual void Init()
        {
            // If the wave is empty then warn the level designer and fire complete event
            if (this.spawnInstructions.Count == 0)
            {
                Debug.LogWarning("[LEVEL] Empty Wave");
                this.SafelyBroadcastWaveCompletedEvent();
                return;
            }

            this.m_SpawnTimer = new RepeatingTimer(this.spawnInstructions[0].delayToSpawn, this.SpawnCurrent);
            this.StartTimer(this.m_SpawnTimer);
        }

        /// <summary>
        /// Handles spawning the current agent and sets up the next agent for spawning
        /// </summary>
        protected virtual void SpawnCurrent()
        {
            this.Spawn();
            if (!this.TrySetupNextSpawn())
            {
                this.SafelyBroadcastWaveCompletedEvent();
                // this is required so wave progress is still accurate
                this.m_CurrentIndex = this.spawnInstructions.Count;
                this.StopTimer(this.m_SpawnTimer);
            }
        }

        /// <summary>
        /// Spawns the current agent
        /// </summary>
        protected void Spawn()
        {
            SpawnInstruction spawnInstruction = this.spawnInstructions[this.m_CurrentIndex];
            this.SpawnAgent(spawnInstruction.agentConfiguration, spawnInstruction.startingNode);
        }

        /// <summary>
        /// Tries to setup the next spawn
        /// </summary>
        /// <returns>true if there is another spawn instruction, false if not</returns>
        protected bool TrySetupNextSpawn()
        {
            bool hasNext = this.spawnInstructions.Next(ref this.m_CurrentIndex);
            if (hasNext)
            {
                SpawnInstruction nextSpawnInstruction = this.spawnInstructions[this.m_CurrentIndex];
                if (nextSpawnInstruction.delayToSpawn <= 0f)
                {
                    this.SpawnCurrent();
                }
                else
                {
                    this.m_SpawnTimer.SetTime(nextSpawnInstruction.delayToSpawn);
                }
            }

            return hasNext;
        }

        /// <summary>
        /// Spawns the agent
        /// </summary>
        /// <param name="agentConfig">The agent to spawn</param>
        /// <param name="node">The starting node that the agent uses</param>
        protected virtual void SpawnAgent(AgentConfiguration agentConfig, Node node)
        {
            Vector3 spawnPosition = node.GetRandomPointInNodeArea();

            var poolable = Poolable.TryGetPoolable<Poolable>(agentConfig.agentPrefab.gameObject);
            if (poolable == null)
            {
                return;
            }

            var agentInstance = poolable.GetComponent<Agent>();
            agentInstance.transform.position = spawnPosition;
            agentInstance.Initialize();
            agentInstance.SetNode(node);
            agentInstance.transform.rotation = node.transform.rotation;
        }

        /// <summary>
        /// Launch the waveCompleted event
        /// </summary>
        protected void SafelyBroadcastWaveCompletedEvent()
        {
            if (this.waveCompleted != null)
            {
                this.waveCompleted();
            }
        }
    }
}