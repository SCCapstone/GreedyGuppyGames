// <copyright file="DeathEffect.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Utilities;
using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Simple class to instantiate a ParticleSystem on a given Damageable's death
    /// </summary>
    public class DeathEffect : MonoBehaviour
    {
        /// <summary>
        /// The DamageableBehaviour that will be used to assign the damageable
        /// </summary>
        [Tooltip("This field does not need to be populated here, it can be set up in code using AssignDamageable")]
        public DamageableBehaviour damageableBehaviour;

        /// <summary>
        /// Death particle system
        /// </summary>
        public ParticleSystem deathParticleSystemPrefab;

        /// <summary>
        /// World space offset of the <see cref="deathParticleSystemPrefab"/> position
        /// </summary>
        public Vector3 deathEffectOffset;

        /// <summary>
        /// The damageable
        /// </summary>
        protected Damageable m_Damageable;

        /// <summary>
        /// Subscribes to the damageable's died event
        /// </summary>
        /// <param name="damageable"></param>
        public void AssignDamageable(Damageable damageable)
        {
            if (this.m_Damageable != null)
            {
                this.m_Damageable.died -= this.OnDied;
            }

            this.m_Damageable = damageable;
            this.m_Damageable.died += this.OnDied;
        }

        /// <summary>
        /// If damageableBehaviour is populated, assigns the damageable
        /// </summary>
        protected virtual void Awake()
        {
            if (this.damageableBehaviour != null)
            {
                this.AssignDamageable(this.damageableBehaviour.configuration);
            }
        }

        /// <summary>
        /// Instantiate a death particle system
        /// </summary>
        private void OnDied(HealthChangeInfo healthChangeInfo)
        {
            if (this.deathParticleSystemPrefab == null)
            {
                return;
            }

            var pfx = Poolable.TryGetPoolable<ParticleSystem>(this.deathParticleSystemPrefab.gameObject);
            pfx.transform.position = this.transform.position + this.deathEffectOffset;
            pfx.Play();
        }
    }
}
