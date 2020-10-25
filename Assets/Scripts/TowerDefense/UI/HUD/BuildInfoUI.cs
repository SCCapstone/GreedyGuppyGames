// <copyright file="BuildInfoUI.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// A class that controls the information display
    /// whilst dragging the ghost tower
    /// </summary>
    [RequireComponent(typeof(TowerUI))]
    public class BuildInfoUI : MonoBehaviour
    {
        /// <summary>
        /// an enum for easily keeping track of UI animation
        /// </summary>
        public enum AnimationState
        {
            /// <summary>
            /// The UI is completely hidden
            /// </summary>
            Hidden,

            /// <summary>
            /// The UI is animation to be shown
            /// </summary>
            Showing,

            /// <summary>
            /// the UI is completely shown
            /// </summary>
            Shown,

            /// <summary>
            /// The UI is animating
            /// </summary>
            Hiding
        }

        /// <summary>
        /// The attached animator
        /// </summary>
        public Animation anim;

        /// <summary>
        /// The name of the clip that shows the UI
        /// </summary>
        public string showClipName = "Show";

        /// <summary>
        /// The name of the clip that hides the UI
        /// </summary>
        public string hideClipName = "Hide";

        /// <summary>
        /// The attached <see cref="TowerUI"/>
        /// </summary>
        protected TowerUI m_TowerUI;

        /// <summary>
        /// The attached canvas
        /// </summary>
        protected Canvas m_Canvas;

        /// <summary>
        /// Tracks the animation of the UI
        /// </summary>
        private AnimationState m_State;

        /// <summary>
        /// NOTE: Plays from Show animation clip event
        /// Fires at the end of the show animation
        /// Sets <see cref="m_State"/> to Show
        /// </summary>
        public void ShowEnd()
        {
            this.m_State = AnimationState.Shown;
        }

        /// <summary>
        /// NOTE: Plays from Hide animation clip event
        /// Fires at the end of the hide animation
        /// Sets <see cref="m_State"/> to Hidden
        /// </summary>
        public void HideEnd()
        {
            this.m_State = AnimationState.Hidden;
        }

        /// <summary>
        /// Shows the information
        /// </summary>
        /// <param name="controller">
        /// The tower information to display
        /// </param>
        public virtual void Show(Tower controller)
        {
            this.m_TowerUI.Show(controller);
            if (this.m_State == AnimationState.Shown)
            {
                return;
            }

            this.anim.Play(this.showClipName);
            if (this.m_State == AnimationState.Hiding)
            {
                this.anim[this.showClipName].normalizedTime = 1;
                this.m_State = AnimationState.Shown;
                return;
            }

            this.m_State = this.anim[this.showClipName].normalizedTime < 1 ? AnimationState.Showing :
                AnimationState.Shown;
        }

        /// <summary>
        /// Hides the information
        /// </summary>
        public virtual void Hide()
        {
            if (this.m_State == AnimationState.Hidden)
            {
                return;
            }

            this.m_TowerUI.Hide();
            this.anim.Play(this.hideClipName);
            this.m_State = this.anim[this.hideClipName].normalizedTime < 1 ? AnimationState.Hiding :
                AnimationState.Hidden;
        }

        /// <summary>
        /// Cache the attached Canvas and the attached TowerControllerUI
        /// </summary>
        protected virtual void Awake()
        {
            this.m_Canvas = this.GetComponent<Canvas>();
            this.m_TowerUI = this.GetComponent<TowerUI>();
        }
    }
}