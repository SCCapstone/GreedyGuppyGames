// <copyright file="TowerLibrary.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense.Towers.Data
{
    /// <summary>
    /// The asset which holds the list of different towers
    /// </summary>
    [CreateAssetMenu(fileName = "TowerLibrary.asset", menuName = "TowerDefense/Tower Library", order = 1)]
    public class TowerLibrary : ScriptableObject, IList<Tower>, IDictionary<string, Tower>
    {
        /// <summary>
        /// The list of all the towers
        /// </summary>
        public List<Tower> configurations;

        /// <summary>
        /// The internal reference to the dictionary made from the list of towers
        /// with the name of tower as the key
        /// </summary>
        private Dictionary<string, Tower> m_ConfigurationDictionary;

        /// <summary>
        /// The accessor to the towers by index
        /// </summary>
        /// <param name="index"></param>
        public Tower this[int index]
        {
            get { return this.configurations[index]; }
        }

        public void OnBeforeSerialize()
        {
        }

        /// <summary>
        /// Convert the list (m_Configurations) to a dictionary for access via name
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (this.configurations == null)
            {
                return;
            }

            this.m_ConfigurationDictionary = this.configurations.ToDictionary(t => t.towerName);
        }

        public bool ContainsKey(string key)
        {
            return this.m_ConfigurationDictionary.ContainsKey(key);
        }

        public void Add(string key, Tower value)
        {
            this.m_ConfigurationDictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return this.m_ConfigurationDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out Tower value)
        {
            return this.m_ConfigurationDictionary.TryGetValue(key, out value);
        }

        Tower IDictionary<string, Tower>.this[string key]
        {
            get { return this.m_ConfigurationDictionary[key]; }
            set { this.m_ConfigurationDictionary[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return ((IDictionary<string, Tower>)this.m_ConfigurationDictionary).Keys; }
        }

        ICollection<Tower> IDictionary<string, Tower>.Values
        {
            get { return this.m_ConfigurationDictionary.Values; }
        }

        IEnumerator<KeyValuePair<string, Tower>> IEnumerable<KeyValuePair<string, Tower>>.GetEnumerator()
        {
            return this.m_ConfigurationDictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<string, Tower> item)
        {
            this.m_ConfigurationDictionary.Add(item.Key, item.Value);
        }

        public bool Remove(KeyValuePair<string, Tower> item)
        {
            return this.m_ConfigurationDictionary.Remove(item.Key);
        }

        public bool Contains(KeyValuePair<string, Tower> item)
        {
            return this.m_ConfigurationDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Tower>[] array, int arrayIndex)
        {
            int count = array.Length;
            for (int i = arrayIndex; i < count; i++)
            {
                Tower config = this.configurations[i - arrayIndex];
                KeyValuePair<string, Tower> current = new KeyValuePair<string, Tower>(config.towerName, config);
                array[i] = current;
            }
        }

        public int IndexOf(Tower item)
        {
            return this.configurations.IndexOf(item);
        }

        public void Insert(int index, Tower item)
        {
            this.configurations.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.configurations.RemoveAt(index);
        }

        Tower IList<Tower>.this[int index]
        {
            get { return this.configurations[index]; }
            set { this.configurations[index] = value; }
        }

        public IEnumerator<Tower> GetEnumerator()
        {
            return this.configurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.configurations).GetEnumerator();
        }

        public void Add(Tower item)
        {
            this.configurations.Add(item);
        }

        public void Clear()
        {
            this.configurations.Clear();
        }

        public bool Contains(Tower item)
        {
            return this.configurations.Contains(item);
        }

        public void CopyTo(Tower[] array, int arrayIndex)
        {
            this.configurations.CopyTo(array, arrayIndex);
        }

        public bool Remove(Tower item)
        {
            return this.configurations.Remove(item);
        }

        public int Count
        {
            get { return this.configurations.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<Tower>)this.configurations).IsReadOnly; }
        }
    }
}