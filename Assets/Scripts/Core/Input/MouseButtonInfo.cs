// <copyright file="MouseButtonInfo.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace Core.Input
{
    /// <summary>
    /// Info for mouse
    /// </summary>
    public class MouseButtonInfo : PointerActionInfo
    {
        /// <summary>
        /// Is this mouse button down
        /// </summary>
        public bool isDown;

        /// <summary>
        /// Our mouse button id
        /// </summary>
        public int mouseButtonId;
    }
}