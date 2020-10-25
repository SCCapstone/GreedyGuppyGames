// <copyright file="InputSchemeSwitcher.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace Core.Input
{
    /// <summary>
    /// Base component that switches between active input schemes
    /// </summary>
    [DisallowMultipleComponent]
    public class InputSchemeSwitcher : MonoBehaviour
    {
        /// <summary>
        /// The attached input schemes
        /// </summary>
        protected InputScheme[] m_InputSchemes;

        /// <summary>
        /// The default scheme based on the platform
        /// </summary>
        protected InputScheme m_DefaultScheme;

        /// <summary>
        /// The current scheme activated
        /// </summary>
        protected InputScheme m_CurrentScheme;

        /// <summary>
        /// Cache the schemes and activate the default
        /// </summary>
        protected virtual void Awake()
        {
            this.m_InputSchemes = this.GetComponents<InputScheme>();
            foreach (InputScheme scheme in this.m_InputSchemes)
            {
                scheme.Deactivate(null);
                if (this.m_CurrentScheme == null && scheme.isDefault)
                {
                    this.m_DefaultScheme = scheme;
                }
            }

            if (this.m_DefaultScheme == null)
            {
                Debug.LogError("[InputSchemeSwitcher] Default scheme not set.");
                return;
            }

            this.m_DefaultScheme.Activate(null);
            this.m_CurrentScheme = this.m_DefaultScheme;
        }

        /// <summary>
        /// Checks the different schemes and activates them if needed
        /// </summary>
        protected virtual void Update()
        {
            foreach (InputScheme scheme in this.m_InputSchemes)
            {
                if (scheme.enabled || !scheme.shouldActivate)
                {
                    continue;
                }

                if (this.m_CurrentScheme != null)
                {
                    this.m_CurrentScheme.Deactivate(scheme);
                }

                scheme.Activate(this.m_CurrentScheme);
                this.m_CurrentScheme = scheme;
                break;
            }
        }
    }
}