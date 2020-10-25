// <copyright file="WeightedObjectList.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Extensions;
using UnityEngine;

namespace ActionGameFramework.Spawning
{
    /// <summary>
    /// List of weighted objects
    /// </summary>
    [Serializable]
    public class WeightedObjectList
    {
        /// <summary>
        /// The weighted items
        /// </summary>
        public WeightedObject[] weightedItems;

        /// <summary>
        /// Sum of the item weights
        /// </summary>
        protected int m_WeightSum = -1;

        /// <summary>
        /// Gets the weight sum.
        /// </summary>
        /// <value>The weight sum.</value>
        public int weightSum
        {
            get
            {
                if (this.m_WeightSum < 0)
                {
                    this.CalculateWeightSum();
                }

                return this.m_WeightSum;
            }
        }

        /// <summary>
        /// Returns a random game object based on weight
        /// </summary>
        /// <returns>The selection.</returns>
        public GameObject WeightedSelection()
        {
            if (this.weightedItems.Length == 0)
            {
                return null;
            }

            WeightedObject item = this.weightedItems.WeightedSelection(this.weightSum, t => t.weight);
            return item.gameObject;
        }

        /// <summary>
        /// Calculates the weight sum.
        /// </summary>
        protected void CalculateWeightSum()
        {
            this.m_WeightSum = 0;
            int count = this.weightedItems.Length;
            for (int i = 0; i < count; i++)
            {
                this.m_WeightSum += this.weightedItems[i].weight;
            }
        }
    }
}