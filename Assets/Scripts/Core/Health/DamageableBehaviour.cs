// <copyright file="DamageableBehaviour.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Abstract class for any MonoBehaviours that can take damage
    /// </summary>
    public class DamageableBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The Damageable object
        /// </summary>
        public Damageable configuration;

        /// <summary>
        /// Gets whether this <see cref="DamageableBehaviour" /> is dead.
        /// </summary>
        /// <value>True if dead</value>
        public bool isDead
        {
            get { return this.configuration.isDead; }
        }

        /// <summary>
        /// The position of the transform
        /// </summary>
        public virtual Vector3 position
        {
            get { return this.transform.position; }
        }

        /// <summary>
        /// Occurs when damage is taken
        /// </summary>
        public event Action<HitInfo> hit;

        /// <summary>
        /// Event that is fired when this instance is removed, such as when pooled or destroyed
        /// </summary>
        public event Action<DamageableBehaviour> removed;

        /// <summary>
        /// Event that is fired when this instance is killed
        /// </summary>
        public event Action<DamageableBehaviour> died;

        /// <summary>
        /// Takes the damage and also provides a position for the damage being dealt
        /// </summary>
        /// <param name="damageValue">Damage value.</param>
        /// <param name="damagePoint">Damage point.</param>
        /// <param name="alignment">Alignment value</param>
        public virtual void TakeDamage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignment)
        {
            HealthChangeInfo info;
            this.configuration.TakeDamage(damageValue, alignment, out info);
            var damageInfo = new HitInfo(info, damagePoint);
            if (this.hit != null)
            {
                this.hit(damageInfo);
            }
        }

        protected virtual void Awake()
        {
            this.configuration.Init();
            this.configuration.died += this.OnConfigurationDied;
        }

        /// <summary>
        /// Kills this damageable
        /// </summary>
        protected virtual void Kill()
        {
            HealthChangeInfo healthChangeInfo;
            this.configuration.TakeDamage(this.configuration.currentHealth, null, out healthChangeInfo);
        }

        /// <summary>
        /// Removes this damageable without killing it
        /// </summary>
        public virtual void Remove()
        {
            // Set health to zero so that this behaviour appears to be dead. This will not fire death events
            this.configuration.SetHealth(0);
            this.OnRemoved();
        }

        /// <summary>
        /// Fires kill events
        /// </summary>
        private void OnDeath()
        {
            if (this.died != null)
            {
                this.died(this);
            }
        }

        /// <summary>
        /// Fires the removed event
        /// </summary>
        private void OnRemoved()
        {
            if (this.removed != null)
            {
                this.removed(this);
            }
        }

        /// <summary>
        /// Event fired when Damageable takes critical damage
        /// </summary>
        private void OnConfigurationDied(HealthChangeInfo changeInfo)
        {
            this.OnDeath();
            this.Remove();
        }
    }
}