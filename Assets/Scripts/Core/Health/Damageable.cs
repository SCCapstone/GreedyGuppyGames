// <copyright file="Damageable.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Damageable class for handling health using events
    /// Could be used on Players or enemies or even destructable world objects
    /// </summary>
    [Serializable]
    public class Damageable
    {
        /// <summary>
        /// The max health of this instance
        /// </summary>
        public float maxHealth;

        public float startingHealth;

        /// <summary>
        /// The alignment of the damager
        /// </summary>
        public SerializableIAlignmentProvider alignment;

        /// <summary>
        /// Gets the current health.
        /// </summary>
        public float currentHealth { protected set; get; }

        /// <summary>
        /// Gets the normalised health.
        /// </summary>
        public float normalisedHealth
        {
            get
            {
                if (Math.Abs(this.maxHealth) <= Mathf.Epsilon)
                {
                    Debug.LogError("Max Health is 0");
                    this.maxHealth = 1f;
                }

                return this.currentHealth / this.maxHealth;
            }
        }

        /// <summary>
        /// Gets the <see cref="IAlignmentProvider"/> of this instance
        /// </summary>
        public IAlignmentProvider alignmentProvider
        {
            get
            {
                return this.alignment != null ? this.alignment.GetInterface() : null;
            }
        }

        /// <summary>
        /// Gets whether this instance is dead.
        /// </summary>
        public bool isDead
        {
            get { return this.currentHealth <= 0f; }
        }

        /// <summary>
        /// Gets whether this instance is at max health.
        /// </summary>
        public bool isAtMaxHealth
        {
            get { return Mathf.Approximately(this.currentHealth, this.maxHealth); }
        }

        // events
        public event Action reachedMaxHealth;

        public event Action<HealthChangeInfo> damaged, healed, died, healthChanged;

        /// <summary>
        /// Init this instance
        /// </summary>
        public virtual void Init()
        {
            this.currentHealth = this.startingHealth;
        }

        /// <summary>
        /// Sets the max health and starting health to the same value
        /// </summary>
        public void SetMaxHealth(float health)
        {
            if (health <= 0)
            {
                return;
            }

            this.maxHealth = this.startingHealth = health;
        }

        /// <summary>
        /// Sets the max health and starting health separately
        /// </summary>
        public void SetMaxHealth(float health, float startingHealth)
        {
            if (health <= 0)
            {
                return;
            }

            this.maxHealth = health;
            this.startingHealth = startingHealth;
        }

        /// <summary>
        /// Sets this instance's health directly.
        /// </summary>
        /// <param name="health">
        /// The value to set <see cref="currentHealth"/> to
        /// </param>
        public void SetHealth(float health)
        {
            var info = new HealthChangeInfo
            {
                damageable = this,
                newHealth = health,
                oldHealth = this.currentHealth
            };

            this.currentHealth = health;

            if (this.healthChanged != null)
            {
                this.healthChanged(info);
            }
        }

        /// <summary>
        /// Use the alignment to see if taking damage is a valid action
        /// </summary>
        /// <param name="damage">
        /// The damage to take
        /// </param>
        /// <param name="damageAlignment">
        /// The alignment of the other combatant
        /// </param>
        /// <param name="output">
        /// The output data if there is damage taken
        /// </param>
        /// <returns>
        /// <value>true if this instance took damage</value>
        /// <value>false if this instance was already dead, or the alignment did not allow the damage</value>
        /// </returns>
        public bool TakeDamage(float damage, IAlignmentProvider damageAlignment, out HealthChangeInfo output)
        {
            output = new HealthChangeInfo
            {
                damageAlignment = damageAlignment,
                damageable = this,
                newHealth = this.currentHealth,
                oldHealth = this.currentHealth
            };

            bool canDamage = damageAlignment == null || this.alignmentProvider == null ||
                             damageAlignment.CanHarm(this.alignmentProvider);

            if (this.isDead || !canDamage)
            {
                return false;
            }

            this.ChangeHealth(-damage, output);
            this.SafelyDoAction(this.damaged, output);
            if (this.isDead)
            {
                this.SafelyDoAction(this.died, output);
            }

            return true;
        }

        /// <summary>
        /// Logic for increasing the health.
        /// </summary>
        /// <param name="health">Health.</param>
        public HealthChangeInfo IncreaseHealth(float health)
        {
            var info = new HealthChangeInfo { damageable = this };
            this.ChangeHealth(health, info);
            this.SafelyDoAction(this.healed, info);
            if (this.isAtMaxHealth)
            {
                this.SafelyDoAction(this.reachedMaxHealth);
            }

            return info;
        }

        /// <summary>
        /// Changes the health.
        /// </summary>
        /// <param name="healthIncrement">Health increment.</param>
        /// <param name="info">HealthChangeInfo for this change</param>
        protected void ChangeHealth(float healthIncrement, HealthChangeInfo info)
        {
            info.oldHealth = this.currentHealth;
            this.currentHealth += healthIncrement;
            this.currentHealth = Mathf.Clamp(this.currentHealth, 0f, this.maxHealth);
            info.newHealth = this.currentHealth;

            if (this.healthChanged != null)
            {
                this.healthChanged(info);
            }
        }

        /// <summary>
        /// A helper method for null checking actions
        /// </summary>
        /// <param name="action">Action to be done</param>
        protected void SafelyDoAction(Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        /// <summary>
        /// A helper method for null checking actions
        /// </summary>
        /// <param name="action">Action to be done</param>
        /// <param name="info">The HealthChangeInfo to be passed to the Action</param>
        protected void SafelyDoAction(Action<HealthChangeInfo> action, HealthChangeInfo info)
        {
            if (action != null)
            {
                action(info);
            }
        }
    }
}