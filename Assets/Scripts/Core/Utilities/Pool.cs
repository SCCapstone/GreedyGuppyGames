// <copyright file="Pool.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Utilities
{
    /// <summary>
    /// Maintains a pool of objects
    /// </summary>
    public class Pool<T>
    {
        /// <summary>
        /// Our factory function
        /// </summary>
        protected Func<T> m_Factory;

        /// <summary>
        /// Our resetting function
        /// </summary>
        protected readonly Action<T> m_Reset;

        /// <summary>
        /// A list of all m_Available items
        /// </summary>
        protected readonly List<T> m_Available;

        /// <summary>
        /// A list of all items managed by the pool
        /// </summary>
        protected readonly List<T> m_All;

        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="reset">Function to use to m_Reset items when retrieving from the pool</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public Pool(Func<T> factory, Action<T> reset, int initialCapacity)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            this.m_Available = new List<T>();
            this.m_All = new List<T>();
            this.m_Factory = factory;
            this.m_Reset = reset;

            if (initialCapacity > 0)
            {
                this.Grow(initialCapacity);
            }
        }

        /// <summary>
        /// Creates a new blank pool
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        public Pool(Func<T> factory)
            : this(factory, null, 0)
        {
        }

        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public Pool(Func<T> factory, int initialCapacity)
            : this(factory, null, initialCapacity)
        {
        }

        /// <summary>
        /// Gets an item from the pool, growing it if necessary
        /// </summary>
        /// <returns></returns>
        public virtual T Get()
        {
            return this.Get(this.m_Reset);
        }

        /// <summary>
        /// Gets an item from the pool, growing it if necessary, and with a specified m_Reset function
        /// </summary>
        /// <param name="resetOverride">A function to use to m_Reset the given object</param>
        public virtual T Get(Action<T> resetOverride)
        {
            if (this.m_Available.Count == 0)
            {
                this.Grow(1);
            }

            if (this.m_Available.Count == 0)
            {
                throw new InvalidOperationException("Failed to grow pool");
            }

            int itemIndex = this.m_Available.Count - 1;
            T item = this.m_Available[itemIndex];
            this.m_Available.RemoveAt(itemIndex);

            if (resetOverride != null)
            {
                resetOverride(item);
            }

            return item;
        }

        /// <summary>
        /// Gets whether or not this pool contains a specified item
        /// </summary>
        public virtual bool Contains(T pooledItem)
        {
            return this.m_All.Contains(pooledItem);
        }

        /// <summary>
        /// Return an item to the pool
        /// </summary>
        public virtual void Return(T pooledItem)
        {
            if (this.m_All.Contains(pooledItem) &&
                !this.m_Available.Contains(pooledItem))
            {
                this.ReturnToPoolInternal(pooledItem);
            }
            else
            {
                throw new InvalidOperationException("Trying to return an item to a pool that does not contain it: " + pooledItem +
                                                    ", " + this);
            }
        }

        /// <summary>
        /// Return all items to the pool
        /// </summary>
        public virtual void ReturnAll()
        {
            this.ReturnAll(null);
        }

        /// <summary>
        /// Returns all items to the pool, and calls a delegate on each one
        /// </summary>
        public virtual void ReturnAll(Action<T> preReturn)
        {
            for (int i = 0; i < this.m_All.Count; ++i)
            {
                T item = this.m_All[i];
                if (!this.m_Available.Contains(item))
                {
                    if (preReturn != null)
                    {
                        preReturn(item);
                    }

                    this.ReturnToPoolInternal(item);
                }
            }
        }

        /// <summary>
        /// Grow the pool by a given number of elements
        /// </summary>
        public void Grow(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                this.AddNewElement();
            }
        }

        /// <summary>
        /// Returns an object to the m_Available list. Does not check for consistency
        /// </summary>
        protected virtual void ReturnToPoolInternal(T element)
        {
            this.m_Available.Add(element);
        }

        /// <summary>
        /// Adds a new element to the pool
        /// </summary>
        protected virtual T AddNewElement()
        {
            T newElement = this.m_Factory();
            this.m_All.Add(newElement);
            this.m_Available.Add(newElement);

            return newElement;
        }

        /// <summary>
        /// Dummy factory that returns the default T value
        /// </summary>
        protected static T DummyFactory()
        {
            return default(T);
        }
    }

    /// <summary>
    /// A variant pool that takes Unity components. Automatically enables and disables them as necessary
    /// </summary>
    public class UnityComponentPool<T> : Pool<T>
        where T : Component
    {
        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public UnityComponentPool(Func<T> factory, Action<T> reset, int initialCapacity)
            : base(factory, reset, initialCapacity)
        {
        }

        /// <summary>
        /// Creates a new blank pool
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        public UnityComponentPool(Func<T> factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public UnityComponentPool(Func<T> factory, int initialCapacity)
            : base(factory, initialCapacity)
        {
        }

        /// <summary>
        /// Retrieve an enabled element from the pool
        /// </summary>
        public override T Get(Action<T> resetOverride)
        {
            T element = base.Get(resetOverride);

            element.gameObject.SetActive(true);

            return element;
        }

        /// <summary>
        /// Automatically disable returned object
        /// </summary>
        protected override void ReturnToPoolInternal(T element)
        {
            element.gameObject.SetActive(false);

            base.ReturnToPoolInternal(element);
        }

        /// <summary>
        /// Keep newly created objects disabled
        /// </summary>
        protected override T AddNewElement()
        {
            T newElement = base.AddNewElement();

            newElement.gameObject.SetActive(false);

            return newElement;
        }
    }

    /// <summary>
    /// A variant pool that takes Unity game objects. Automatically enables and disables them as necessary
    /// </summary>
    public class GameObjectPool : Pool<GameObject>
    {
        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public GameObjectPool(Func<GameObject> factory, Action<GameObject> reset, int initialCapacity)
            : base(factory, reset, initialCapacity)
        {
        }

        /// <summary>
        /// Creates a new blank pool
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        public GameObjectPool(Func<GameObject> factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Create a new pool with a given number of starting elements
        /// </summary>
        /// <param name="factory">The function that creates pool objects</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public GameObjectPool(Func<GameObject> factory, int initialCapacity)
            : base(factory, initialCapacity)
        {
        }

        /// <summary>
        /// Retrieve an enabled element from the pool
        /// </summary>
        public override GameObject Get(Action<GameObject> resetOverride)
        {
            GameObject element = base.Get(resetOverride);

            element.SetActive(true);

            return element;
        }

        /// <summary>
        /// Automatically disable returned object
        /// </summary>
        protected override void ReturnToPoolInternal(GameObject element)
        {
            element.SetActive(false);

            base.ReturnToPoolInternal(element);
        }

        /// <summary>
        /// Keep newly created objects disabled
        /// </summary>
        protected override GameObject AddNewElement()
        {
            GameObject newElement = base.AddNewElement();

            newElement.SetActive(false);

            return newElement;
        }
    }

    /// <summary>
    /// Variant pool that automatically instantiates objects from a given Unity gameobject prefab
    /// </summary>
    public class AutoGameObjectPrefabPool : GameObjectPool
    {
        /// <summary>
        /// Create our new prefab item clone
        /// </summary>
        private GameObject PrefabFactory()
        {
            GameObject newElement = Object.Instantiate(this.m_Prefab);
            if (this.m_Initialize != null)
            {
                this.m_Initialize(newElement);
            }

            return newElement;
        }

        /// <summary>
        /// Our base prefab
        /// </summary>
        protected readonly GameObject m_Prefab;

        /// <summary>
        /// Initialisation method for objects
        /// </summary>
        protected readonly Action<GameObject> m_Initialize;

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        public AutoGameObjectPrefabPool(GameObject prefab)
            : this(prefab, null, null, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize)
            : this(prefab, initialize, null, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize, Action<GameObject> reset)
            : this(prefab, initialize, reset, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab with a given number of starting elements
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public AutoGameObjectPrefabPool(GameObject prefab, int initialCapacity)
            : this(prefab, null, null, initialCapacity)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public AutoGameObjectPrefabPool(GameObject prefab, Action<GameObject> initialize, Action<GameObject> reset,
                                        int initialCapacity)
            : base(DummyFactory, reset, 0)
        {
            // Pass 0 to initial capacity because we need to set ourselves up first
            // We then call Grow again ourselves
            this.m_Initialize = initialize;
            this.m_Prefab = prefab;
            this.m_Factory = this.PrefabFactory;
            if (initialCapacity > 0)
            {
                this.Grow(initialCapacity);
            }
        }
    }

    /// <summary>
    /// Variant pool that automatically instantiates objects from a given Unity component prefab
    /// </summary>
    public class AutoComponentPrefabPool<T> : UnityComponentPool<T>
        where T : Component
    {
        /// <summary>
        /// Create our new prefab item clone
        /// </summary>
        private T PrefabFactory()
        {
            T newElement = Object.Instantiate(this.m_Prefab);
            if (this.m_Initialize != null)
            {
                this.m_Initialize(newElement);
            }

            return newElement;
        }

        /// <summary>
        /// Our base prefab
        /// </summary>
        protected readonly T m_Prefab;

        /// <summary>
        /// Initialisation method for objects
        /// </summary>
        protected readonly Action<T> m_Initialize;

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        public AutoComponentPrefabPool(T prefab)
            : this(prefab, null, null, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        public AutoComponentPrefabPool(T prefab, Action<T> initialize)
            : this(prefab, initialize, null, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        public AutoComponentPrefabPool(T prefab, Action<T> initialize, Action<T> reset)
            : this(prefab, initialize, reset, 0)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab with a given number of starting elements
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public AutoComponentPrefabPool(T prefab, int initialCapacity)
            : this(prefab, null, null, initialCapacity)
        {
        }

        /// <summary>
        /// Create a new pool for the given Unity prefab
        /// </summary>
        /// <param name="prefab">The prefab we're cloning</param>
        /// <param name="initialize">An initialisation function to call after creating prefabs</param>
        /// <param name="reset">Function to use to reset items when retrieving from the pool</param>
        /// <param name="initialCapacity">The number of elements to seed the pool with</param>
        public AutoComponentPrefabPool(T prefab, Action<T> initialize, Action<T> reset, int initialCapacity)
            : base(DummyFactory, reset, 0)
        {
            // Pass 0 to initial capacity because we need to set ourselves up first
            // We then call Grow again ourselves
            this.m_Initialize = initialize;
            this.m_Prefab = prefab;
            this.m_Factory = this.PrefabFactory;
            if (initialCapacity > 0)
            {
                this.Grow(initialCapacity);
            }
        }
    }
}