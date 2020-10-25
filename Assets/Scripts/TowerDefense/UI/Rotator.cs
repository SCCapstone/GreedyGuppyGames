// <copyright file="Rotator.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.UI
{
    /// <summary>
    /// A simple component that applies a constant rotation to a transform
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        public Vector3 rotationSpeed;

        private void Update()
        {
            this.transform.localEulerAngles += this.rotationSpeed;
        }
    }
}
