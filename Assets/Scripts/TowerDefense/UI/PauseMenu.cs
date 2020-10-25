// <copyright file="PauseMenu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Game;
using TowerDefense.Game;
using TowerDefense.UI.HUD;
using UnityEngine;
using UnityEngine.UI;
using GameUIState = TowerDefense.UI.HUD.GameUI.State;

namespace TowerDefense.UI
{
    /// <summary>
    /// In-game pause menu
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Enum to represent state of pause menu
        /// </summary>
        protected enum State
        {
            Open,
            LevelSelectPressed,
            RestartPressed,
            Closed
        }

        /// <summary>
        /// The CanvasGroup that holds the pause menu UI
        /// </summary>
        public Canvas pauseMenuCanvas;

        public Text titleText;

        public Text descriptionText;

        /// <summary>
        /// The buttons present in the pause menu
        /// </summary>
        public Button levelSelectConfirmButton;

        public Button restartConfirmButton;

        public Button levelSelectButton;

        public Button restartButton;

        public Image topPanel;

        /// <summary>
        /// Color to change the top panel to highlight confirmation button
        /// </summary>
        public Color topPanelDisabledColor = new Color(1, 1, 1, 1);

        /// <summary>
        /// State of pause menu
        /// </summary>
        protected State m_State;

        /// <summary>
        /// If the pause menu was opened/closed this frame
        /// </summary>
        private bool m_MenuChangedThisFrame;

        /// <summary>
        /// Open the pause menu
        /// </summary>
        public void OpenPauseMenu()
        {
            this.SetPauseMenuCanvas(true);

            LevelItem level = GameManager.instance.GetLevelForCurrentScene();
            if (level == null)
            {
                return;
            }

            if (this.titleText != null)
            {
                this.titleText.text = level.name;
            }

            if (this.descriptionText != null)
            {
                this.descriptionText.text = level.description;
            }

            this.m_State = State.Open;
        }

        /// <summary>
        /// Fired when GameUI's State changes
        /// </summary>
        /// <param name="oldState">The State that GameUI is leaving</param>
        /// <param name="newState">The State that GameUI is entering</param>
        protected void OnGameUIStateChanged(GameUIState oldState, GameUIState newState)
        {
            this.m_MenuChangedThisFrame = true;
            if (newState == GameUIState.Paused)
            {
                this.OpenPauseMenu();
            }
            else
            {
                this.ClosePauseMenu();
            }
        }

        /// <summary>
        /// Level select button pressed, display/hide confirmation button
        /// </summary>
        public void LevelSelectPressed()
        {
            bool open = this.m_State == State.Open;
            this.restartButton.interactable = !open;
            this.topPanel.color = open ? this.topPanelDisabledColor : Color.white;
            this.levelSelectConfirmButton.gameObject.SetActive(open);
            this.m_State = open ? State.LevelSelectPressed : State.Open;
        }

        /// <summary>
        /// Restart button pressed, display/hide confirmation button
        /// </summary>
        public void RestartPressed()
        {
            bool open = this.m_State == State.Open;
            this.levelSelectButton.interactable = !open;
            this.topPanel.color = open ? this.topPanelDisabledColor : Color.white;
            this.restartConfirmButton.gameObject.SetActive(open);
            this.m_State = open ? State.RestartPressed : State.Open;
        }

        /// <summary>
        /// Close the pause menu
        /// </summary>
        public void ClosePauseMenu()
        {
            this.SetPauseMenuCanvas(false);

            this.levelSelectConfirmButton.gameObject.SetActive(false);
            this.restartConfirmButton.gameObject.SetActive(false);
            this.levelSelectButton.interactable = true;
            this.restartButton.interactable = true;
            this.topPanel.color = Color.white;

            this.m_State = State.Closed;
        }

        /// <summary>
        /// Hide the pause menu on awake
        /// </summary>
        protected void Awake()
        {
            this.SetPauseMenuCanvas(false);
            this.m_State = State.Closed;
        }

        /// <summary>
        /// Subscribe to GameUI's stateChanged event
        /// </summary>
        protected void Start()
        {
            if (GameUI.instanceExists)
            {
                GameUI.instance.stateChanged += this.OnGameUIStateChanged;
            }
        }

        /// <summary>
        /// Unpause the game if the game is paused and the Escape key is pressed
        /// </summary>
        protected virtual void Update()
        {
            if (this.m_MenuChangedThisFrame)
            {
                this.m_MenuChangedThisFrame = false;
                return;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && GameUI.instance.state == GameUIState.Paused)
            {
                this.Unpause();
            }
        }

        /// <summary>
        /// Show/Hide the pause menu canvas group
        /// </summary>
        protected void SetPauseMenuCanvas(bool enable)
        {
            this.pauseMenuCanvas.enabled = enable;
        }

        public void Pause()
        {
            if (GameUI.instanceExists)
            {
                GameUI.instance.Pause();
            }
        }

        public void Unpause()
        {
            if (GameUI.instanceExists)
            {
                GameUI.instance.Unpause();
            }
        }
    }
}