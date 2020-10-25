// <copyright file="LevelManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Economy;
using Core.Health;
using Core.Utilities;
using TowerDefense.Economy;
using TowerDefense.Towers.Data;
using UnityEngine;

namespace TowerDefense.Level
{
    /// <summary>
    /// The level manager - handles the level states and tracks the player's currency
    /// </summary>
    [RequireComponent(typeof(WaveManager))]
    public class LevelManager : Singleton<LevelManager>
    {
        /// <summary>
        /// The configured level intro. If this is null the LevelManager will fall through to the gameplay state (i.e. SpawningEnemies)
        /// </summary>
        public LevelIntro intro;

        /// <summary>
        /// The tower library for this level
        /// </summary>
        public TowerLibrary towerLibrary;

        /// <summary>
        /// The currency that the player starts with
        /// </summary>
        public int startingCurrency;

        /// <summary>
        /// The controller for gaining currency
        /// </summary>
        public CurrencyGainer currencyGainer;

        /// <summary>
        /// Configuration for if the player gains currency even in pre-build phase
        /// </summary>
        [Header("Setting this will allow currency gain during the Intro and Pre-Build phase")]
        public bool alwaysGainCurrency;

        /// <summary>
        /// The home bases that the player must defend
        /// </summary>
        public PlayerHomeBase[] homeBases;

        public Collider[] environmentColliders;

        /// <summary>
        /// The attached wave manager
        /// </summary>
        public WaveManager waveManager { get; protected set; }

        /// <summary>
        /// Number of enemies currently in the level
        /// </summary>
        public int numberOfEnemies { get; protected set; }

        /// <summary>
        /// The current state of the level
        /// </summary>
        public LevelState levelState { get; protected set; }

        /// <summary>
        /// The currency controller
        /// </summary>
        public Currency currency { get; protected set; }

        /// <summary>
        /// Number of home bases left
        /// </summary>
        public int numberOfHomeBasesLeft { get; protected set; }

        /// <summary>
        /// Starting number of home bases
        /// </summary>
        public int numberOfHomeBases { get; protected set; }

        /// <summary>
        /// An accessor for the home bases
        /// </summary>
        public PlayerHomeBase[] playerHomeBases
        {
            get { return this.homeBases; }
        }

        /// <summary>
        /// If the game is over
        /// </summary>
        public bool isGameOver
        {
            get { return (this.levelState == LevelState.Win) || (this.levelState == LevelState.Lose); }
        }

        /// <summary>
        /// Fired when all the waves are done and there are no more enemies left
        /// </summary>
        public event Action levelCompleted;

        /// <summary>
        /// Fired when all of the home bases are destroyed
        /// </summary>
        public event Action levelFailed;

        /// <summary>
        /// Fired when the level state is changed - first parameter is the old state, second parameter is the new state
        /// </summary>
        public event Action<LevelState, LevelState> levelStateChanged;

        /// <summary>
        /// Fired when the number of enemies has changed
        /// </summary>
        public event Action<int> numberOfEnemiesChanged;

        /// <summary>
        /// Event for home base being destroyed
        /// </summary>
        public event Action homeBaseDestroyed;

        /// <summary>
        /// Increments the number of enemies. Called on Agent spawn
        /// </summary>
        public virtual void IncrementNumberOfEnemies()
        {
            this.numberOfEnemies++;
            this.SafelyCallNumberOfEnemiesChanged();
        }

        /// <summary>
        /// Returns the sum of all HomeBases' health
        /// </summary>
        public float GetAllHomeBasesHealth()
        {
            float health = 0.0f;
            foreach (PlayerHomeBase homebase in this.homeBases)
            {
                health += homebase.configuration.currentHealth;
            }

            return health;
        }

        /// <summary>
        /// Decrements the number of enemies. Called on Agent death
        /// </summary>
        public virtual void DecrementNumberOfEnemies()
        {
            this.numberOfEnemies--;
            this.SafelyCallNumberOfEnemiesChanged();
            if (this.numberOfEnemies < 0)
            {
                Debug.LogError("[LEVEL] There should never be a negative number of enemies. Something broke!");
                this.numberOfEnemies = 0;
            }

            if (this.numberOfEnemies == 0 && this.levelState == LevelState.AllEnemiesSpawned)
            {
                this.ChangeLevelState(LevelState.Win);
            }
        }

        /// <summary>
        /// Completes building phase, setting state to spawn enemies
        /// </summary>
        public virtual void BuildingCompleted()
        {
            this.ChangeLevelState(LevelState.SpawningEnemies);
        }

        /// <summary>
        /// Caches the attached wave manager and subscribes to the spawning completed event
        /// Sets the level state to intro and ensures that the number of enemies is set to 0
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            this.waveManager = this.GetComponent<WaveManager>();
            this.waveManager.spawningCompleted += this.OnSpawningCompleted;

            // Does not use the change state function as we don't need to broadcast the event for this default value
            this.levelState = LevelState.Intro;
            this.numberOfEnemies = 0;

            // Ensure currency change listener is assigned
            this.currency = new Currency(this.startingCurrency);
            this.currencyGainer.Initialize(this.currency);

            // If there's an intro use it, otherwise fall through to gameplay
            if (this.intro != null)
            {
                this.intro.introCompleted += this.IntroCompleted;
            }
            else
            {
                this.IntroCompleted();
            }

            // Iterate through home bases and subscribe
            this.numberOfHomeBases = this.homeBases.Length;
            this.numberOfHomeBasesLeft = this.numberOfHomeBases;
            for (int i = 0; i < this.numberOfHomeBases; i++)
            {
                this.homeBases[i].died += this.OnHomeBaseDestroyed;
            }
        }

        /// <summary>
        /// Updates the currency gain controller
        /// </summary>
        protected virtual void Update()
        {
            if (this.alwaysGainCurrency ||
                (!this.alwaysGainCurrency && this.levelState != LevelState.Building && this.levelState != LevelState.Intro))
            {
                this.currencyGainer.Tick(Time.deltaTime);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.waveManager != null)
            {
                this.waveManager.spawningCompleted -= this.OnSpawningCompleted;
            }

            if (this.intro != null)
            {
                this.intro.introCompleted -= this.IntroCompleted;
            }

            // Iterate through home bases and unsubscribe
            for (int i = 0; i < this.numberOfHomeBases; i++)
            {
                this.homeBases[i].died -= this.OnHomeBaseDestroyed;
            }
        }

        /// <summary>
        /// Fired when Intro is completed or immediately, if no intro is specified
        /// </summary>
        protected virtual void IntroCompleted()
        {
            this.ChangeLevelState(LevelState.Building);
        }

        /// <summary>
        /// Fired when the WaveManager has finished spawning enemies
        /// </summary>
        protected virtual void OnSpawningCompleted()
        {
            this.ChangeLevelState(LevelState.AllEnemiesSpawned);
        }

        /// <summary>
        /// Changes the state and broadcasts the event
        /// </summary>
        /// <param name="newState">The new state to transitioned to</param>
        protected virtual void ChangeLevelState(LevelState newState)
        {
            // If the state hasn't changed then return
            if (this.levelState == newState)
            {
                return;
            }

            LevelState oldState = this.levelState;
            this.levelState = newState;
            if (this.levelStateChanged != null)
            {
                this.levelStateChanged(oldState, newState);
            }

            switch (newState)
            {
                case LevelState.SpawningEnemies:
                    this.waveManager.StartWaves();
                    break;
                case LevelState.AllEnemiesSpawned:
                    // Win immediately if all enemies are already dead
                    if (this.numberOfEnemies == 0)
                    {
                        this.ChangeLevelState(LevelState.Win);
                    }

                    break;
                case LevelState.Lose:
                    this.SafelyCallLevelFailed();
                    break;
                case LevelState.Win:
                    this.SafelyCallLevelCompleted();
                    break;
            }
        }

        /// <summary>
        /// Fired when a home base is destroyed
        /// </summary>
        protected virtual void OnHomeBaseDestroyed(DamageableBehaviour homeBase)
        {
            // Decrement the number of home bases
            this.numberOfHomeBasesLeft--;

            // Call the destroyed event
            if (this.homeBaseDestroyed != null)
            {
                this.homeBaseDestroyed();
            }

            // If there are no home bases left and the level is not over then set the level to lost
            if ((this.numberOfHomeBasesLeft == 0) && !this.isGameOver)
            {
                this.ChangeLevelState(LevelState.Lose);
            }
        }

        /// <summary>
        /// Calls the <see cref="levelCompleted"/> event
        /// </summary>
        protected virtual void SafelyCallLevelCompleted()
        {
            if (this.levelCompleted != null)
            {
                this.levelCompleted();
            }
        }

        /// <summary>
        /// Calls the <see cref="numberOfEnemiesChanged"/> event
        /// </summary>
        protected virtual void SafelyCallNumberOfEnemiesChanged()
        {
            if (this.numberOfEnemiesChanged != null)
            {
                this.numberOfEnemiesChanged(this.numberOfEnemies);
            }
        }

        /// <summary>
        /// Calls the <see cref="levelFailed"/> event
        /// </summary>
        protected virtual void SafelyCallLevelFailed()
        {
            if (this.levelFailed != null)
            {
                this.levelFailed();
            }
        }
    }
}