// <copyright file="HealthVisualizer.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace Core.Health
{
    /// <summary>
    /// Class to visualizer the health of a damageable
    /// </summary>
    public class HealthVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The DamageableBehaviour that will be used to assign the damageable
        /// </summary>
        [Tooltip("This field does not need to be populated here, it can be set up in code using AssignDamageable")]
        public DamageableBehaviour damageableBehaviour;

        /// <summary>
        /// The object whose X-scale we change to decrease the health bar. Should have a default uniform scale
        /// </summary>
        public Transform healthBar;

        /// <summary>
        /// The object whose X-scale we change to increase the health bar background. Should have a default uniform scale
        /// </summary>
        public Transform backgroundBar;

        /// <summary>
        /// Whether to show this health bar even when it is full
        /// </summary>
        public bool showWhenFull;

        /// <summary>
        /// Camera to face the visualization at
        /// </summary>
        protected Transform m_CameraToFace;

        /// <summary>
        /// Damageable whose health is visualized
        /// </summary>
        protected Damageable m_Damageable;

        /// <summary>
        /// Updates the visualization of the health
        /// </summary>
        /// <param name="normalizedHealth">Normalized health value</param>
        public void UpdateHealth(float normalizedHealth)
        {
            Vector3 scale = Vector3.one;

            if (this.healthBar != null)
            {
                scale.x = normalizedHealth;
                this.healthBar.transform.localScale = scale;
            }

            if (this.backgroundBar != null)
            {
                scale.x = 1 - normalizedHealth;
                this.backgroundBar.transform.localScale = scale;
            }

            this.SetVisible(this.showWhenFull || normalizedHealth < 1.0f);
        }

        /// <summary>
        /// Sets the visibility status of this visualiser
        /// </summary>
        public void SetVisible(bool visible)
        {
            this.gameObject.SetActive(visible);
        }

        /// <summary>
        /// Assigns the damageable, subscribing to the damaged event
        /// </summary>
        /// <param name="damageable">Damageable to assign</param>
        public void AssignDamageable(Damageable damageable)
        {
            if (this.m_Damageable != null)
            {
                this.m_Damageable.healthChanged -= this.OnHealthChanged;
            }

            this.m_Damageable = damageable;
            this.m_Damageable.healthChanged += this.OnHealthChanged;
        }

        /// <summary>
        /// Turns us to face the camera
        /// </summary>
        protected virtual void Update()
        {
            Vector3 direction = this.m_CameraToFace.transform.forward;
            this.transform.forward = -direction;
        }

        /// <summary>
        /// Assigns a damageable if damageableBehaviour is populated
        /// </summary>
        protected virtual void Awake()
        {
            if (this.damageableBehaviour != null)
            {
                this.AssignDamageable(this.damageableBehaviour.configuration);
            }
        }

        /// <summary>
        /// Caches the main camera
        /// </summary>
        protected virtual void Start()
        {
            this.m_CameraToFace = UnityEngine.Camera.main.transform;
        }

        private void OnHealthChanged(HealthChangeInfo healthChangeInfo)
        {
            this.UpdateHealth(this.m_Damageable.normalisedHealth);
        }
    }
}