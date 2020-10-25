// <copyright file="SelfDestroyTimer.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense.Towers
{
    /// <summary>
    /// A helper component for self destruction
    /// </summary>
    public class SelfDestroyTimer : MonoBehaviour
    {
        /// <summary>
        /// The time before destruction
        /// </summary>
        public float time = 5;

        /// <summary>
        /// The controlling timer
        /// </summary>
        public Timer timer;

        /// <summary>
        /// The exposed death callback
        /// </summary>
        public UnityEvent death;

        /// <summary>
        /// Potentially initialize the time if necessary
        /// </summary>
        protected virtual void OnEnable()
        {
            if (this.timer == null)
            {
                this.timer = new Timer(this.time, this.OnTimeEnd);
            }
            else
            {
                this.timer.Reset();
            }
        }

        /// <summary>
        /// Update the timer
        /// </summary>
        protected virtual void Update()
        {
            if (this.timer == null)
            {
                return;
            }

            this.timer.Tick(Time.deltaTime);
        }

        /// <summary>
        /// Fires at the end of timer
        /// </summary>
        protected virtual void OnTimeEnd()
        {
            this.death.Invoke();
            Poolable.TryPool(this.gameObject);
            this.timer.Reset();
        }
    }
}