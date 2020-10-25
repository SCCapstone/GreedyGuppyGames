// <copyright file="Targetable.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// A simple class for identifying enemies
    /// </summary>
    public class Targetable : DamageableBehaviour
    {
        /// <summary>
        /// The transform that will be targeted
        /// </summary>
        public Transform targetTransform;

        /// <summary>
        /// The position of the object
        /// </summary>
        protected Vector3 m_CurrentPosition, m_PreviousPosition;

        /// <summary>
        /// The velocity of the rigidbody
        /// </summary>
        public virtual Vector3 velocity { get; protected set; }

        /// <summary>
        /// The transform that objects target, which falls back to this object's transform if not set
        /// </summary>
        public Transform targetableTransform
        {
            get
            {
                return this.targetTransform == null ? this.transform : this.targetTransform;
            }
        }

        /// <summary>
        /// Returns our targetable's transform position
        /// </summary>
        public override Vector3 position
        {
            get { return this.targetableTransform.position; }
        }

        /// <summary>
        /// Initialises any DamageableBehaviour logic
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            this.ResetPositionData();
        }

        /// <summary>
        /// Sets up the position data so velocity can be calculated
        /// </summary>
        protected void ResetPositionData()
        {
            this.m_CurrentPosition = this.position;
            this.m_PreviousPosition = this.position;
        }

        /// <summary>
        /// Calculates the velocity and updates the position
        /// </summary>
        private void FixedUpdate()
        {
            this.m_CurrentPosition = this.position;
            this.velocity = (this.m_CurrentPosition - this.m_PreviousPosition) / Time.fixedDeltaTime;
            this.m_PreviousPosition = this.m_CurrentPosition;
        }
    }
}