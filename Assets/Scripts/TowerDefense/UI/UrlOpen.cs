// <copyright file="UrlOpen.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.UI
{
    /// <summary>
    /// Simple script to open a URL
    /// </summary>
    public class UrlOpen : MonoBehaviour
    {
        /// <summary>
        /// Open the given url
        /// </summary>
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}