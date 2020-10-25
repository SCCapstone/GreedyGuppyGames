// <copyright file="WeightedAudioList.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Extensions;
using UnityEngine;

namespace ActionGameFramework.Audio
{
    /// <summary>
    /// Weighted audio list
    /// </summary>
    [Serializable]
    public class WeightedAudioList
    {
        /// <summary>
        /// Items with their corresponding weights
        /// </summary>
        public WeightedAudioClip[] weightedItems;

        /// <summary>
        /// The sum of all items weights
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
        /// Gets a random audio clip from the weighted list
        /// </summary>
        /// <returns>The selection.</returns>
        public AudioClip WeightedSelection()
        {
            if (this.weightedItems.Length == 0)
            {
                return null;
            }

            WeightedAudioClip item = this.weightedItems.WeightedSelection(this.weightSum, t => t.weight);
            return item.clip;
        }

        /// <summary>
        /// Calculates the sum of all item weights
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