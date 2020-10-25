// <copyright file="TowerLevel.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Core.Health;
using TowerDefense.Affectors;
using TowerDefense.Towers.Data;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers
{
    /// <summary>
    /// An individual level of a tower
    /// </summary>
    [DisallowMultipleComponent]
    public class TowerLevel : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The prefab for communicating placement in the scene
        /// </summary>
        public TowerPlacementGhost towerGhostPrefab;

        /// <summary>
        /// Build effect gameObject to instantiate on start
        /// </summary>
        public GameObject buildEffectPrefab;

        /// <summary>
        /// Reference to scriptable object with level data on it
        /// </summary>
        public TowerLevelData levelData;

        /// <summary>
        /// The parent tower controller of this tower
        /// </summary>
        protected Tower m_ParentTower;

        /// <summary>
        /// The list of effects attached to the tower
        /// </summary>
        private Affector[] m_Affectors;

        /// <summary>
        /// Gets the list of effects attached to the tower
        /// </summary>
        protected Affector[] Affectors
        {
            get
            {
                if (this.m_Affectors == null)
                {
                    this.m_Affectors = this.GetComponentsInChildren<Affector>();
                }

                return this.m_Affectors;
            }
        }

        /// <summary>
        /// The physics layer mask that the tower searches on
        /// </summary>
        public LayerMask mask { get; protected set; }

        /// <summary>
        /// Gets the cost value
        /// </summary>
        public int cost
        {
            get { return this.levelData.cost; }
        }

        /// <summary>
        /// Gets the sell value
        /// </summary>
        public int sell
        {
            get { return this.levelData.sell; }
        }

        /// <summary>
        /// Gets the max health
        /// </summary>
        public int maxHealth
        {
            get { return this.levelData.maxHealth; }
        }

        /// <summary>
        /// Gets the starting health
        /// </summary>
        public int startingHealth
        {
            get { return this.levelData.startingHealth; }
        }

        /// <summary>
        /// Gets the tower description
        /// </summary>
        public string description
        {
            get { return this.levelData.description; }
        }

        /// <summary>
        /// Gets the tower description
        /// </summary>
        public string upgradeDescription
        {
            get { return this.levelData.upgradeDescription; }
        }

        /// <summary>
        /// Initialises the Effects attached to this object
        /// </summary>
        public virtual void Initialize(Tower tower, LayerMask enemyMask, IAlignmentProvider alignment)
        {
            this.mask = enemyMask;

            foreach (Affector effect in this.Affectors)
            {
                effect.Initialize(alignment, this.mask);
            }

            this.m_ParentTower = tower;
        }

        /// <summary>
        /// A method for activating or deactivating the attached <see cref="Affectors"/>
        /// </summary>
        public void SetAffectorState(bool state)
        {
            foreach (Affector affector in this.Affectors)
            {
                if (affector != null)
                {
                    affector.enabled = state;
                }
            }
        }

        /// <summary>
        /// Returns a list of affectors that implement ITowerRadiusVisualizer
        /// </summary>
        /// <returns>ITowerRadiusVisualizers of tower</returns>
        public List<ITowerRadiusProvider> GetRadiusVisualizers()
        {
            List<ITowerRadiusProvider> visualizers = new List<ITowerRadiusProvider>();
            foreach (Affector affector in this.Affectors)
            {
                var visualizer = affector as ITowerRadiusProvider;
                if (visualizer != null)
                {
                    visualizers.Add(visualizer);
                }
            }

            return visualizers;
        }

        /// <summary>
        /// Returns the dps of the tower
        /// </summary>
        /// <returns>The dps of the tower</returns>
        public float GetTowerDps()
        {
            float dps = 0;
            foreach (Affector affector in this.Affectors)
            {
                var attack = affector as AttackAffector;
                if (attack != null && attack.damagerProjectile != null)
                {
                    dps += attack.GetProjectileDamage() * attack.fireRate;
                }
            }

            return dps;
        }

        public void Kill()
        {
            this.m_ParentTower.KillTower();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            // Setting this member to null is required because we are setting this value on a prefab which will
            // persists post run in editor, so we null this member to ensure it is repopulated every run
            this.m_Affectors = null;
        }

        /// <summary>
        /// Insntiate the build particle effect object
        /// </summary>
        private void Start()
        {
            if (this.buildEffectPrefab != null)
            {
                Instantiate(this.buildEffectPrefab, this.transform);
            }
        }
    }
}