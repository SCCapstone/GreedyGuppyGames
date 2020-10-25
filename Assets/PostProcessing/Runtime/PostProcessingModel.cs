// <copyright file="PostProcessingModel.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public abstract class PostProcessingModel
    {
        [SerializeField, GetSet("enabled")]
        private bool m_Enabled;

        public bool enabled
        {
            get { return this.m_Enabled; }

            set
            {
                this.m_Enabled = value;

                if (value)
                {
                    this.OnValidate();
                }
            }
        }

        public abstract void Reset();

        public virtual void OnValidate()
        { }
    }
}
