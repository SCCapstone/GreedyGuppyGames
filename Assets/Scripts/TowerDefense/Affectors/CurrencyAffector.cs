// <copyright file="CurrencyAffector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Economy;
using TowerDefense.Level;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// A tower effect for generating currency
    /// </summary>
    public class CurrencyAffector : Affector
    {
        /// <summary>
        /// The controller for currency gain
        /// </summary>
        public CurrencyGainer currencyGainer;

        /// <summary>
        /// Format for displaying the the properties of this affector
        /// </summary>
        public string descriptionFormat = "<b>Produces</b> {1} at {2} units per second";

        /// <summary>
        /// The audio source attached
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// The attached particle system
        /// </summary>
        public ParticleSystem currencyParticleSystem;

        /// <summary>
        /// Initialize the currency gain
        /// </summary>
        protected virtual void Start()
        {
            this.currencyGainer.Initialize(LevelManager.instance.currency);
        }

        /// <summary>
        /// Update the currency gain
        /// </summary>
        protected virtual void Update()
        {
            this.currencyGainer.Tick(Time.deltaTime);
        }

        /// <summary>
        /// Subscribe to currency gain events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.currencyGainer.currencyChanged += this.OnCurrencyChanged;
        }

        /// <summary>
        /// Unsubscribe to currency gain event
        /// </summary>
        protected virtual void OnDisable()
        {
            this.currencyGainer.currencyChanged -= this.OnCurrencyChanged;
        }

        /// <summary>
        /// Fires when currency changed in <see cref="currencyGainer"/>
        /// </summary>
        /// <param name="info">
        /// The info for the currency gainer
        /// </param>
        protected void OnCurrencyChanged(CurrencyChangeInfo info)
        {
            if (this.audioSource != null)
            {
                this.audioSource.Play();
            }

            if (this.currencyParticleSystem != null)
            {
                this.currencyParticleSystem.Play();
            }
        }
    }
}