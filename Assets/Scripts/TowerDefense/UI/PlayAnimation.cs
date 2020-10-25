// <copyright file="PlayAnimation.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.UI
{
    /// <summary>
    /// A simple component that plays an animation
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public class PlayAnimation : MonoBehaviour
    {
        private Animation m_Animation;

        public void Play(string animationName)
        {
            this.m_Animation.Play(animationName);
        }

        private void Start()
        {
            this.m_Animation = this.GetComponent<Animation>();
        }
    }
}
