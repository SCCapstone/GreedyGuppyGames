// <copyright file="CurrencyUI.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Economy;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// A class for controlling the displaying the currency
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        /// <summary>
        /// The text element to display information on
        /// </summary>
        public Text display;

        /// <summary>
        /// The currency prefix to display next to the amount
        /// </summary>
        public string currencySymbol = "$";

        protected Currency m_Currency;

        /// <summary>
        /// Assign the correct currency value
        /// </summary>
        protected virtual void Start()
        {
            if (LevelManager.instance != null)
            {
                this.m_Currency = LevelManager.instance.currency;

                this.UpdateDisplay();
                this.m_Currency.currencyChanged += this.UpdateDisplay;
            }
            else
            {
                Debug.LogError("[UI] No level manager to get currency from");
            }
        }

        /// <summary>
        /// Unsubscribe from events
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (this.m_Currency != null)
            {
                this.m_Currency.currencyChanged -= this.UpdateDisplay;
            }
        }

        /// <summary>
        /// A method for updating the display based on the current currency
        /// </summary>
        protected void UpdateDisplay()
        {
            int current = this.m_Currency.currentCurrency;
            this.display.text = current.ToString();
        }
    }
}