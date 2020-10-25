// <copyright file="MouseScroll.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace TowerDefense.UI
{
    /// <summary>
    /// Component to override ScrollRect, uses normalized mouse position inside ScrollRect to scroll
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class MouseScroll : MonoBehaviour
    {
        /// <summary>
        /// If the normalized scroll position should be clamped between 0 & 1
        /// </summary>
        public bool clampScroll = true;

        /// <summary>
        /// Buffer to adjust ScrollRect size
        /// </summary>
        public float scrollXBuffer;

        public float scrollYBuffer;

        protected ScrollRect m_ScrollRect;
        protected RectTransform m_ScrollRectTransform;

        protected bool m_OverrideScrolling,
                       m_HasRightBuffer;

        public void SetHasRightBuffer(bool rightBuffer)
        {
            this.m_HasRightBuffer = rightBuffer;
        }

        /// <summary>
        /// If appropriate, we cache ScrollRect reference, disable it and enable scrolling override
        /// </summary>
        private void Start()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            this.m_ScrollRect = this.GetComponent<ScrollRect>();
            this.m_ScrollRect.enabled = false;
            this.m_OverrideScrolling = true;
            this.m_ScrollRectTransform = (RectTransform)this.m_ScrollRect.transform;
#else
			m_OverrideScrolling = false;
#endif
        }

        /// <summary>
        ///  Use normalized mouse position inside ScrollRect to scroll
        /// </summary>
        private void Update()
        {
            if (!this.m_OverrideScrolling)
            {
                return;
            }

            Vector3 mousePosition = UnityInput.mousePosition;

            // only scroll if mouse is inside ScrollRect
            bool inside = RectTransformUtility.RectangleContainsScreenPoint(this.m_ScrollRectTransform, mousePosition);
            if (!inside)
            {
                return;
            }

            Rect rect = this.m_ScrollRectTransform.rect;
            float adjustmentX = rect.width * this.scrollXBuffer,
                  adjustmentY = rect.height * this.scrollYBuffer;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_ScrollRectTransform, mousePosition, null, out localPoint);

            Vector2 pivot = this.m_ScrollRectTransform.pivot;
            float x = (localPoint.x + (rect.width - adjustmentX) * pivot.x) / (rect.width - 2 * adjustmentX);
            float y = (localPoint.y + (rect.height - adjustmentY) * pivot.y) / (rect.height - 2 * adjustmentY);

            if (this.clampScroll)
            {
                x = Mathf.Clamp01(x);
                y = Mathf.Clamp01(y);
            }

            this.m_ScrollRect.normalizedPosition = new Vector2(x, y);
        }

        /// <summary>
        /// Called when a button inside the scroll is selected
        /// </summary>
        /// <param name="levelSelectButton">Selected child</param>
        public void SelectChild(LevelSelectButton levelSelectButton)
        {
            // minus one if  buffer
            int childCount = levelSelectButton.transform.parent.childCount - (this.m_HasRightBuffer ? 1 : 0);
            if (childCount > 1)
            {
                float normalized = (float)levelSelectButton.transform.GetSiblingIndex() / (childCount - 1);
                this.m_ScrollRect.normalizedPosition = new Vector2(normalized, 0);
            }
        }
    }
}