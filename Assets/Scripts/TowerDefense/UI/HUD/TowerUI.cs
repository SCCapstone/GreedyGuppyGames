// <copyright file="TowerUI.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Level;
using TowerDefense.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// Controls the UI objects that draw the tower data
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class TowerUI : MonoBehaviour
    {
        /// <summary>
        /// The text object for the name
        /// </summary>
        public Text towerName;

        /// <summary>
        /// The text object for the description
        /// </summary>
        public Text description;

        public Text upgradeDescription;

        /// <summary>
        /// The attached sell button
        /// </summary>
        public Button sellButton;

        /// <summary>
        /// The attached upgrade button
        /// </summary>
        public Button upgradeButton;

        /// <summary>
        /// Component to display the relevant information of the tower
        /// </summary>
        public TowerInfoDisplay towerInfoDisplay;

        public RectTransform panelRectTransform;

        public GameObject[] confirmationButtons;

        /// <summary>
        /// The main game camera
        /// </summary>
        protected Camera m_GameCamera;

        /// <summary>
        /// The current tower to draw
        /// </summary>
        protected Tower m_Tower;

        /// <summary>
        /// The canvas attached to the gameObject
        /// </summary>
        protected Canvas m_Canvas;

        /// <summary>
        /// Draws the tower data on to the canvas
        /// </summary>
        /// <param name="towerToShow">
        /// The tower to gain info from
        /// </param>
        public virtual void Show(Tower towerToShow)
        {
            if (towerToShow == null)
            {
                return;
            }

            this.m_Tower = towerToShow;
            this.AdjustPosition();

            this.m_Canvas.enabled = true;

            int sellValue = this.m_Tower.GetSellLevel();
            if (this.sellButton != null)
            {
                this.sellButton.gameObject.SetActive(sellValue > 0);
            }

            if (this.upgradeButton != null)
            {
                this.upgradeButton.interactable =
                    LevelManager.instance.currency.CanAfford(this.m_Tower.GetCostForNextLevel());
                bool maxLevel = this.m_Tower.isAtMaxLevel;
                this.upgradeButton.gameObject.SetActive(!maxLevel);
                if (!maxLevel)
                {
                    this.upgradeDescription.text =
                        this.m_Tower.levels[this.m_Tower.currentLevel + 1].upgradeDescription.ToUpper();
                }
            }

            LevelManager.instance.currency.currencyChanged += this.OnCurrencyChanged;
            this.towerInfoDisplay.Show(towerToShow);
            foreach (var button in this.confirmationButtons)
            {
                button.SetActive(false);
            }
        }

        /// <summary>
        /// Hides the tower info UI and the radius visualizer
        /// </summary>
        public virtual void Hide()
        {
            this.m_Tower = null;
            if (GameUI.instanceExists)
            {
                GameUI.instance.HideRadiusVisualizer();
            }

            this.m_Canvas.enabled = false;
            LevelManager.instance.currency.currencyChanged -= this.OnCurrencyChanged;
        }

        /// <summary>
        /// Upgrades the tower through <see cref="GameUI"/>
        /// </summary>
        public void UpgradeButtonClick()
        {
            GameUI.instance.UpgradeSelectedTower();
        }

        /// <summary>
        /// Sells the tower through <see cref="GameUI"/>
        /// </summary>
        public void SellButtonClick()
        {
            GameUI.instance.SellSelectedTower();
        }

        /// <summary>
        /// Get the text attached to the buttons
        /// </summary>
        protected virtual void Awake()
        {
            this.m_Canvas = this.GetComponent<Canvas>();
        }

        /// <summary>
        /// Fires when tower is selected/deselected
        /// </summary>
        /// <param name="newTower"></param>
        protected virtual void OnUISelectionChanged(Tower newTower)
        {
            if (newTower != null)
            {
                this.Show(newTower);
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Subscribe to mouse button action
        /// </summary>
        protected virtual void Start()
        {
            this.m_GameCamera = Camera.main;
            this.m_Canvas.enabled = false;
            if (GameUI.instanceExists)
            {
                GameUI.instance.selectionChanged += this.OnUISelectionChanged;
                GameUI.instance.stateChanged += this.OnGameUIStateChanged;
            }
        }

        /// <summary>
        /// Adjust position when the camera moves
        /// </summary>
        protected virtual void Update()
        {
            this.AdjustPosition();
        }

        /// <summary>
        /// Unsubscribe from currencyChanged
        /// </summary>
        protected virtual void OnDisable()
        {
            if (LevelManager.instanceExists)
            {
                LevelManager.instance.currency.currencyChanged -= this.OnCurrencyChanged;
            }
        }

        /// <summary>
        /// Adjust the position of the UI
        /// </summary>
        protected void AdjustPosition()
        {
            if (this.m_Tower == null)
            {
                return;
            }

            Vector3 point = this.m_GameCamera.WorldToScreenPoint(this.m_Tower.position);
            point.z = 0;
            this.panelRectTransform.transform.position = point;
        }

        /// <summary>
        /// Fired when the <see cref="GameUI"/> state changes
        /// If the new state is <see cref="GameUI.State.GameOver"/> we need to hide the <see cref="TowerUI"/>
        /// </summary>
        /// <param name="oldState">The previous state</param>
        /// <param name="newState">The state to transition to</param>
        protected void OnGameUIStateChanged(GameUI.State oldState, GameUI.State newState)
        {
            if (newState == GameUI.State.GameOver)
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Check if player can afford upgrade on currency changed
        /// </summary>
        private void OnCurrencyChanged()
        {
            if (this.m_Tower != null && this.upgradeButton != null)
            {
                this.upgradeButton.interactable =
                    LevelManager.instance.currency.CanAfford(this.m_Tower.GetCostForNextLevel());
            }
        }

        /// <summary>
        /// Unsubscribe from GameUI selectionChanged and stateChanged
        /// </summary>
        private void OnDestroy()
        {
            if (GameUI.instanceExists)
            {
                GameUI.instance.selectionChanged -= this.OnUISelectionChanged;
                GameUI.instance.stateChanged -= this.OnGameUIStateChanged;
            }
        }
    }
}