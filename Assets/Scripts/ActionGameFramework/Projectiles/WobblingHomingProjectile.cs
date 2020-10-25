// <copyright file="WobblingHomingProjectile.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// A projectile that shoots upwards and then starts homing
    /// </summary>
    public class WobblingHomingProjectile : HomingLinearProjectile
    {
        protected enum State
        {
            Wobbling,
            Turning,
            Targeting
        }

        /// <summary>
        /// The time the projectile wobbles upward is randomized from this range
        /// </summary>
        public Vector2 wobbleTimeRange = new Vector2(1, 2);

        /// <summary>
        /// The number of wobble direction changes per second
        /// </summary>
        public float wobbleDirectionChangeSpeed = 4;

        /// <summary>
        /// The intensity of the wobble
        /// </summary>
        public float wobbleMagnitude = 7;

        /// <summary>
        /// The time the projectile takes to turn and home
        /// </summary>
        public float turningTime = 0.5f;

        /// <summary>
        /// State of projectile
        /// </summary>
        private State m_State;

        /// <summary>
        /// Seconds wobbling
        /// </summary>
        protected float m_CurrentWobbleTime;

        /// <summary>
        /// Total time to wobble
        /// </summary>
        protected float m_WobbleDuration;

        /// <summary>
        /// Seconds turning to face homing target
        /// </summary>
        protected float m_CurrentTurnTime;

        /// <summary>
        /// Seconds for current turn
        /// </summary>
        protected float m_WobbleChangeTime;

        protected Vector3 m_WobbleVector,
                          m_TargetWobbleVector;

        protected override void Update()
        {
            // regular HomingLinearProjectile behaviour, handles a null homing target
            if (this.m_HomingTarget == null || this.m_State == State.Targeting)
            {
                base.Update();
                return;
            }

            switch (this.m_State)
            {
                // wobble the projectile
                case State.Wobbling:
                    this.m_CurrentWobbleTime += Time.deltaTime;
                    if (this.m_CurrentWobbleTime >= this.m_WobbleDuration)
                    {
                        this.m_State = State.Turning;
                        this.m_CurrentTurnTime = 0;
                    }

                    this.m_WobbleChangeTime += Time.deltaTime * this.wobbleDirectionChangeSpeed;
                    if (this.m_WobbleChangeTime >= 1)
                    {
                        this.m_WobbleChangeTime = 0;
                        this.m_TargetWobbleVector = new Vector3(Random.Range(-this.wobbleMagnitude, this.wobbleMagnitude),
                                                           Random.Range(-this.wobbleMagnitude, this.wobbleMagnitude), 0);
                        this.m_WobbleVector = Vector3.zero;
                    }

                    this.m_WobbleVector = Vector3.Lerp(this.m_WobbleVector, this.m_TargetWobbleVector, this.m_WobbleChangeTime);
                    this.m_Rigidbody.velocity = Quaternion.Euler(this.m_WobbleVector) * this.m_Rigidbody.velocity;

                    this.m_Rigidbody.rotation = Quaternion.LookRotation(this.m_Rigidbody.velocity);
                    break;
                // turn the projectile to face the homing target
                case State.Turning:
                    this.m_CurrentTurnTime += Time.deltaTime;
                    Quaternion aimDirection = Quaternion.LookRotation(this.GetHeading());

                    this.m_Rigidbody.rotation = Quaternion.Lerp(this.m_Rigidbody.rotation, aimDirection, this.m_CurrentTurnTime / this.turningTime);
                    this.m_Rigidbody.velocity = this.transform.forward * this.m_Rigidbody.velocity.magnitude;

                    if (this.m_CurrentTurnTime >= this.turningTime)
                    {
                        this.m_State = State.Targeting;
                    }

                    break;
            }
        }

        // select first wobble vector and set to wobble state
        protected override void Fire(Vector3 firingVector)
        {
            this.m_TargetWobbleVector = new Vector3(Random.Range(-this.wobbleMagnitude, this.wobbleMagnitude),
                                               Random.Range(-this.wobbleMagnitude, this.wobbleMagnitude), 0);
            this.m_WobbleDuration = Random.Range(this.wobbleTimeRange.x, this.wobbleTimeRange.y);
            base.Fire(firingVector);
            this.m_State = State.Wobbling;
            this.m_CurrentWobbleTime = 0.0f;
        }
    }
}