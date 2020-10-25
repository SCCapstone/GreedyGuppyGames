// <copyright file="Damager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Health;
using Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// A component that does damage to damageables
    /// </summary>
    public class Damager : MonoBehaviour
    {
        /// <summary>
        /// The amount of damage this damager does
        /// </summary>
        public float damage;

        /// <summary>
        /// The event that fires off when the damager has been damaged
        /// </summary>
        public Action<Vector3> hasDamaged;

        /// <summary>
        /// Random chance to spawn collision projectile prefab
        /// </summary>
        [Range(0, 1)]
        public float chanceToSpawnCollisionPrefab = 1.0f;

        /// <summary>
        /// The particle system to fire off when the damager attacks
        /// </summary>
        public ParticleSystem collisionParticles;

        /// <summary>
        /// The alignment of the damager
        /// </summary>
        public SerializableIAlignmentProvider alignment;

        /// <summary>
        /// Gets the alignment of the damager
        /// </summary>
        public IAlignmentProvider alignmentProvider
        {
            get { return this.alignment != null ? this.alignment.GetInterface() : null; }
        }

        /// <summary>
        /// The logic to set the value of the damage
        /// </summary>
        /// <param name="damageAmount">
        /// The amount to set the damage by,
        /// will not be set for values less than zero
        /// </param>
        public void SetDamage(float damageAmount)
        {
            if (damageAmount < 0)
            {
                return;
            }

            this.damage = damageAmount;
        }

        /// <summary>
        /// Damagable will tell damager that it has successful hurt the damagable
        /// </summary>
        public void HasDamaged(Vector3 point, IAlignmentProvider otherAlignment)
        {
            if (this.hasDamaged != null)
            {
                this.hasDamaged(point);
            }
        }

        /// <summary>
        /// Instantiate particle system and play it
        /// </summary>
        private void OnCollisionEnter(Collision other)
        {
            if (this.collisionParticles == null || Random.value > this.chanceToSpawnCollisionPrefab)
            {
                return;
            }

            var pfx = Poolable.TryGetPoolable<ParticleSystem>(this.collisionParticles.gameObject);

            pfx.transform.position = this.transform.position;
            pfx.Play();
        }
    }
}