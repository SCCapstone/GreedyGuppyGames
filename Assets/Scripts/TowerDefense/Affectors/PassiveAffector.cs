// <copyright file="PassiveAffector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Agents;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// Abstract class that is used to apply <see cref="AgentEffect"/>s to <see cref="Agent"/>s
    /// </summary>
    [RequireComponent(typeof(Targetter))]
    public abstract class PassiveAffector : Affector, ITowerRadiusProvider
    {
        /// <summary>
        /// Color of effect radius visualization
        /// </summary>
        public Color radiusEffectColor;

        public Targetter towerTargetter;

        /// <summary>
        /// Gets or sets the attack radius
        /// </summary>
        public float effectRadius
        {
            get { return this.towerTargetter.effectRadius; }
        }

        /// <summary>
        /// Gets the color used for effect radius visualisation
        /// </summary>
        public Color effectColor
        {
            get { return this.radiusEffectColor; }
        }

        /// <summary>
        /// Gets the targetter
        /// </summary>
        public Targetter targetter
        {
            get { return this.towerTargetter; }
        }
    }
}