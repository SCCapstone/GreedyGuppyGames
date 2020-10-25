// <copyright file="MainMenu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// Abstract base class of a MainMenu
    /// The concrete class should expose serialized fields for the different pages e.g. OptionsMenu
    /// The concrete class should expose methods for change pages that use ChangePage() under the hood. e.g. OpenOptionsMenu()
    /// </summary>
    public abstract class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// Currently open MenuPage
        /// </summary>
        protected IMainMenuPage m_CurrentPage;

        /// <summary>
        /// This stack is to track the pages used to get to specific page - use by the back methods
        /// </summary>
        protected Stack<IMainMenuPage> m_PageStack = new Stack<IMainMenuPage>();

        /// <summary>
        /// Change page
        /// </summary>
        /// <param name="newPage">the page to transition to</param>
        protected virtual void ChangePage(IMainMenuPage newPage)
        {
            this.DeactivateCurrentPage();
            this.ActivateCurrentPage(newPage);
        }

        /// <summary>
        /// Deactivates the current page is there is one
        /// </summary>
        protected void DeactivateCurrentPage()
        {
            if (this.m_CurrentPage != null)
            {
                this.m_CurrentPage.Hide();
            }
        }

        /// <summary>
        /// Activates the new page, sets it to the current page an adds it to the stack
        /// </summary>
        /// <param name="newPage">the page to be activated</param>
        protected void ActivateCurrentPage(IMainMenuPage newPage)
        {
            this.m_CurrentPage = newPage;
            this.m_CurrentPage.Show();
            this.m_PageStack.Push(this.m_CurrentPage);
        }

        /// <summary>
        /// Goes back to a certain page
        /// </summary>
        /// <param name="backPage">Page to go back to</param>
        protected void SafeBack(IMainMenuPage backPage)
        {
            this.DeactivateCurrentPage();
            this.ActivateCurrentPage(backPage);
        }

        /// <summary>
        /// Goes back one page if possible
        /// </summary>
        public virtual void Back()
        {
            if (this.m_PageStack.Count == 0)
            {
                return;
            }

            this.DeactivateCurrentPage();
            this.m_PageStack.Pop();
            this.ActivateCurrentPage(this.m_PageStack.Pop());
        }

        /// <summary>
        /// Goes back to a specified page if possible
        /// </summary>
        /// <param name="backPage">Page to go back to</param>
        public virtual void Back(IMainMenuPage backPage)
        {
            int count = this.m_PageStack.Count;
            if (count == 0)
            {
                this.SafeBack(backPage);
                return;
            }

            for (int i = count - 1; i >= 0; i--)
            {
                IMainMenuPage currentPage = this.m_PageStack.Pop();
                if (currentPage == backPage)
                {
                    this.SafeBack(backPage);
                    return;
                }
            }

            this.SafeBack(backPage);
        }
    }
}