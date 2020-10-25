// <copyright file="PoolManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Managers a dictionary of pools, getting and returning
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        /// <summary>
        /// List of poolables that will be used to initialize corresponding pools
        /// </summary>
        public List<Poolable> poolables;

        /// <summary>
        /// Dictionary of pools, key is the prefab
        /// </summary>
        protected Dictionary<Poolable, AutoComponentPrefabPool<Poolable>> m_Pools;

        /// <summary>
        /// Gets a poolable component from the corresponding pool
        /// </summary>
        /// <param name="poolablePrefab"></param>
        /// <returns></returns>
        public Poolable GetPoolable(Poolable poolablePrefab)
        {
            if (!this.m_Pools.ContainsKey(poolablePrefab))
            {
                this.m_Pools.Add(poolablePrefab, new AutoComponentPrefabPool<Poolable>(poolablePrefab, this.Initialize, null,
                                                                                  poolablePrefab.initialPoolCapacity));
            }

            AutoComponentPrefabPool<Poolable> pool = this.m_Pools[poolablePrefab];
            Poolable spawnedInstance = pool.Get();

            spawnedInstance.pool = pool;
            return spawnedInstance;
        }

        /// <summary>
        /// Returns the poolable component to its component pool
        /// </summary>
        /// <param name="poolable"></param>
        public void ReturnPoolable(Poolable poolable)
        {
            poolable.pool.Return(poolable);
        }

        /// <summary>
        /// Initializes the dicionary of pools
        /// </summary>
        protected void Start()
        {
            this.m_Pools = new Dictionary<Poolable, AutoComponentPrefabPool<Poolable>>();

            foreach (var poolable in this.poolables)
            {
                if (poolable == null)
                {
                    continue;
                }

                this.m_Pools.Add(poolable, new AutoComponentPrefabPool<Poolable>(poolable, this.Initialize, null,
                                                                            poolable.initialPoolCapacity));
            }
        }

        private void Initialize(Component poolable)
        {
            poolable.transform.SetParent(this.transform, false);
        }
    }
}