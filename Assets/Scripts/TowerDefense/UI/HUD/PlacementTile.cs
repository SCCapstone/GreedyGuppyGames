// <copyright file="PlacementTile.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// States the placement tile can be in
    /// </summary>
    public enum PlacementTileState
    {
        Filled,
        Empty
    }

    /// <summary>
    /// Simple class to illustrate tile placement locations
    /// </summary>
    public class PlacementTile : MonoBehaviour
    {
        /// <summary>
        /// Material to use when this tile is empty
        /// </summary>
        public Material emptyMaterial;
        /// <summary>
        /// Material to use when this tile is filled
        /// </summary>
        public Material filledMaterial;
        /// <summary>
        /// The renderer whose material we're changing
        /// </summary>
        public Renderer tileRenderer;

        /// <summary>
        /// Update the state of this placement tile
        /// </summary>
        public void SetState(PlacementTileState newState)
        {
            switch (newState)
            {
                case PlacementTileState.Filled:
                    if (this.tileRenderer != null && this.filledMaterial != null)
                    {
                        this.tileRenderer.sharedMaterial = this.filledMaterial;
                    }

                    break;
                case PlacementTileState.Empty:
                    if (this.tileRenderer != null && this.emptyMaterial != null)
                    {
                        this.tileRenderer.sharedMaterial = this.emptyMaterial;
                    }

                    break;
            }
        }
    }
}