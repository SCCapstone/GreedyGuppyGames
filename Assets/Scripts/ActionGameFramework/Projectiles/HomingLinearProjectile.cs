// <copyright file="HomingLinearProjectile.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// Basic override of LinearProjectile that allows them to adjust their path in-flight to intercept a designated target.
    /// </summary>
    public class HomingLinearProjectile : LinearProjectile
    {
        public int leadingPrecision = 2;

        public bool leadTarget;

        protected Targetable m_HomingTarget;
        private Vector3 m_TargetVelocity;

        /// <summary>
        /// Sets the target transform that will be homed in on once fired.
        /// </summary>
        /// <param name="target">Transform of the target to home in on.</param>
        public void SetHomingTarget(Targetable target)
        {
            this.m_HomingTarget = target;
        }

        protected virtual void FixedUpdate()
        {
            if (this.m_HomingTarget == null)
            {
                return;
            }

            this.m_TargetVelocity = this.m_HomingTarget.velocity;
        }

        protected override void Update()
        {
            if (!this.m_Fired)
            {
                return;
            }

            if (this.m_HomingTarget == null)
            {
                this.m_Rigidbody.rotation = Quaternion.LookRotation(this.m_Rigidbody.velocity);
                return;
            }

            Quaternion aimDirection = Quaternion.LookRotation(this.GetHeading());

            this.m_Rigidbody.rotation = aimDirection;
            this.m_Rigidbody.velocity = this.transform.forward * this.m_Rigidbody.velocity.magnitude;

            base.Update();
        }

        protected Vector3 GetHeading()
        {
            if (this.m_HomingTarget == null)
            {
                return Vector3.zero;
            }

            Vector3 heading;
            if (this.leadTarget)
            {
                heading = Ballistics.CalculateLinearLeadingTargetPoint(this.transform.position, this.m_HomingTarget.position,
                                                                       this.m_TargetVelocity, this.m_Rigidbody.velocity.magnitude,
                                                                       this.acceleration,
                                                                       this.leadingPrecision) - this.transform.position;
            }
            else
            {
                heading = this.m_HomingTarget.position - this.transform.position;
            }

            return heading.normalized;
        }

        protected override void Fire(Vector3 firingVector)
        {
            if (this.m_HomingTarget == null)
            {
                Debug.LogError("Homing target has not been specified. Aborting fire.");
                return;
            }

            this.m_HomingTarget.removed += this.OnTargetDied;

            base.Fire(firingVector);
        }

        private void OnTargetDied(DamageableBehaviour targetable)
        {
            targetable.removed -= this.OnTargetDied;
            this.m_HomingTarget = null;
        }
    }
}