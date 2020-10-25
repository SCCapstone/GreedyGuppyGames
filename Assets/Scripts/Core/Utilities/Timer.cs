// <copyright file="Timer.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// A timer data model. Consumed/process by the TimedBehaviour
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Event fired on elapsing
        /// </summary>
        private readonly Action m_Callback;

        /// <summary>
        /// The time
        /// </summary>
        private float m_Time, m_CurrentTime;

        /// <summary>
        /// Normalized progress of the timer
        /// </summary>
        public float normalizedProgress
        {
            get { return Mathf.Clamp(this.m_CurrentTime / this.m_Time, 0f, 1f); }
        }

        /// <summary>
        /// Timer constructor
        /// </summary>
        /// <param name="newTime">the time that timer is counting</param>
        /// <param name="onElapsed">the event fired at the end of the timer elapsing</param>
        public Timer(float newTime, Action onElapsed = null)
        {
            this.SetTime(newTime);

            this.m_CurrentTime = 0f;
            this.m_Callback += onElapsed;
        }

        /// <summary>
        /// Returns the result of AssessTime
        /// </summary>
        /// <param name="deltaTime">change in time between ticks</param>
        /// <returns>true if the timer has elapsed, false otherwise</returns>
        public virtual bool Tick(float deltaTime)
        {
            return this.AssessTime(deltaTime);
        }

        /// <summary>
        /// Checks if the time has elapsed and fires the tick event
        /// </summary>
        /// <param name="deltaTime">the change in time between assessments</param>
        /// <returns>true if the timer has elapsed, false otherwise</returns>
        protected bool AssessTime(float deltaTime)
        {
            this.m_CurrentTime += deltaTime;
            if (this.m_CurrentTime >= this.m_Time)
            {
                this.FireEvent();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets the current time to 0
        /// </summary>
        public void Reset()
        {
            this.m_CurrentTime = 0;
        }

        /// <summary>
        /// Fires the associated timer event
        /// </summary>
        public void FireEvent()
        {
            this.m_Callback.Invoke();
        }

        /// <summary>
        /// Sets the elapsed time
        /// </summary>
        /// <param name="newTime">sets the time to a new value</param>
        public void SetTime(float newTime)
        {
            this.m_Time = newTime;

            if (newTime <= 0)
            {
                this.m_Time = 0.1f;
            }
        }
    }
}