// <copyright file="TowerDefenseMainMenu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.UI;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// Main menu implementation for tower defense
    /// </summary>
    public class TowerDefenseMainMenu : MainMenu
    {
        /// <summary>
        /// Reference to options menu
        /// </summary>
        public OptionsMenu optionsMenu;

        /// <summary>
        /// Reference to title menu
        /// </summary>
        public SimpleMainMenuPage titleMenu;

        /// <summary>
        /// Reference to level select menu
        /// </summary>
        public LevelSelectScreen levelSelectMenu;

        /// <summary>
        /// Bring up the options menu
        /// </summary>
        public void ShowOptionsMenu()
        {
            this.ChangePage(this.optionsMenu);
        }

        /// <summary>
        /// Bring up the options menu
        /// </summary>
        public void ShowLevelSelectMenu()
        {
            this.ChangePage(this.levelSelectMenu);
        }

        /// <summary>
        /// Returns to the title screen
        /// </summary>
        public void ShowTitleScreen()
        {
            this.Back(this.titleMenu);
        }

        /// <summary>
        /// Set initial page
        /// </summary>
        protected virtual void Awake()
        {
            this.ShowTitleScreen();
        }

        /// <summary>
        /// Escape key input
        /// </summary>
        protected virtual void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if ((SimpleMainMenuPage)this.m_CurrentPage == this.titleMenu)
                {
                    Application.Quit();
                }
                else
                {
                    this.Back();
                }
            }
        }
    }
}