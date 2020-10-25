// <copyright file="AttackAffector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using ActionGameFramework.Audio;
using ActionGameFramework.Health;
using Core.Health;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using TowerDefense.Towers.Projectiles;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// The common effect for handling firing projectiles to attack
    ///
    /// Requires an ILauncher but it is not automatically added
    /// Add an ILauncher implementation to this GameObject before you add this script
    /// </summary>
    [RequireComponent(typeof(ILauncher))]
    public class AttackAffector : Affector, ITowerRadiusProvider
    {
        /// <summary>
        /// The projectile used to attack
        /// </summary>
        public GameObject projectile;

        /// <summary>
        /// The list of points to launch the projectiles from
        /// </summary>
        public Transform[] projectilePoints;

        /// <summary>
        /// The reference to the center point where the tower will search from
        /// </summary>
        public Transform epicenter;

        /// <summary>
        /// Configuration for when the tower does splash damage
        /// </summary>
        public bool isMultiAttack;

        /// <summary>
        /// The fire rate in fires-per-second
        /// </summary>
        public float fireRate;

        /// <summary>
        /// The audio source to play when firing
        /// </summary>
        public RandomAudioSource randomAudioSource;

        /// <summary>
        /// Gets the targetter
        /// </summary>
        public Targetter towerTargetter;

        /// <summary>
        /// Color of effect radius visualization
        /// </summary>
        public Color radiusEffectColor;

        /// <summary>
        /// Search condition
        /// </summary>
        public Filter searchCondition;

        /// <summary>
        /// Fire condition
        /// </summary>
        public Filter fireCondition;

        /// <summary>
        /// The reference to the attached launcher
        /// </summary>
        protected ILauncher m_Launcher;

        /// <summary>
        /// The time before firing is possible
        /// </summary>
        protected float m_FireTimer;

        /// <summary>
        /// Reference to the current tracked enemy
        /// </summary>
        protected Targetable m_TrackingEnemy;

        /// <summary>
        /// Gets the search rate from the targetter
        /// </summary>
        public float searchRate
        {
            get { return this.towerTargetter.searchRate; }
            set { this.towerTargetter.searchRate = value; }
        }

        /// <summary>
        /// Gets the targetable
        /// </summary>
        public Targetable trackingEnemy
        {
            get { return this.m_TrackingEnemy; }
        }

        /// <summary>
        /// Gets or sets the attack radius
        /// </summary>
        public float effectRadius
        {
            get { return this.towerTargetter.effectRadius; }
        }

        public Color effectColor
        {
            get { return this.radiusEffectColor; }
        }

        public Targetter targetter
        {
            get { return this.towerTargetter; }
        }

        /// <summary>
        /// Initializes the attack affector
        /// </summary>
        public override void Initialize(IAlignmentProvider affectorAlignment)
        {
            this.Initialize(affectorAlignment, -1);
        }

        /// <summary>
        /// Initialises the  attack affector with a layer mask
        /// </summary>
        public override void Initialize(IAlignmentProvider affectorAlignment, LayerMask mask)
        {
            base.Initialize(affectorAlignment, mask);
            this.SetUpTimers();

            this.towerTargetter.ResetTargetter();
            this.towerTargetter.alignment = affectorAlignment;
            this.towerTargetter.acquiredTarget += this.OnAcquiredTarget;
            this.towerTargetter.lostTarget += this.OnLostTarget;
        }

        private void OnDestroy()
        {
            this.towerTargetter.acquiredTarget -= this.OnAcquiredTarget;
            this.towerTargetter.lostTarget -= this.OnLostTarget;
        }

        private void OnLostTarget()
        {
            this.m_TrackingEnemy = null;
        }

        private void OnAcquiredTarget(Targetable acquiredTarget)
        {
            this.m_TrackingEnemy = acquiredTarget;
        }

        public Damager damagerProjectile
        {
            get { return this.projectile == null ? null : this.projectile.GetComponent<Damager>(); }
        }

        /// <summary>
        /// Returns the total projectile damage
        /// </summary>
        public float GetProjectileDamage()
        {
            var splash = this.projectile.GetComponent<SplashDamager>();
            float splashDamage = splash != null ? splash.damage : 0;
            return this.damagerProjectile.damage + splashDamage;
        }

        /// <summary>
        /// Initialise the RepeatingTimer
        /// </summary>
        protected virtual void SetUpTimers()
        {
            this.m_FireTimer = 1 / this.fireRate;
            this.m_Launcher = this.GetComponent<ILauncher>();
        }

        /// <summary>
        /// Update the timers
        /// </summary>
        protected virtual void Update()
        {
            this.m_FireTimer -= Time.deltaTime;
            if (this.trackingEnemy != null && this.m_FireTimer <= 0.0f)
            {
                this.OnFireTimer();
                this.m_FireTimer = 1 / this.fireRate;
            }
        }

        /// <summary>
        /// Fired at every poll of the fire rate timer
        /// </summary>
        protected virtual void OnFireTimer()
        {
            if (this.fireCondition != null)
            {
                if (!this.fireCondition())
                {
                    return;
                }
            }

            this.FireProjectile();
        }

        /// <summary>
        /// Common logic when attacking
        /// </summary>
        protected virtual void FireProjectile()
        {
            if (this.m_TrackingEnemy == null)
            {
                return;
            }

            if (this.isMultiAttack)
            {
                List<Targetable> enemies = this.towerTargetter.GetAllTargets();
                this.m_Launcher.Launch(enemies, this.projectile, this.projectilePoints);
            }
            else
            {
                this.m_Launcher.Launch(this.m_TrackingEnemy, this.damagerProjectile.gameObject, this.projectilePoints);
            }

            if (this.randomAudioSource != null)
            {
                this.randomAudioSource.PlayRandomClip();
            }
        }

        /// <summary>
        /// A delegate to compare distances of components
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        protected virtual int ByDistance(Targetable first, Targetable second)
        {
            float firstSqrMagnitude = Vector3.SqrMagnitude(first.position - this.epicenter.position);
            float secondSqrMagnitude = Vector3.SqrMagnitude(second.position - this.epicenter.position);
            return firstSqrMagnitude.CompareTo(secondSqrMagnitude);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draws the search area
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.epicenter.position, this.towerTargetter.effectRadius);
        }
#endif
    }

    /// <summary>
    /// A delegate for boolean calculation logic
    /// </summary>
    public delegate bool Filter();
}