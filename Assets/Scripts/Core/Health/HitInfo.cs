// <copyright file="HitInfo.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Damage info - a class required by some damage listeners
    /// </summary>
    public struct HitInfo
    {
        private readonly HealthChangeInfo m_HealthChangeInfo;
        private readonly Vector3 m_DamagePoint;

        /// <summary>
        /// Gets or sets the health change info.
        /// </summary>
        /// <value>The health change info.</value>
        public HealthChangeInfo healthChangeInfo
        {
            get { return this.m_HealthChangeInfo; }
        }

        /// <summary>
        /// Gets or sets the damage point.
        /// </summary>
        /// <value>The damage point.</value>
        public Vector3 damagePoint
        {
            get { return this.m_DamagePoint; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitInfo" /> struct.
        /// </summary>
        /// <param name="info">The health change info</param>
        /// <param name="damageLocation">Damage point.</param>
        public HitInfo(HealthChangeInfo info, Vector3 damageLocation)
        {
            this.m_DamagePoint = damageLocation;
            this.m_HealthChangeInfo = info;
        }
    }
}