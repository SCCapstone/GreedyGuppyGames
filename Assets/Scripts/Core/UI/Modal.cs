// <copyright file="Modal.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// Abstract base class for all modals
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Modal : MonoBehaviour
    {
        /// <summary>
        /// The attached CanvasGroup
        /// </summary>
        public CanvasGroup canvasGroup;

        /// <summary>
        /// Closes the modal
        /// </summary>
        public virtual void CloseModal()
        {
            this.gameObject.SetActive(false);
            this.DisableInteractivity();
        }

        /// <summary>
        /// Shows the modal
        /// </summary>
        public virtual void Show()
        {
            this.LazyLoad();
            this.gameObject.SetActive(true);
            this.EnableInteractivity();
        }

        /// <summary>
        /// Allows interactions
        /// </summary>
        protected virtual void EnableInteractivity()
        {
            this.canvasGroup.interactable = true;
        }

        /// <summary>
        /// Turns off interactions
        /// </summary>
        protected virtual void DisableInteractivity()
        {
            this.canvasGroup.interactable = false;
        }

        /// <summary>
        /// Lazy loads the canvas group into the local variable
        /// </summary>
        protected virtual void LazyLoad()
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup = this.GetComponent<CanvasGroup>();
            }
        }
    }
}