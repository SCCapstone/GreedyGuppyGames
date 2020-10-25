// <copyright file="HitscanAttack.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
    /// <summary>
    /// Implementation of hitscan projectile
    /// The principle behind this weapon is that it instantly attacks enemies
    /// </summary>
    [RequireComponent(typeof(Damager))]
    public class HitscanAttack : MonoBehaviour
    {
        /// <summary>
        /// The amount of time to delay
        /// </summary>
        public float delay;

        /// <summary>
        /// The delay timer
        /// </summary>
        protected Timer m_Timer;

        /// <summary>
        /// The enemy this projectile will attack
        /// </summary>
        protected Targetable m_Enemy;

        /// <summary>
        /// The Damager attached to the object
        /// </summary>
        protected Damager m_Damager;

        /// <summary>
        /// The towers projectile position
        /// </summary>
        protected Vector3 m_Origin;

        /// <summary>
        /// Configuration for pausing the timer delay timer
        /// without setting Time.timeScale to 0
        /// </summary>
        protected bool m_PauseTimer;

        /// <summary>
        /// The delay configuration for the attacking
        /// </summary>
        /// <param name="origin">
        /// The point the attack will be fired from
        /// </param>
        /// <param name="enemy">
        /// The enemy to attack
        /// </param>
        public void AttackEnemy(Vector3 origin, Targetable enemy)
        {
            this.m_Enemy = enemy;
            this.m_Origin = origin;
            this.m_Timer.Reset();
            this.m_PauseTimer = false;
        }

        /// <summary>
        /// The actual attack of the hitscan attack.
        /// Early returns from the method if the there is no enemy to attack.
        /// </summary>
        protected void DealDamage()
        {
            Poolable.TryPool(this.gameObject);

            if (this.m_Enemy == null)
            {
                return;
            }

            // effects
            ParticleSystem pfxPrefab = this.m_Damager.collisionParticles;
            var attackEffect = Poolable.TryGetPoolable<ParticleSystem>(pfxPrefab.gameObject);
            attackEffect.transform.position = this.m_Enemy.position;
            attackEffect.Play();

            this.m_Enemy.TakeDamage(this.m_Damager.damage, this.m_Enemy.position, this.m_Damager.alignmentProvider);
            this.m_PauseTimer = true;
        }

        /// <summary>
        /// Cache the damager component attached to this object
        /// </summary>
        protected virtual void Awake()
        {
            this.m_Damager = this.GetComponent<Damager>();
            this.m_Timer = new Timer(this.delay, this.DealDamage);
        }

        /// <summary>
        /// Update the m_Timer if it is available
        /// </summary>
        protected virtual void Update()
        {
            if (!this.m_PauseTimer)
            {
                this.m_Timer.Tick(Time.deltaTime);
            }
        }
    }
}