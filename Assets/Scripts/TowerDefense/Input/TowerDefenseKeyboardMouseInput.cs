// <copyright file="TowerDefenseKeyboardMouseInput.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Input;
using TowerDefense.Level;
using TowerDefense.Towers;
using TowerDefense.UI.HUD;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using State = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.Input
{
    [RequireComponent(typeof(GameUI))]
    public class TowerDefenseKeyboardMouseInput : KeyboardMouseInput
    {
        /// <summary>
        /// Cached eference to gameUI
        /// </summary>
        private GameUI m_GameUI;

        /// <summary>
        /// Register input events
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            this.m_GameUI = this.GetComponent<GameUI>();

            if (InputController.instanceExists)
            {
                InputController controller = InputController.instance;

                controller.tapped += this.OnTap;
                controller.mouseMoved += this.OnMouseMoved;
            }
        }

        /// <summary>
        /// Deregister input events
        /// </summary>
        protected override void OnDisable()
        {
            if (!InputController.instanceExists)
            {
                return;
            }

            InputController controller = InputController.instance;

            controller.tapped -= this.OnTap;
            controller.mouseMoved -= this.OnMouseMoved;
        }

        /// <summary>
        /// Handle camera panning behaviour
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // Escape handling
            if (UnityInput.GetKeyDown(KeyCode.Escape))
            {
                switch (this.m_GameUI.state)
                {
                    case State.Normal:
                        if (this.m_GameUI.isTowerSelected)
                        {
                            this.m_GameUI.DeselectTower();
                        }
                        else
                        {
                            this.m_GameUI.Pause();
                        }

                        break;
                    case State.BuildingWithDrag:
                    case State.Building:
                        this.m_GameUI.CancelGhostPlacement();
                        break;
                }
            }

            // place towers with keyboard numbers
            if (LevelManager.instanceExists)
            {
                int towerLibraryCount = LevelManager.instance.towerLibrary.Count;

                // find the lowest value between 9 (keyboard numbers)
                // and the amount of towers in the library
                int count = Mathf.Min(9, towerLibraryCount);
                KeyCode highestKey = KeyCode.Alpha1 + count;

                for (var key = KeyCode.Alpha1; key < highestKey; key++)
                {
                    // add offset for the KeyCode Alpha 1 index to find correct keycodes
                    if (UnityInput.GetKeyDown(key))
                    {
                        Tower controller = LevelManager.instance.towerLibrary[key - KeyCode.Alpha1];
                        if (LevelManager.instance.currency.CanAfford(controller.purchaseCost))
                        {
                            if (this.m_GameUI.isBuilding)
                            {
                                this.m_GameUI.CancelGhostPlacement();
                            }

                            GameUI.instance.SetToBuildMode(controller);
                            GameUI.instance.TryMoveGhost(InputController.instance.basicMouseInfo);
                        }

                        break;
                    }
                }

                // special case for 0 mapping to index 9
                if (count < 10 && UnityInput.GetKeyDown(KeyCode.Alpha0))
                {
                    Tower controller = LevelManager.instance.towerLibrary[9];
                    GameUI.instance.SetToBuildMode(controller);
                    GameUI.instance.TryMoveGhost(InputController.instance.basicMouseInfo);
                }
            }
        }

        /// <summary>
        /// Ghost follows pointer
        /// </summary>
        private void OnMouseMoved(PointerInfo pointer)
        {
            // We only respond to mouse info
            var mouseInfo = pointer as MouseCursorInfo;

            if ((mouseInfo != null) && (this.m_GameUI.isBuilding))
            {
                this.m_GameUI.TryMoveGhost(pointer, false);
            }
        }

        /// <summary>
        /// Select towers or position ghosts
        /// </summary>
        private void OnTap(PointerActionInfo pointer)
        {
            // We only respond to mouse info
            var mouseInfo = pointer as MouseButtonInfo;

            if (mouseInfo != null && !mouseInfo.startedOverUI)
            {
                if (this.m_GameUI.isBuilding)
                {
                    if (mouseInfo.mouseButtonId == 0) // LMB confirms
                    {
                        this.m_GameUI.TryPlaceTower(pointer);
                    }
                    else // RMB cancels
                    {
                        this.m_GameUI.CancelGhostPlacement();
                    }
                }
                else
                {
                    if (mouseInfo.mouseButtonId == 0)
                    {
                        // select towers
                        this.m_GameUI.TrySelectTower(pointer);
                    }
                }
            }
        }
    }
}