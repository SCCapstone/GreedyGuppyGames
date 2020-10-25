// <copyright file="Targetter.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.Targetting
{
    /// <summary>
    /// Class used to track targets for an affector
    /// </summary>
    public class Targetter : MonoBehaviour
    {
        /// <summary>
        /// Fires when a targetable enters the target collider
        /// </summary>
        public event Action<Targetable> targetEntersRange;

        /// <summary>
        /// Fires when a targetable exits the target collider
        /// </summary>
        public event Action<Targetable> targetExitsRange;

        /// <summary>
        /// Fires when an appropriate target is found
        /// </summary>
        public event Action<Targetable> acquiredTarget;

        /// <summary>
        /// Fires when the current target was lost
        /// </summary>
        public event Action lostTarget;

        /// <summary>
        /// The transform to point at the target
        /// </summary>
        public Transform turret;

        /// <summary>
        /// The range of the turret's x rotation
        /// </summary>
        public Vector2 turretXRotationRange = new Vector2(0, 359);

        /// <summary>
        /// If m_Turret rotates freely or only on y;
        /// </summary>
        public bool onlyYTurretRotation;

        /// <summary>
        /// The search rate in searches per second
        /// </summary>
        public float searchRate;

        /// <summary>
        /// Y rotation speed while the turret is idle in degrees per second
        /// </summary>
        public float idleRotationSpeed = 39f;

        /// <summary>
        /// The time it takes for the tower to correct its x rotation on idle in seconds
        /// </summary>
        public float idleCorrectionTime = 2.0f;

        /// <summary>
        /// The collider attached to the targetter
        /// </summary>
        public Collider attachedCollider;

        /// <summary>
        /// How long the turret waits in its idle form before spinning in seconds
        /// </summary>
        public float idleWaitTime = 2.0f;

        /// <summary>
        /// The current targetables in the collider
        /// </summary>
        protected List<Targetable> m_TargetsInRange = new List<Targetable>();

        /// <summary>
        /// The seconds until a search is allowed
        /// </summary>
        protected float m_SearchTimer = 0.0f;

        /// <summary>
        /// The seconds until the tower starts spinning
        /// </summary>
        protected float m_WaitTimer = 0.0f;

        /// <summary>
        /// The current targetable
        /// </summary>
        protected Targetable m_CurrrentTargetable;

        /// <summary>
        /// Counter used for x rotation correction
        /// </summary>
        protected float m_XRotationCorrectionTime;

        /// <summary>
        /// If there was a targetable in the last frame
        /// </summary>
        protected bool m_HadTarget;

        /// <summary>
        /// How fast this turret is spinning
        /// </summary>
        protected float m_CurrentRotationSpeed;

        /// <summary>
        /// returns the radius of the collider whether
        /// its a sphere or capsule
        /// </summary>
        public float effectRadius
        {
            get
            {
                var sphere = this.attachedCollider as SphereCollider;
                if (sphere != null)
                {
                    return sphere.radius;
                }

                var capsule = this.attachedCollider as CapsuleCollider;
                if (capsule != null)
                {
                    return capsule.radius;
                }

                return 0;
            }
        }

        /// <summary>
        /// The alignment of the affector
        /// </summary>
        public IAlignmentProvider alignment;

        /// <summary>
        /// Returns the current target
        /// </summary>
        public Targetable GetTarget()
        {
            return this.m_CurrrentTargetable;
        }

        /// <summary>
        /// Clears the list of current targets and clears all events
        /// </summary>
        public void ResetTargetter()
        {
            this.m_TargetsInRange.Clear();
            this.m_CurrrentTargetable = null;

            this.targetEntersRange = null;
            this.targetExitsRange = null;
            this.acquiredTarget = null;
            this.lostTarget = null;

            // Reset turret facing
            if (this.turret != null)
            {
                this.turret.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Returns all the targets within the collider. This list must not be changed as it is the working
        /// list of the targetter. Changing it could break the targetter
        /// </summary>
        public List<Targetable> GetAllTargets()
        {
            return this.m_TargetsInRange;
        }

        /// <summary>
        /// Checks if the targetable is a valid target
        /// </summary>
        /// <param name="targetable"></param>
        /// <returns>true if targetable is vaild, false if not</returns>
        protected virtual bool IsTargetableValid(Targetable targetable)
        {
            if (targetable == null)
            {
                return false;
            }

            IAlignmentProvider targetAlignment = targetable.configuration.alignmentProvider;
            bool canDamage = this.alignment == null || targetAlignment == null ||
                             this.alignment.CanHarm(targetAlignment);

            return canDamage;
        }

        /// <summary>
        /// On exiting the trigger, a valid targetable is removed from the tracking list.
        /// </summary>
        /// <param name="other">The other collider in the collision</param>
        protected virtual void OnTriggerExit(Collider other)
        {
            var targetable = other.GetComponent<Targetable>();
            if (!this.IsTargetableValid(targetable))
            {
                return;
            }

            this.m_TargetsInRange.Remove(targetable);
            if (this.targetExitsRange != null)
            {
                this.targetExitsRange(targetable);
            }

            if (targetable == this.m_CurrrentTargetable)
            {
                this.OnTargetRemoved(targetable);
            }
            else
            {
                // Only need to remove if we're not our actual target, otherwise OnTargetRemoved will do the work above
                targetable.removed -= this.OnTargetRemoved;
            }
        }

        /// <summary>
        /// On entering the trigger, a valid targetable is added to the tracking list.
        /// </summary>
        /// <param name="other">The other collider in the collision</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            var targetable = other.GetComponent<Targetable>();
            if (!this.IsTargetableValid(targetable))
            {
                return;
            }

            targetable.removed += this.OnTargetRemoved;
            this.m_TargetsInRange.Add(targetable);
            if (this.targetEntersRange != null)
            {
                this.targetEntersRange(targetable);
            }
        }

        /// <summary>
        /// Returns the nearest targetable within the currently tracked targetables
        /// </summary>
        /// <returns>The nearest targetable if there is one, null otherwise</returns>
        protected virtual Targetable GetNearestTargetable()
        {
            int length = this.m_TargetsInRange.Count;

            if (length == 0)
            {
                return null;
            }

            Targetable nearest = null;
            float distance = float.MaxValue;
            for (int i = length - 1; i >= 0; i--)
            {
                Targetable targetable = this.m_TargetsInRange[i];
                if (targetable == null || targetable.isDead)
                {
                    this.m_TargetsInRange.RemoveAt(i);
                    continue;
                }

                float currentDistance = Vector3.Distance(this.transform.position, targetable.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    nearest = targetable;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Starts the search timer
        /// </summary>
        protected virtual void Start()
        {
            this.m_SearchTimer = this.searchRate;
            this.m_WaitTimer = this.idleWaitTime;
        }

        /// <summary>
        /// Checks if any targets are destroyed and aquires a new targetable if appropriate
        /// </summary>
        protected virtual void Update()
        {
            this.m_SearchTimer -= Time.deltaTime;

            if (this.m_SearchTimer <= 0.0f && this.m_CurrrentTargetable == null && this.m_TargetsInRange.Count > 0)
            {
                this.m_CurrrentTargetable = this.GetNearestTargetable();
                if (this.m_CurrrentTargetable != null)
                {
                    if (this.acquiredTarget != null)
                    {
                        this.acquiredTarget(this.m_CurrrentTargetable);
                    }

                    this.m_SearchTimer = this.searchRate;
                }
            }

            this.AimTurret();

            this.m_HadTarget = this.m_CurrrentTargetable != null;
        }

        /// <summary>
        /// Fired by the agents died event or when the current target moves out of range,
        /// Fires the lostTarget event.
        /// </summary>
        private void OnTargetRemoved(DamageableBehaviour target)
        {
            target.removed -= this.OnTargetRemoved;
            if (this.m_CurrrentTargetable != null && target.configuration == this.m_CurrrentTargetable.configuration)
            {
                if (this.lostTarget != null)
                {
                    this.lostTarget();
                }

                this.m_HadTarget = false;
                this.m_TargetsInRange.Remove(this.m_CurrrentTargetable);
                this.m_CurrrentTargetable = null;
                this.m_XRotationCorrectionTime = 0.0f;
            }
            else // wasnt the current target, find and remove from targets list
            {
                for (int i = 0; i < this.m_TargetsInRange.Count; i++)
                {
                    if (this.m_TargetsInRange[i].configuration == target.configuration)
                    {
                        this.m_TargetsInRange.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Aims the turret at the current target
        /// </summary>
        protected virtual void AimTurret()
        {
            if (this.turret == null)
            {
                return;
            }

            if (this.m_CurrrentTargetable == null) // do idle rotation
            {
                if (this.m_WaitTimer > 0)
                {
                    this.m_WaitTimer -= Time.deltaTime;
                    if (this.m_WaitTimer <= 0)
                    {
                        this.m_CurrentRotationSpeed = (Random.value * 2 - 1) * this.idleRotationSpeed;
                    }
                }
                else
                {
                    Vector3 euler = this.turret.rotation.eulerAngles;
                    euler.x = Mathf.Lerp(Wrap180(euler.x), 0, this.m_XRotationCorrectionTime);
                    this.m_XRotationCorrectionTime = Mathf.Clamp01((this.m_XRotationCorrectionTime + Time.deltaTime) / this.idleCorrectionTime);
                    euler.y += this.m_CurrentRotationSpeed * Time.deltaTime;

                    this.turret.eulerAngles = euler;
                }
            }
            else
            {
                this.m_WaitTimer = this.idleWaitTime;

                Vector3 targetPosition = this.m_CurrrentTargetable.position;
                if (this.onlyYTurretRotation)
                {
                    targetPosition.y = this.turret.position.y;
                }

                Vector3 direction = targetPosition - this.turret.position;
                Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
                Vector3 lookEuler = look.eulerAngles;
                // We need to convert the rotation to a -180/180 wrap so that we can clamp the angle with a min/max
                float x = Wrap180(lookEuler.x);
                lookEuler.x = Mathf.Clamp(x, this.turretXRotationRange.x, this.turretXRotationRange.y);
                look.eulerAngles = lookEuler;
                this.turret.rotation = look;
            }
        }

        /// <summary>
        /// A simply function to convert an angle to a -180/180 wrap
        /// </summary>
        private static float Wrap180(float angle)
        {
            angle %= 360;
            if (angle < -180)
            {
                angle += 360;
            }
            else if (angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }
    }
}