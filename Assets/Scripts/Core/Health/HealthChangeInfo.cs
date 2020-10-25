// <copyright file="HealthChangeInfo.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Health change info - stores information about the health change
    /// </summary>
    public struct HealthChangeInfo
    {
        public Damageable damageable;

        public float oldHealth;

        public float newHealth;

        public IAlignmentProvider damageAlignment;

        public float healthDifference
        {
            get { return this.newHealth - this.oldHealth; }
        }

        public float absHealthDifference
        {
            get { return Mathf.Abs(this.healthDifference); }
        }
    }
}