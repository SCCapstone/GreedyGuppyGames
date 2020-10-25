// <copyright file="TowerSpawnButton.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using Core.Economy;
using TowerDefense.Level;
using TowerDefense.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// A button controller for spawning towers
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class TowerSpawnButton : MonoBehaviour, IDragHandler
    {
        /// <summary>
        /// The text attached to the button
        /// </summary>
        public Text buttonText;

        public Image towerIcon;

        public Button buyButton;

        public Image energyIcon;

        public Color energyDefaultColor;

        public Color energyInvalidColor;

        /// <summary>
        /// Fires when the button is tapped
        /// </summary>
        public event Action<Tower> buttonTapped;

        /// <summary>
        /// Fires when the pointer is outside of the button bounds
        /// and still down
        /// </summary>
        public event Action<Tower> draggedOff;

        /// <summary>
        /// The tower controller that defines the button
        /// </summary>
        private Tower m_Tower;

        /// <summary>
        /// Cached reference to level currency
        /// </summary>
        private Currency m_Currency;

        /// <summary>
        /// The attached rect transform
        /// </summary>
        private RectTransform m_RectTransform;

        /// <summary>
        /// Checks if the pointer is out of bounds
        /// and then fires the draggedOff event
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(this.m_RectTransform, eventData.position))
            {
                if (this.draggedOff != null)
                {
                    this.draggedOff(this.m_Tower);
                }
            }
        }

        /// <summary>
        /// Define the button information for the tower
        /// </summary>
        /// <param name="towerData">
        /// The tower to initialize the button with
        /// </param>
        public void InitializeButton(Tower towerData)
        {
            this.m_Tower = towerData;

            if (towerData.levels.Length > 0)
            {
                TowerLevel firstTower = towerData.levels[0];
                this.buttonText.text = firstTower.cost.ToString();
                this.towerIcon.sprite = firstTower.levelData.icon;
            }
            else
            {
                Debug.LogWarning("[Tower Spawn Button] No level data for tower");
            }

            if (LevelManager.instanceExists)
            {
                this.m_Currency = LevelManager.instance.currency;
                this.m_Currency.currencyChanged += this.UpdateButton;
            }
            else
            {
                Debug.LogWarning("[Tower Spawn Button] No level manager to get currency object");
            }

            this.UpdateButton();
        }

        /// <summary>
        /// Cache the rect transform
        /// </summary>
        protected virtual void Awake()
        {
            this.m_RectTransform = (RectTransform)this.transform;
        }

        /// <summary>
        /// Unsubscribe from events
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (this.m_Currency != null)
            {
                this.m_Currency.currencyChanged -= this.UpdateButton;
            }
        }

        /// <summary>
        /// The click for when the button is tapped
        /// </summary>
        public void OnClick()
        {
            if (this.buttonTapped != null)
            {
                this.buttonTapped(this.m_Tower);
            }
        }

        /// <summary>
        /// Update the button's button state based on cost
        /// </summary>
        private void UpdateButton()
        {
            if (this.m_Currency == null)
            {
                return;
            }

            // Enable button
            if (this.m_Currency.CanAfford(this.m_Tower.purchaseCost) && !this.buyButton.interactable)
            {
                this.buyButton.interactable = true;
                this.energyIcon.color = this.energyDefaultColor;
            }
            else if (!this.m_Currency.CanAfford(this.m_Tower.purchaseCost) && this.buyButton.interactable)
            {
                this.buyButton.interactable = false;
                this.energyIcon.color = this.energyInvalidColor;
            }
        }
    }
}