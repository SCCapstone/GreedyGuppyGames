// <copyright file="ContactDestroyer.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
    /// <summary>
    /// For objects that destroyer themselves on contact
    /// </summary>
    public class ContactDestroyer : MonoBehaviour
    {
        /// <summary>
        /// The y-value of the position the object will destroy itself
        /// </summary>
        public float yDestroyPoint = -50;

        /// <summary>
        /// The attached collider
        /// </summary>
        protected Collider m_AttachedCollider;

        /// <summary>
        /// Caches the attached collider
        /// </summary>
        protected virtual void Awake()
        {
            this.m_AttachedCollider = this.GetComponent<Collider>();
        }

        /// <summary>
        /// Checks the y-position against <see cref="yDestroyPoint"/>
        /// </summary>
        protected virtual void Update()
        {
            if (this.transform.position.y < this.yDestroyPoint)
            {
                this.ReturnToPool();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            this.ReturnToPool();
        }

        /// <summary>
        /// Returns the object to pool if possible, otherwise destroys
        /// </summary>
        private void ReturnToPool()
        {
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }

            Poolable.TryPool(this.gameObject);
        }
    }
}