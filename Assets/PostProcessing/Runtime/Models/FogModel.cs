// <copyright file="FogModel.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public class FogModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            [Tooltip("Should the fog affect the skybox?")]
            public bool excludeSkybox;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        excludeSkybox = true
                    };
                }
            }
        }

        [SerializeField]
        private Settings m_Settings = Settings.defaultSettings;

        public Settings settings
        {
            get { return this.m_Settings; }
            set { this.m_Settings = value; }
        }

        public override void Reset()
        {
            this.m_Settings = Settings.defaultSettings;
        }
    }
}
