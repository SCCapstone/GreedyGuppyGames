// <copyright file="FlyingAgent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense.Agents
{
    /// <summary>
    /// Agent that can pass "over" towers that block the path
    /// </summary>
    public class FlyingAgent : Agent
    {
        /// <summary>
        /// Time to wait to clear the navmesh obstacles
        /// </summary>
        protected float m_WaitTime = 0.5f;

        /// <summary>
        /// The current time to wait until we can resume agent movement as normal
        /// </summary>
        protected float m_CurrentWaitTime;

        /// <summary>
        /// If flying agents are blocked, they should still move through obstacles
        /// </summary>
        protected override void OnPartialPathUpdate()
        {
            if (!this.isPathBlocked)
            {
                this.state = State.OnCompletePath;
                return;
            }

            if (!this.isAtDestination)
            {
                return;
            }

            this.m_NavMeshAgent.enabled = false;
            this.m_CurrentWaitTime = this.m_WaitTime;
            this.state = State.PushingThrough;
        }

        /// <summary>
        /// Controls behaviour based on the states <see cref="Agent.State.OnCompletePath"/>, <see cref="Agent.State.OnPartialPath"/>
        /// and <see cref="Agent.State.PushingThrough"/>
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
                case State.PushingThrough:
                    this.PushingThrough();
                    break;
            }
        }

        /// <summary>
        /// When flying agents are pushing through, give them a small amount of time to clear the gap and turn on their agent
        /// once time elapses
        /// </summary>
        protected void PushingThrough()
        {
            this.m_CurrentWaitTime -= Time.deltaTime;

            // Move the agent, overriding its NavMeshAgent until it reaches its destination
            this.transform.LookAt(this.m_Destination, Vector3.up);
            this.transform.position += this.transform.forward * this.m_NavMeshAgent.speed * Time.deltaTime;
            if (this.m_CurrentWaitTime > 0)
            {
                return;
            }

            // Check if there is a navmesh under the agent, if not, then reset the timer
            NavMeshHit hit;
            if (!NavMesh.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, this.navMeshMask))
            {
                this.m_CurrentWaitTime = this.m_WaitTime;
            }
            else
            {
                // If the time elapses, and there is a NavMesh under it, resume agent movement as normal
                this.m_NavMeshAgent.enabled = true;
                this.NavigateTo(this.m_Destination);
                this.state = this.isPathBlocked ? State.OnPartialPath : State.OnCompletePath;
            }
        }
    }
}