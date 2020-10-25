// <copyright file="TowerDefenseTouchInput.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Input;
using TowerDefense.UI;
using TowerDefense.UI.HUD;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using State = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.Input
{
    [RequireComponent(typeof(GameUI))]
    public class TowerDefenseTouchInput : TouchInput
    {
        /// <summary>
        /// A percentage of the screen where panning occurs while dragging
        /// </summary>
        [Range(0, 0.5f)]
        public float panAreaScreenPercentage = 0.2f;

        /// <summary>
        /// The object that holds the confirmation buttons
        /// </summary>
        public MovingCanvas confirmationButtons;

        /// <summary>
        /// The object that holds the invalid selection
        /// </summary>
        public MovingCanvas invalidButtons;

        /// <summary>
        /// The attached Game UI object
        /// </summary>
        private GameUI m_GameUI;

        /// <summary>
        /// Keeps track of whether or not the ghost tower is selected
        /// </summary>
        private bool m_IsGhostSelected;

        /// <summary>
        /// The pointer at the edge of the screen
        /// </summary>
        private TouchInfo m_DragPointer;

        /// <summary>
        /// Called by the confirm button on the UI
        /// </summary>
        public void OnTowerPlacementConfirmation()
        {
            this.confirmationButtons.canvasEnabled = false;
            if (!this.m_GameUI.IsGhostAtValidPosition())
            {
                return;
            }

            this.m_GameUI.BuyTower();
        }

        /// <summary>
        /// Called by the close button on the UI
        /// </summary>
        public void Cancel()
        {
            GameUI.instance.CancelGhostPlacement();
            this.confirmationButtons.canvasEnabled = false;
            this.invalidButtons.canvasEnabled = false;
        }

        /// <summary>
        /// Register input events
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            this.m_GameUI = this.GetComponent<GameUI>();

            this.m_GameUI.stateChanged += this.OnStateChanged;
            this.m_GameUI.ghostBecameValid += this.OnGhostBecameValid;

            // Register tap event
            if (InputController.instanceExists)
            {
                InputController.instance.tapped += this.OnTap;
                InputController.instance.startedDrag += this.OnStartDrag;
            }

            // disable pop ups
            this.confirmationButtons.canvasEnabled = false;
            this.invalidButtons.canvasEnabled = false;
        }

        /// <summary>
        /// Deregister input events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (this.confirmationButtons != null)
            {
                this.confirmationButtons.canvasEnabled = false;
            }

            if (this.invalidButtons != null)
            {
                this.invalidButtons.canvasEnabled = false;
            }

            if (InputController.instanceExists)
            {
                InputController.instance.tapped -= this.OnTap;
                InputController.instance.startedDrag -= this.OnStartDrag;
            }

            if (this.m_GameUI != null)
            {
                this.m_GameUI.stateChanged -= this.OnStateChanged;
                this.m_GameUI.ghostBecameValid -= this.OnGhostBecameValid;
            }
        }

        /// <summary>
        /// Hide UI
        /// </summary>
        protected virtual void Awake()
        {
            if (this.confirmationButtons != null)
            {
                this.confirmationButtons.canvasEnabled = false;
            }

            if (this.invalidButtons != null)
            {
                this.invalidButtons.canvasEnabled = false;
            }
        }

        /// <summary>
        /// Decay flick
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // Edge pan
            if (this.m_DragPointer != null)
            {
                this.EdgePan();
            }

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
                    case State.Building:
                        this.m_GameUI.CancelGhostPlacement();
                        break;
                }
            }
        }

        /// <summary>
        /// Called on input press
        /// </summary>
        protected override void OnPress(PointerActionInfo pointer)
        {
            base.OnPress(pointer);
            var touchInfo = pointer as TouchInfo;
            // Press starts on a ghost? Then we can pick it up
            if (touchInfo != null)
            {
                if (this.m_GameUI.state == State.Building)
                {
                    this.m_IsGhostSelected = this.m_GameUI.IsPointerOverGhost(pointer);
                    if (this.m_IsGhostSelected)
                    {
                        this.m_DragPointer = touchInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Called on input release, for flicks
        /// </summary>
        protected override void OnRelease(PointerActionInfo pointer)
        {
            // Override normal behaviour. We only want to do flicks if there's no ghost selected
            // For this reason, we intentionally do not call base
            var touchInfo = pointer as TouchInfo;

            if (touchInfo != null)
            {
                // Show UI on release
                if (this.m_GameUI.isBuilding)
                {
                    Vector2 screenPoint = this.cameraRig.cachedCamera.WorldToScreenPoint(this.m_GameUI.GetGhostPosition());
                    if (this.m_GameUI.IsGhostAtValidPosition() && this.m_GameUI.IsValidPurchase())
                    {
                        this.confirmationButtons.canvasEnabled = true;
                        this.invalidButtons.canvasEnabled = false;
                        this.confirmationButtons.TryMove(screenPoint);
                    }
                    else
                    {
                        this.invalidButtons.canvasEnabled = true;
                        this.confirmationButtons.canvasEnabled = false;
                        this.confirmationButtons.TryMove(screenPoint);
                    }

                    if (this.m_IsGhostSelected)
                    {
                        this.m_GameUI.ReturnToBuildMode();
                    }
                }

                if (!this.m_IsGhostSelected && this.cameraRig != null)
                {
                    // Do normal base behaviour here
                    this.DoReleaseFlick(pointer);
                }

                this.m_IsGhostSelected = false;

                // Reset m_DragPointer if released
                if (this.m_DragPointer != null && this.m_DragPointer.touchId == touchInfo.touchId)
                {
                    this.m_DragPointer = null;
                }
            }
        }

        /// <summary>
        /// Called on tap,
        /// calls confirmation of tower placement
        /// </summary>
        protected virtual void OnTap(PointerActionInfo pointerActionInfo)
        {
            var touchInfo = pointerActionInfo as TouchInfo;
            if (touchInfo != null)
            {
                if (this.m_GameUI.state == State.Normal && !touchInfo.startedOverUI)
                {
                    this.m_GameUI.TrySelectTower(touchInfo);
                }
                else if (this.m_GameUI.state == State.Building && !touchInfo.startedOverUI)
                {
                    this.m_GameUI.TryMoveGhost(touchInfo, false);
                    if (this.m_GameUI.IsGhostAtValidPosition() && this.m_GameUI.IsValidPurchase())
                    {
                        this.confirmationButtons.canvasEnabled = true;
                        this.invalidButtons.canvasEnabled = false;
                        this.confirmationButtons.TryMove(touchInfo.currentPosition);
                    }
                    else
                    {
                        this.invalidButtons.canvasEnabled = true;
                        this.invalidButtons.TryMove(touchInfo.currentPosition);
                        this.confirmationButtons.canvasEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Assigns the drag pointer and sets the UI into drag mode
        /// </summary>
        /// <param name="pointer"></param>
        protected virtual void OnStartDrag(PointerActionInfo pointer)
        {
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null)
            {
                if (this.m_IsGhostSelected)
                {
                    this.m_GameUI.ChangeToDragMode();
                    this.m_DragPointer = touchInfo;
                }
            }
        }

        /// <summary>
        /// Called when we drag
        /// </summary>
        protected override void OnDrag(PointerActionInfo pointer)
        {
            // Override normal behaviour. We only want to pan if there's no ghost selected
            // For this reason, we intentionally do not call base
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null)
            {
                // Try to pick up the tower if it was dragged off
                if (this.m_IsGhostSelected)
                {
                    this.m_GameUI.TryMoveGhost(pointer, false);
                }

                if (this.m_GameUI.state == State.BuildingWithDrag)
                {
                    this.DragGhost(touchInfo);
                }
                else
                {
                    // Do normal base behaviour only if no ghost selected
                    if (this.cameraRig != null)
                    {
                        this.DoDragPan(pointer);

                        if (this.invalidButtons.canvasEnabled)
                        {
                            this.invalidButtons.TryMove(this.cameraRig.cachedCamera.WorldToScreenPoint(this.m_GameUI.GetGhostPosition()));
                        }

                        if (this.confirmationButtons.canvasEnabled)
                        {
                            this.confirmationButtons.TryMove(this.cameraRig.cachedCamera.WorldToScreenPoint(this.m_GameUI.GetGhostPosition()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drags the ghost
        /// </summary>
        private void DragGhost(TouchInfo touchInfo)
        {
            if (touchInfo.touchId == this.m_DragPointer.touchId)
            {
                this.m_GameUI.TryMoveGhost(touchInfo, false);

                if (this.invalidButtons.canvasEnabled)
                {
                    this.invalidButtons.canvasEnabled = false;
                }

                if (this.confirmationButtons.canvasEnabled)
                {
                    this.confirmationButtons.canvasEnabled = false;
                }
            }
        }

        /// <summary>
        /// pans at the edge of the screen
        /// </summary>
        private void EdgePan()
        {
            float edgeWidth = this.panAreaScreenPercentage * Screen.width;
            this.PanWithScreenCoordinates(this.m_DragPointer.currentPosition, edgeWidth, this.panSpeed);
        }

        /// <summary>
        /// If the new state is <see cref="GameUI.State.Building"/> then move the ghost to the center of the screen
        /// </summary>
        /// <param name="previousState">
        /// The previous the GameUI was is in
        /// </param>
        /// <param name="currentState">
        /// The new state the GameUI is in
        /// </param>
        private void OnStateChanged(State previousState, State currentState)
        {
            // Early return for two reasons
            // 1. We are not moving into Build Mode
            // 2. We are not actually touching
            if (UnityInput.touchCount == 0)
            {
                return;
            }

            if (currentState == State.Building && previousState != State.BuildingWithDrag)
            {
                this.m_GameUI.MoveGhostToCenter();
                this.confirmationButtons.canvasEnabled = false;
                this.invalidButtons.canvasEnabled = false;
            }

            if (currentState == State.BuildingWithDrag)
            {
                this.m_IsGhostSelected = true;
            }
        }

        /// <summary>
        /// Displays the correct confirmation buttons when the tower has become valid
        /// </summary>
        private void OnGhostBecameValid()
        {
            // this only needs to be done if the invalid buttons are already on screen
            if (!this.invalidButtons.canvasEnabled)
            {
                return;
            }

            Vector2 screenPoint = this.cameraRig.cachedCamera.WorldToScreenPoint(this.m_GameUI.GetGhostPosition());
            if (!this.confirmationButtons.canvasEnabled)
            {
                this.confirmationButtons.canvasEnabled = true;
                this.invalidButtons.canvasEnabled = false;
                this.confirmationButtons.TryMove(screenPoint);
            }
        }
    }
}