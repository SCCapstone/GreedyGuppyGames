// <copyright file="IMainMenuPage.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace Core.UI
{
    /// <summary>
    /// Base interface for menu pages
    /// </summary>
    public interface IMainMenuPage
    {
        /// <summary>
        /// Deactivates this page
        /// </summary>
        void Hide();

        /// <summary>
        /// Activates this page
        /// </summary>
        void Show();
    }
}