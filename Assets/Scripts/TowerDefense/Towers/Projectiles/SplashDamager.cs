// <copyright file="SplashDamager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
    /// <summary>
    /// Component that will apply splash damage on collision enter
    /// </summary>
    public class SplashDamager : MonoBehaviour
    {
        /// <summary>
        /// The Area this projectile will attack in
        /// </summary>
        public float attackRange = 0.6f;

        /// <summary>
        /// The amount of damage done, a percentage of the damager damage
        /// </summary>
        public float damageAmount;

        /// <summary>
        /// The physics layer mask to search on
        /// </summary>
        public LayerMask mask = -1;

        /// <summary>
        /// The alignment of the projectile
        /// </summary>
        public SerializableIAlignmentProvider alignment;
        private static readonly Collider[] s_Enemies = new Collider[64];

        public float damage
        {
            get { return this.damageAmount; }
        }

        /// <summary>
        /// Gets this damager's alignment
        /// </summary>
        public IAlignmentProvider alignmentProvider
        {
            get { return this.alignment != null ? this.alignment.GetInterface() : null; }
        }

        /// <summary>
        /// Searches for Targetables within a radius of <see cref="attackRange"/>
        /// and damages them if valid
        /// </summary>
        protected virtual void OnCollisionEnter(Collision other)
        {
            int number = Physics.OverlapSphereNonAlloc(this.transform.position, this.attackRange, s_Enemies, this.mask);
            for (int index = 0; index < number; index++)
            {
                Collider enemy = s_Enemies[index];
                var damageable = enemy.GetComponent<Targetable>();
                if (damageable == null)
                {
                    continue;
                }

                damageable.TakeDamage(this.damageAmount, damageable.position, this.alignmentProvider);
            }
        }
    }
}