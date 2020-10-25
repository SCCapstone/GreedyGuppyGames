// <copyright file="PostProcessorConfigurationSelector.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.PostProcessing;

namespace TowerDefense.Cameras
{
    /// <summary>
    /// Simple component to select lower quality post processing configurations on mobile
    /// </summary>
    [RequireComponent(typeof(PostProcessingBehaviour))]
    public class PostProcessorConfigurationSelector : MonoBehaviour
    {
        public PostProcessingProfile highQualityProfile;

        public PostProcessingProfile lowQualityProfile;

        protected virtual void Awake()
        {
            var attachedPostProcessor = this.GetComponent<PostProcessingBehaviour>();

            PostProcessingProfile selectedProfile;

#if UNITY_STANDALONE
            selectedProfile = this.highQualityProfile;
#else
			selectedProfile = lowQualityProfile;
#endif

            attachedPostProcessor.profile = selectedProfile;
        }
    }
}