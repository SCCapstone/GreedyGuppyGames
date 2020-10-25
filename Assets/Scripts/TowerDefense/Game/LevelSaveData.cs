// <copyright file="LevelSaveData.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;

namespace TowerDefense.Game
{
    /// <summary>
    /// A calss to save level data
    /// </summary>
    [Serializable]
    public class LevelSaveData
    {
        public string id;
        public int numberOfStars;

        public LevelSaveData(string levelId, int numberOfStarsEarned)
        {
            this.id = levelId;
            this.numberOfStars = numberOfStarsEarned;
        }
    }
}