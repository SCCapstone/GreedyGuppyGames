// <copyright file="LevelSelectButton.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Game;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    /// <summary>
    /// The button for selecting a level
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LevelSelectButton : MonoBehaviour, ISelectHandler
    {
        /// <summary>
        /// Reference to the required button component
        /// </summary>
        protected Button m_Button;

        /// <summary>
        /// The UI text element that displays the name of the level
        /// </summary>
        public Text titleDisplay;

        public Text description;

        public Sprite starAchieved;

        public Image[] stars;

        protected MouseScroll m_MouseScroll;

        /// <summary>
        /// The data concerning the level this button displays
        /// </summary>
        protected LevelItem m_Item;

        /// <summary>
        /// When the user clicks the button, change the scene
        /// </summary>
        public void ButtonClicked()
        {
            this.ChangeScenes();
        }

        /// <summary>
        /// A method for assigning the data from item to the button
        /// </summary>
        /// <param name="item">
        /// The data with the information concerning the level
        /// </param>
        public void Initialize(LevelItem item, MouseScroll mouseScroll)
        {
            this.LazyLoad();
            if (this.titleDisplay == null)
            {
                return;
            }

            this.m_Item = item;
            this.titleDisplay.text = item.name;
            this.description.text = item.description;
            this.HasPlayedState();
            this.m_MouseScroll = mouseScroll;
        }

        /// <summary>
        /// Configures the feedback concerning if the player has played
        /// </summary>
        protected void HasPlayedState()
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager == null)
            {
                return;
            }

            int starsForLevel = gameManager.GetStarsForLevel(this.m_Item.id);
            for (int i = 0; i < starsForLevel; i++)
            {
                this.stars[i].sprite = this.starAchieved;
            }
        }

        /// <summary>
        /// Changes the scene to the scene name provided by m_Item
        /// </summary>
        protected void ChangeScenes()
        {
            SceneManager.LoadScene(this.m_Item.sceneName);
        }

        /// <summary>
        /// Ensure <see cref="m_Button"/> is not null
        /// </summary>
        protected void LazyLoad()
        {
            if (this.m_Button == null)
            {
                this.m_Button = this.GetComponent<Button>();
            }
        }

        /// <summary>
        /// Remove all listeners on the button before destruction
        /// </summary>
        protected void OnDestroy()
        {
            if (this.m_Button != null)
            {
                this.m_Button.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// Implementation of ISelectHandler
        /// </summary>
        /// <param name="eventData">Select event data</param>
        public void OnSelect(BaseEventData eventData)
        {
            this.m_MouseScroll.SelectChild(this);
        }
    }
}