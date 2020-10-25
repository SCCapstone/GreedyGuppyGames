// <copyright file="IDataStore.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace Core.Data
{
    /// <summary>
    /// Interface for data store
    /// </summary>
    public interface IDataStore
    {
        void PreSave();

        void PostLoad();
    }
}