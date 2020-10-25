// <copyright file="Affector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Health;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// A class for providing information on to whether
    /// the children classes effects are valid
    /// </summary>
    public abstract class Affector : MonoBehaviour
    {
        /// <summary>
        /// Short description for affector for displaying in the UI
        /// </summary>
        public string description;

        /// <summary>
        /// Gets or sets the alignment
        /// </summary>
        public IAlignmentProvider alignment { get; protected set; }

        /// <summary>
        /// The physics mask to check against
        /// </summary>
        public LayerMask enemyMask { get; protected set; }

        /// <summary>
        /// Initializes the effect with search data
        /// </summary>
        /// <param name="affectorAlignment">
        /// The alignment of the effect for search purposes
        /// </param>
        /// <param name="mask">
        /// The physics layer of to search for
        /// </param>
        public virtual void Initialize(IAlignmentProvider affectorAlignment, LayerMask mask)
        {
            this.alignment = affectorAlignment;
            this.enemyMask = mask;
        }

        /// <summary>
        /// Initializes the effect with search data
        /// </summary>
        /// <param name="affectorAlignment">
        /// The alignment of the effect for search purposes
        /// </param>
        public virtual void Initialize(IAlignmentProvider affectorAlignment)
        {
            this.Initialize(affectorAlignment, -1);
        }
    }
}