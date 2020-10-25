// <copyright file="DamageableListener.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Health
{
    /// <summary>
    /// A UnityEvent that passes through the HealthChangeInfo
    /// </summary>
    [Serializable]
    public class HealthChangeEvent : UnityEvent<HealthChangeInfo>
    {
    }

    /// <summary>
    /// A UnityEvent that passes through the HitInfo
    /// </summary>
    [Serializable]
    public class HitEvent : UnityEvent<HitInfo>
    {
    }

    /// <summary>
    /// Damageable listener.
    /// </summary>
    public class DamageableListener : MonoBehaviour
    {
        // The damageable behaviour to listen to
        [Tooltip("Leave this empty if the DamageableBehaviour and DamageableListener are on the same component")]
        public DamageableBehaviour damageableBehaviour;

        // Events for health change (i.e. healing/damage) - to be configured in the editor
        public HealthChangeEvent damaged;

        public HealthChangeEvent healed;

        // Events for death and max health - to be configured in the editor
        public UnityEvent died;

        public UnityEvent reachedMaxHealth;

        // Event for when health is change
        public HealthChangeEvent healthChanged;

        // Event for hits
        [Header("The hit event is different from the damage event as it also contains hit position data")]
        public HitEvent hit;

        /// <summary>
        /// Lazy loads the DamageableBehaviour
        /// </summary>
        protected virtual void Awake()
        {
            this.LazyLoad();
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.damageableBehaviour.configuration.died += this.OnDeath;
            this.damageableBehaviour.configuration.reachedMaxHealth += this.OnReachedMaxHealth;
            this.damageableBehaviour.configuration.healed += this.OnHealed;
            this.damageableBehaviour.configuration.damaged += this.OnDamaged;
            this.damageableBehaviour.configuration.healthChanged += this.OnHealthChanged;
            this.damageableBehaviour.hit += this.OnHit;
        }

        /// <summary>
        /// Unsubscribes from events on disable
        /// </summary>
        protected virtual void OnDisable()
        {
            this.damageableBehaviour.configuration.died -= this.OnDeath;
            this.damageableBehaviour.configuration.reachedMaxHealth -= this.OnReachedMaxHealth;
            this.damageableBehaviour.configuration.healed -= this.OnHealed;
            this.damageableBehaviour.configuration.damaged -= this.OnDamaged;
            this.damageableBehaviour.configuration.healthChanged -= this.OnHealthChanged;
            this.damageableBehaviour.hit -= this.OnHit;
        }

        /// <summary>
        /// Raises the death UnityEvent.
        /// </summary>
        protected virtual void OnDeath(HealthChangeInfo info)
        {
            this.died.Invoke();
        }

        /// <summary>
        /// Raises the max health UnityEvent.
        /// </summary>
        protected virtual void OnReachedMaxHealth()
        {
            this.reachedMaxHealth.Invoke();
        }

        /// <summary>
        /// Raises the heal UnityEvent.
        /// </summary>
        /// <param name="info">Info.</param>
        protected virtual void OnHealed(HealthChangeInfo info)
        {
            this.healed.Invoke(info);
        }

        /// <summary>
        /// Raises the damage UnityEvent.
        /// </summary>
        /// <param name="info">Info.</param>
        protected virtual void OnDamaged(HealthChangeInfo info)
        {
            this.damaged.Invoke(info);
        }

        /// <summary>
        /// Raises the healthChanged UnityEvent.
        /// </summary>
        /// <param name="info">Info.</param>
        protected virtual void OnHealthChanged(HealthChangeInfo info)
        {
            this.healthChanged.Invoke(info);
        }

        /// <summary>
        /// Raises the hit UnityEvent.
        /// </summary>
        /// <param name="info">Info.</param>
        protected virtual void OnHit(HitInfo info)
        {
            this.hit.Invoke(info);
        }

        /// <summary>
        /// Looks for the damageableBehaviour if it is not already assigned
        /// It may be assigned in editor or from a previous LazyLoad() call
        /// </summary>
        protected void LazyLoad()
        {
            if (this.damageableBehaviour != null)
            {
                return;
            }

            this.damageableBehaviour = this.GetComponent<DamageableBehaviour>();
        }
    }
}