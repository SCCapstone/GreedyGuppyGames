// <copyright file="ProjectileEffect.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Projectiles;
using Core.Utilities;
using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.Effects
{
    /// <summary>
    /// Class for spawning and managing effects on this projectile. Used for effects that should persist
    /// a little longer after a projectile is destroyed/repooled. Creates the effect on enable, moves it to
    /// follow us every frame while we're active.
    ///
    /// On disable, it'll try and find a SelfDestroyTimer on the effect to trigger its destruction, otherwise
    /// repools it immediately.
    /// </summary>
    [RequireComponent(typeof(IProjectile))]
    public class ProjectileEffect : MonoBehaviour
    {
        /// <summary>
        /// Preafb that gets spawned when this projectile fires
        /// </summary>
        public GameObject effectPrefab;

        /// <summary>
        /// Transform the effect follows
        /// </summary>
        public Transform followTransform;

        /// <summary>
        /// Cached spawned effect
        /// </summary>
        private GameObject m_SpawnedEffect;

        /// <summary>
        /// Cached destruction timer on the spawned object
        /// </summary>
        private SelfDestroyTimer m_DestroyTimer;

        /// <summary>
        /// Cached poolable effect on the spawned object
        /// </summary>
        private PoolableEffect m_Resetter;

        /// <summary>
        /// Cached projectile
        /// </summary>
        private IProjectile m_Projectile;

        /// <summary>
        /// Register projectile fire events
        /// </summary>
        protected virtual void Awake()
        {
            this.m_Projectile = this.GetComponent<IProjectile>();
            this.m_Projectile.fired += this.OnFired;
            if (this.followTransform != null)
            {
                this.followTransform = this.transform;
            }
        }

        /// <summary>
        /// Unregister delegates
        /// </summary>
        protected virtual void OnDestroy()
        {
            this.m_Projectile.fired -= this.OnFired;
        }

        /// <summary>
        /// Spawn our effect
        /// </summary>
        protected virtual void OnFired()
        {
            if (this.effectPrefab != null)
            {
                this.m_SpawnedEffect = Poolable.TryGetPoolable(this.effectPrefab);
                this.m_SpawnedEffect.transform.parent = null;
                this.m_SpawnedEffect.transform.position = this.followTransform.position;
                this.m_SpawnedEffect.transform.rotation = this.followTransform.rotation;

                // Make sure to disable timer if it's on initially, so it doesn't destroy this object
                this.m_DestroyTimer = this.m_SpawnedEffect.GetComponent<SelfDestroyTimer>();
                if (this.m_DestroyTimer != null)
                {
                    this.m_DestroyTimer.enabled = false;
                }

                this.m_Resetter = this.m_SpawnedEffect.GetComponent<PoolableEffect>();
                if (this.m_Resetter != null)
                {
                    this.m_Resetter.TurnOnAllSystems();
                }
            }
        }

        /// <summary>
        /// Make effect follow us
        /// </summary>
        protected virtual void Update()
        {
            // Make the effect follow our position.
            // We don't reparent it because it should not be disabled when we are
            if (this.m_SpawnedEffect != null)
            {
                this.m_SpawnedEffect.transform.position = this.followTransform.position;
            }
        }

        /// <summary>
        /// Destroy and start destruction of effect
        /// </summary>
        protected virtual void OnDisable()
        {
            if (this.m_SpawnedEffect == null)
            {
                return;
            }

            // Initiate destruction timer
            if (this.m_DestroyTimer != null)
            {
                this.m_DestroyTimer.enabled = true;

                if (this.m_Resetter != null)
                {
                    this.m_Resetter.StopAll();
                }
            }
            else
            {
                // Repool immediately
                Poolable.TryPool(this.m_SpawnedEffect);
            }

            this.m_SpawnedEffect = null;
            this.m_DestroyTimer = null;
        }
    }
}