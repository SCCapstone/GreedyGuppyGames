// <copyright file="RandomNodeSelector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Extensions;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace TowerDefense.Nodes
{
    /// <summary>
    /// Randomly selects the next node
    /// </summary>
    public class RandomNodeSelector : NodeSelector
    {
        /// <summary>
        /// The sum of all Node weights in m_LinkedNodes
        /// </summary>
        protected int m_WeightSum;

        /// <summary>
        /// Gets a random node in the list
        /// </summary>
        /// <returns>The randomly selected node</returns>
        public override Node GetNextNode()
        {
            if (this.linkedNodes == null)
            {
                return null;
            }

            int totalWeight = this.m_WeightSum;
            return this.linkedNodes.WeightedSelection(totalWeight, t => t.weight);
        }

        protected void Awake()
        {
            // cache the linked node weights
            this.m_WeightSum = this.TotalLinkedNodeWeights();
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            base.OnDrawGizmos();
        }
#endif
        /// <summary>
        /// Sums up the weights of the linked nodes for random selection
        /// </summary>
        /// <returns>Weight Sum of Linked Nodes</returns>
        protected int TotalLinkedNodeWeights()
        {
            int totalWeight = 0;
            int count = this.linkedNodes.Count;
            for (int i = 0; i < count; i++)
            {
                totalWeight += this.linkedNodes[i].weight;
            }

            return totalWeight;
        }
    }
}