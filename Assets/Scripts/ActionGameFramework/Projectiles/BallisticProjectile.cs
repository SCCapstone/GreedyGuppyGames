// <copyright file="BallisticProjectile.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using ActionGameFramework.Helpers;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// Simple IProjectile implementation for projectile that flies in a parabolic arc with no further m_Acceleration.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BallisticProjectile : MonoBehaviour, IProjectile
    {
        public BallisticArcHeight arcPreference;

        public BallisticFireMode fireMode;

        [Range(-90, 90)]
        public float firingAngle;

        public float startSpeed;

        /// <summary>
        /// The duration that collisions between this gameObjects colliders
        /// and the given colliders will be ignored.
        /// </summary>
        public float collisionIgnoreTime = 0.35f;

        protected bool m_Fired, m_IgnoringCollsions;
        protected float m_CollisionIgnoreCount = 0;
        protected Rigidbody m_Rigidbody;
        protected List<Collider> m_CollidersIgnoring = new List<Collider>();

        /// <summary>
        /// All the colliders attached to this gameObject and its children
        /// </summary>
        protected Collider[] m_Colliders;

        public event Action fired;

        /// <summary>
        /// Fires this projectile from a designated start point to a designated world coordinate.
        /// Automatically sets firing angle to suit launch speed unless angle is overridden, in which case launch speed is overridden to suit angle.
        /// </summary>
        /// <param name="startPoint">Start point of the flight.</param>
        /// <param name="targetPoint">Target point to fly to.</param>
        public virtual void FireAtPoint(Vector3 startPoint, Vector3 targetPoint)
        {
            this.transform.position = startPoint;

            Vector3 firingVector;

            switch (this.fireMode)
            {
                case BallisticFireMode.UseLaunchSpeed:
                    firingVector =
                        Ballistics.CalculateBallisticFireVectorFromVelocity(startPoint, targetPoint, this.startSpeed, this.arcPreference);
                    this.firingAngle = Ballistics.CalculateBallisticFireAngle(startPoint, targetPoint, this.startSpeed, this.arcPreference);
                    break;
                case BallisticFireMode.UseLaunchAngle:
                    firingVector = Ballistics.CalculateBallisticFireVectorFromAngle(startPoint, targetPoint, this.firingAngle);
                    this.startSpeed = firingVector.magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.Fire(firingVector);
        }

        /// <summary>
        /// Fires this projectile in a designated direction at the launch speed.
        /// </summary>
        /// <param name="startPoint">Start point of the flight.</param>
        /// <param name="fireVector">Vector representing launch direction.</param>
        public virtual void FireInDirection(Vector3 startPoint, Vector3 fireVector)
        {
            this.transform.position = startPoint;

            this.Fire(fireVector.normalized * this.startSpeed);
        }

        /// <summary>
        /// Fires this projectile at a designated starting velocity, overriding any starting speeds.
        /// </summary>
        /// <param name="startPoint">Start point of the flight.</param>
        /// <param name="fireVelocity">Vector3 representing launch velocity.</param>
        public void FireAtVelocity(Vector3 startPoint, Vector3 fireVelocity)
        {
            this.transform.position = startPoint;

            this.startSpeed = fireVelocity.magnitude;

            this.Fire(fireVelocity);
        }

        /// <summary>
        /// Ignores all collisions between this and the given colliders for a defined period of time
        /// </summary>
        /// <param name="collidersToIgnore">Colliders to ignore</param>
        public void IgnoreCollision(Collider[] collidersToIgnore)
        {
            if (this.collisionIgnoreTime > 0)
            {
                this.m_IgnoringCollsions = true;
                this.m_CollisionIgnoreCount = 0.0f;
                foreach (Collider colliderToIgnore in collidersToIgnore)
                {
                    if (this.m_CollidersIgnoring.Contains(colliderToIgnore))
                    {
                        continue;
                    }

                    foreach (Collider projectileCollider in this.m_Colliders)
                    {
                        Physics.IgnoreCollision(colliderToIgnore, projectileCollider, true);
                    }

                    this.m_CollidersIgnoring.Add(colliderToIgnore);
                }
            }
        }

        protected virtual void Awake()
        {
            this.m_Rigidbody = this.GetComponent<Rigidbody>();
            this.m_Colliders = this.GetComponentsInChildren<Collider>();
        }

        protected virtual void Update()
        {
            if (!this.m_Fired)
            {
                return;
            }

            // If we are ignoring collisions, increment counter.
            // If counter is complete, reenable collisions
            if (this.m_IgnoringCollsions)
            {
                this.m_CollisionIgnoreCount += Time.deltaTime;
                if (this.m_CollisionIgnoreCount >= this.collisionIgnoreTime)
                {
                    this.m_IgnoringCollsions = false;
                    foreach (Collider colliderIgnoring in this.m_CollidersIgnoring)
                    {
                        foreach (Collider projectileCollider in this.m_Colliders)
                        {
                            Physics.IgnoreCollision(colliderIgnoring, projectileCollider, false);
                        }
                    }

                    this.m_CollidersIgnoring.Clear();
                }
            }

            this.transform.rotation = Quaternion.LookRotation(this.m_Rigidbody.velocity);
        }

        protected virtual void Fire(Vector3 firingVector)
        {
            this.transform.rotation = Quaternion.LookRotation(firingVector);

            this.m_Rigidbody.velocity = firingVector;

            this.m_Fired = true;

            this.m_CollidersIgnoring.Clear();

            if (this.fired != null)
            {
                this.fired();
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Mathf.Abs(this.firingAngle) >= 90f)
            {
                this.firingAngle = Mathf.Sign(this.firingAngle) * 89.5f;
                Debug.LogWarning("Clamping angle to under +- 90 degrees to avoid errors.");
            }
        }
#endif
    }
}