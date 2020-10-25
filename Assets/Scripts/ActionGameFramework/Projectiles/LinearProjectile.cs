// <copyright file="LinearProjectile.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using ActionGameFramework.Helpers;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// Simple IProjectile implementation for a projectile that flies in a straight line, optionally under m_Acceleration.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LinearProjectile : MonoBehaviour, IProjectile
    {
        public float acceleration;

        public float startSpeed;

        protected bool m_Fired;

        protected Rigidbody m_Rigidbody;

        public event Action fired;

        /// <summary>
        /// Fires this projectile from a designated start point to a designated world coordinate.
        /// </summary>
        /// <param name="startPoint">Start point of the flight.</param>
        /// <param name="targetPoint">Target point to fly to.</param>
        public virtual void FireAtPoint(Vector3 startPoint, Vector3 targetPoint)
        {
            this.transform.position = startPoint;

            this.Fire(Ballistics.CalculateLinearFireVector(startPoint, targetPoint, this.startSpeed));
        }

        /// <summary>
        /// Fires this projectile in a designated direction.
        /// </summary>
        /// <param name="startPoint">Start point of the flight.</param>
        /// <param name="fireVector">Vector representing direction of flight.</param>
        public virtual void FireInDirection(Vector3 startPoint, Vector3 fireVector)
        {
            this.transform.position = startPoint;

            // If we have no initial speed, we provide a small one to give the launch vector a baseline magnitude.
            if (Math.Abs(this.startSpeed) < float.Epsilon)
            {
                this.startSpeed = 0.001f;
            }

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

        protected virtual void Awake()
        {
            this.m_Rigidbody = this.GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (!this.m_Fired)
            {
                return;
            }

            if (Math.Abs(this.acceleration) >= float.Epsilon)
            {
                this.m_Rigidbody.velocity += this.transform.forward * this.acceleration * Time.deltaTime;
            }
        }

        protected virtual void Fire(Vector3 firingVector)
        {
            this.m_Fired = true;

            this.transform.rotation = Quaternion.LookRotation(firingVector);

            this.m_Rigidbody.velocity = firingVector;

            if (this.fired != null)
            {
                this.fired();
            }
        }
    }
}