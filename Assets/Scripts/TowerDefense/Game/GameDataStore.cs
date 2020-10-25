// <copyright file="GameDataStore.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Core.Data;
using UnityEngine;

namespace TowerDefense.Game
{
    /// <summary>
    /// The data store for TD
    /// </summary>
    public sealed class GameDataStore : GameDataStoreBase
    {
        /// <summary>
        /// A list of level IDs for completed levels
        /// </summary>
        public List<LevelSaveData> completedLevels = new List<LevelSaveData>();

        /// <summary>
        /// Outputs to debug
        /// </summary>
        public override void PreSave()
        {
            Debug.Log("[GAME] Saving Game");
        }

        /// <summary>
        /// Outputs to debug
        /// </summary>
        public override void PostLoad()
        {
            Debug.Log("[GAME] Loaded Game");
        }

        /// <summary>
        /// Marks a level complete
        /// </summary>
        /// <param name="levelId">The levelId to mark as complete</param>
        /// <param name="starsEarned">Stars earned</param>
        public void CompleteLevel(string levelId, int starsEarned)
        {
            foreach (LevelSaveData level in this.completedLevels)
            {
                if (level.id == levelId)
                {
                    level.numberOfStars = Mathf.Max(level.numberOfStars, starsEarned);
                    return;
                }
            }

            this.completedLevels.Add(new LevelSaveData(levelId, starsEarned));
        }

        /// <summary>
        /// Determines if a specific level is completed
        /// </summary>
        /// <param name="levelId">The level ID to check</param>
        /// <returns>true if the level is completed</returns>
        public bool IsLevelCompleted(string levelId)
        {
            foreach (LevelSaveData level in this.completedLevels)
            {
                if (level.id == levelId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves the star count for a given level
        /// </summary>
        public int GetNumberOfStarForLevel(string levelId)
        {
            foreach (LevelSaveData level in this.completedLevels)
            {
                if (level.id == levelId)
                {
                    return level.numberOfStars;
                }
            }

            return 0;
        }
    }
}