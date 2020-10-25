// <copyright file="MovingCanvas.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

namespace TowerDefense.UI
{
    /// <summary>
    /// A class for controlling conditional motion of the canvas
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class MovingCanvas : MonoBehaviour
    {
        /// <summary>
        /// The RectTransform used to check against the screen bounds
        /// </summary>
        public RectTransform content;

        /// <summary>
        /// To offset the position the canvas is placed at
        /// </summary>
        public Vector2 offset;

        /// <summary>
        /// The attached canvas
        /// </summary>
        private Canvas m_Canvas;

        /// <summary>
        /// Property for disabling and enabling the attached canvas
        /// </summary>
        public bool canvasEnabled
        {
            get
            {
                if (this.m_Canvas == null)
                {
                    this.m_Canvas = this.GetComponent<Canvas>();
                }

                return this.m_Canvas.enabled;
            }

            set
            {
                if (this.m_Canvas == null)
                {
                    this.m_Canvas = this.GetComponent<Canvas>();
                }

                this.m_Canvas.enabled = value;
            }
        }

        /// <summary>
        /// Try to move the canvas based on <see cref="content"/>'s rect
        /// </summary>
        /// <param name="position">
        /// The position to move to
        /// </param>
        public void TryMove(Vector3 position)
        {
            Rect rect = this.content.rect;
            position += (Vector3)this.offset;
            rect.position = position;

            if (rect.xMin < rect.width * 0.5f)
            {
                position.x = rect.width * 0.5f;
            }

            if (rect.xMax > Screen.width - rect.width * 0.5f)
            {
                position.x = Screen.width - rect.width * 0.5f;
            }

            if (rect.yMin < rect.height * 0.5f)
            {
                position.y = rect.height * 0.5f;
            }

            if (rect.yMax > Screen.height - rect.height * 0.5f)
            {
                position.y = Screen.height - rect.height * 0.5f;
            }

            this.transform.position = position;
        }

        /// <summary>
        /// Cache the attached canvas
        /// </summary>
        protected virtual void Awake()
        {
            this.canvasEnabled = false;
        }
    }
}