// <copyright file="IDataSaver.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace Core.Data
{
    /// <summary>
    /// Interface for saving data
    /// </summary>
    public interface IDataSaver<T> where T : IDataStore
    {
        void Save(T data);

        bool Load(out T data);

        void Delete();
    }
}