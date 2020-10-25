// <copyright file="GameManagerBase.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Data
{
    /// <summary>
    /// Base game manager
    /// </summary>
    public abstract class GameManagerBase<TGameManager, TDataStore> : PersistentSingleton<TGameManager>
        where TDataStore : GameDataStoreBase, new()
        where TGameManager : GameManagerBase<TGameManager, TDataStore>
    {
        /// <summary>
        /// File name of saved game
        /// </summary>
        private const string k_SavedGameFile = "save";

        /// <summary>
        /// Reference to audio mixer for volume changing
        /// </summary>
        public AudioMixer gameMixer;

        /// <summary>
        /// Master volume parameter on the mixer
        /// </summary>
        public string masterVolumeParameter;

        /// <summary>
        /// SFX volume parameter on the mixer
        /// </summary>
        public string sfxVolumeParameter;

        /// <summary>
        /// Music volume parameter on the mixer
        /// </summary>
        public string musicVolumeParameter;

        /// <summary>
        /// The serialization implementation for persistence
        /// </summary>
        protected JsonSaver<TDataStore> m_DataSaver;

        /// <summary>
        /// The object used for persistence
        /// </summary>
        protected TDataStore m_DataStore;

        /// <summary>
        /// Retrieve volumes from data store
        /// </summary>
        public virtual void GetVolumes(out float master, out float sfx, out float music)
        {
            master = this.m_DataStore.masterVolume;
            sfx = this.m_DataStore.sfxVolume;
            music = this.m_DataStore.musicVolume;
        }

        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public virtual void SetVolumes(float master, float sfx, float music, bool save)
        {
            // Early out if no mixer set
            if (this.gameMixer == null)
            {
                return;
            }

            // Transform 0-1 into logarithmic -80-0
            if (this.masterVolumeParameter != null)
            {
                this.gameMixer.SetFloat(this.masterVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(master)));
            }

            if (this.sfxVolumeParameter != null)
            {
                this.gameMixer.SetFloat(this.sfxVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(sfx)));
            }

            if (this.musicVolumeParameter != null)
            {
                this.gameMixer.SetFloat(this.musicVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(music)));
            }

            if (save)
            {
                // Apply to save data too
                this.m_DataStore.masterVolume = master;
                this.m_DataStore.sfxVolume = sfx;
                this.m_DataStore.musicVolume = music;
                this.SaveData();
            }
        }

        /// <summary>
        /// Load data
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            this.LoadData();
        }

        /// <summary>
        /// Initialize volumes. We cannot change mixer params on awake
        /// </summary>
        protected virtual void Start()
        {
            this.SetVolumes(this.m_DataStore.masterVolume, this.m_DataStore.sfxVolume, this.m_DataStore.musicVolume, false);
        }

        /// <summary>
        /// Set up persistence
        /// </summary>
        protected void LoadData()
        {
            // If it is in Unity Editor use the standard JSON (human readable for debugging) otherwise encrypt it for deployed version
#if UNITY_EDITOR
            this.m_DataSaver = new JsonSaver<TDataStore>(k_SavedGameFile);
#else
			m_DataSaver = new EncryptedJsonSaver<TDataStore>(k_SavedGameFile);
#endif

            try
            {
                if (!this.m_DataSaver.Load(out this.m_DataStore))
                {
                    this.m_DataStore = new TDataStore();
                    this.SaveData();
                }
            }
            catch (Exception)
            {
                Debug.Log("Failed to load data, resetting");
                this.m_DataStore = new TDataStore();
                this.SaveData();
            }
        }

        /// <summary>
        /// Saves the gamme
        /// </summary>
        protected virtual void SaveData()
        {
            this.m_DataSaver.Save(this.m_DataStore);
        }

        /// <summary>
        /// Transform volume from linear to logarithmic
        /// </summary>
        protected static float LogarithmicDbTransform(float volume)
        {
            volume = (Mathf.Log(89 * volume + 1) / Mathf.Log(90)) * 80;
            return volume - 80;
        }
    }
}