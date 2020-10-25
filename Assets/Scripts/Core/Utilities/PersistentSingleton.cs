// <copyright file="PersistentSingleton.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace Core.Utilities
{
    /// <summary>
    /// Singleton that persists across multiple scenes
    /// </summary>
    public class PersistentSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
    }
}