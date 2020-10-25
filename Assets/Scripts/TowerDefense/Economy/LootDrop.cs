// <copyright file="LootDrop.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Health;
using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Economy
{
    /// <summary>
    /// A class that adds money to the currency when the attached DamagableBehaviour dies
    /// </summary>
    [RequireComponent(typeof(DamageableBehaviour))]
    public class LootDrop : MonoBehaviour
    {
        /// <summary>
        /// The amount of loot/currency dropped when object "dies"
        /// </summary>
        public int lootDropped = 1;

        /// <summary>
        /// The attached DamagableBehaviour
        /// </summary>
        protected DamageableBehaviour m_DamageableBehaviour;

        /// <summary>
        /// Caches attached DamageableBehaviour
        /// </summary>
        protected virtual void OnEnable()
        {
            if (this.m_DamageableBehaviour == null)
            {
                this.m_DamageableBehaviour = this.GetComponent<DamageableBehaviour>();
            }

            this.m_DamageableBehaviour.configuration.died += this.OnDeath;
        }

        /// <summary>
        /// Unsubscribed from the <see cref="m_DamageableBehaviour"/> died event
        /// </summary>
        protected virtual void OnDisable()
        {
            this.m_DamageableBehaviour.configuration.died -= this.OnDeath;
        }

        /// <summary>
        /// The callback for when the attached object "dies".
        /// Add <see cref="lootDropped"/> to current currency
        /// </summary>
        protected virtual void OnDeath(HealthChangeInfo info)
        {
            this.m_DamageableBehaviour.configuration.died -= this.OnDeath;

            if (info.damageAlignment == null ||
                !info.damageAlignment.CanHarm(this.m_DamageableBehaviour.configuration.alignmentProvider))
            {
                return;
            }

            LevelManager levelManager = LevelManager.instance;
            if (levelManager == null)
            {
                return;
            }

            levelManager.currency.AddCurrency(this.lootDropped);
        }
    }
}