// <copyright file="CurrencyChangeInfo.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.Economy
{
    /// <summary>
    /// A struct for holding currency change data
    /// </summary>
    public struct CurrencyChangeInfo
    {
        /// <summary>
        /// The previous value of the currency
        /// </summary>
        public readonly int previousCurrency;

        /// <summary>
        /// The new value of the currency
        /// </summary>
        public readonly int currentCurrency;

        /// <summary>
        /// The difference in amount
        /// </summary>
        public readonly int difference;

        /// <summary>
        /// Gets the absolute difference in amount
        /// </summary>
        public readonly int absoluteDifference;

        /// <summary>
        /// Initializes the CurrencyChangeInfo
        /// </summary>
        /// <param name="previous">
        /// The previous value of the currency
        /// </param>
        /// <param name="current">
        /// The current value of the currency
        /// </param>
        public CurrencyChangeInfo(int previous, int current)
        {
            this.previousCurrency = previous;
            this.currentCurrency = current;
            this.difference = this.currentCurrency - this.previousCurrency;
            this.absoluteDifference = Mathf.Abs(this.difference);
        }
    }
}