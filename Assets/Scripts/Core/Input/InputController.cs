// <copyright file="InputController.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;
using UnityInput = UnityEngine.Input;

namespace Core.Input
{
    /// <summary>
    /// Class to manage tap/drag/pinch gestures and other controls
    /// </summary>
    public class InputController : Singleton<InputController>
    {
        /// <summary>
        /// How quickly flick velocity is accumulated with movements
        /// </summary>
        private const float k_FlickAccumulationFactor = 0.8f;

        /// <summary>
        /// How far fingers must move before starting a drag
        /// </summary>
        public float dragThresholdTouch = 5;

        /// <summary>
        /// How far mouse must move before starting a drag
        /// </summary>
        public float dragThresholdMouse;

        /// <summary>
        /// How long before a touch can no longer be considered a tap
        /// </summary>
        public float tapTime = 0.2f;

        /// <summary>
        /// How long before a touch is considered a hold
        /// </summary>
        public float holdTime = 0.8f;

        /// <summary>
        /// Sensitivity of mouse-wheel based zoom
        /// </summary>
        public float mouseWheelSensitivity = 1.0f;

        /// <summary>
        /// How many mouse buttons to track
        /// </summary>
        public int trackMouseButtons = 2;

        /// <summary>
        /// Flick movement threshold
        /// </summary>
        public float flickThreshold = 2f;

        /// <summary>
        /// All the touches we're tracking
        /// </summary>
        private List<TouchInfo> m_Touches;

        /// <summary>
        /// Mouse button info
        /// </summary>
        private List<MouseButtonInfo> m_MouseInfo;

        /// <summary>
        /// Gets the number of active touches
        /// </summary>
        public int activeTouchCount
        {
            get { return this.m_Touches.Count; }
        }

        /// <summary>
        /// Tracks if any of the mouse buttons were pressed this frame
        /// </summary>
        public bool mouseButtonPressedThisFrame { get; private set; }

        /// <summary>
        /// Tracks if the mouse moved this frame
        /// </summary>
        public bool mouseMovedOnThisFrame { get; private set; }

        /// <summary>
        /// Tracks if a touch began this frame
        /// </summary>
        public bool touchPressedThisFrame { get; private set; }

        /// <summary>
        /// Current mouse pointer info
        /// </summary>
        public PointerInfo basicMouseInfo { get; private set; }

        /// <summary>
        /// Event called when a pointer press is detected
        /// </summary>
        public event Action<PointerActionInfo> pressed;

        /// <summary>
        /// Event called when a pointer is released
        /// </summary>
        public event Action<PointerActionInfo> released;

        /// <summary>
        /// Event called when a pointer is tapped
        /// </summary>
        public event Action<PointerActionInfo> tapped;

        /// <summary>
        /// Event called when a drag starts
        /// </summary>
        public event Action<PointerActionInfo> startedDrag;

        /// <summary>
        /// Event called when a pointer is dragged
        /// </summary>
        public event Action<PointerActionInfo> dragged;

        /// <summary>
        /// Event called when a pointer starts a hold
        /// </summary>
        public event Action<PointerActionInfo> startedHold;

        /// <summary>
        /// Event called when the user scrolls the mouse wheel
        /// </summary>
        public event Action<WheelInfo> spunWheel;

        /// <summary>
        /// Event called when the user performs a pinch gesture
        /// </summary>
        public event Action<PinchInfo> pinched;

        /// <summary>
        /// Event called whenever the mouse is moved
        /// </summary>
        public event Action<PointerInfo> mouseMoved;

        protected override void Awake()
        {
            base.Awake();
            this.m_Touches = new List<TouchInfo>();

            // Mouse specific initialization
            if (UnityInput.mousePresent)
            {
                this.m_MouseInfo = new List<MouseButtonInfo>();
                this.basicMouseInfo = new MouseCursorInfo { currentPosition = UnityInput.mousePosition };

                for (int i = 0; i < this.trackMouseButtons; ++i)
                {
                    this.m_MouseInfo.Add(new MouseButtonInfo
                    {
                        currentPosition = UnityInput.mousePosition,
                        mouseButtonId = i
                    });
                }
            }

            UnityInput.simulateMouseWithTouches = false;
        }

        /// <summary>
        /// Update all input
        /// </summary>
        private void Update()
        {
            if (this.basicMouseInfo != null)
            {
                // Mouse was detected as present
                this.UpdateMouse();
            }

            // Handle touches
            this.UpdateTouches();
        }

        /// <summary>
        /// Perform logic to update mouse/pointing device
        /// </summary>
        private void UpdateMouse()
        {
            this.basicMouseInfo.previousPosition = this.basicMouseInfo.currentPosition;
            this.basicMouseInfo.currentPosition = UnityInput.mousePosition;
            this.basicMouseInfo.delta = this.basicMouseInfo.currentPosition - this.basicMouseInfo.previousPosition;
            this.mouseMovedOnThisFrame = this.basicMouseInfo.delta.sqrMagnitude >= Mathf.Epsilon;
            this.mouseButtonPressedThisFrame = false;

            // Move event
            if (this.basicMouseInfo.delta.sqrMagnitude > Mathf.Epsilon)
            {
                if (this.mouseMoved != null)
                {
                    this.mouseMoved(this.basicMouseInfo);
                }
            }

            // Button events
            for (int i = 0; i < this.trackMouseButtons; ++i)
            {
                MouseButtonInfo mouseButton = this.m_MouseInfo[i];
                mouseButton.delta = this.basicMouseInfo.delta;
                mouseButton.previousPosition = this.basicMouseInfo.previousPosition;
                mouseButton.currentPosition = this.basicMouseInfo.currentPosition;
                if (UnityInput.GetMouseButton(i))
                {
                    if (!mouseButton.isDown)
                    {
                        // First press
                        this.mouseButtonPressedThisFrame = true;
                        mouseButton.isDown = true;
                        mouseButton.startPosition = UnityInput.mousePosition;
                        mouseButton.startTime = Time.realtimeSinceStartup;
                        mouseButton.startedOverUI = EventSystem.current.IsPointerOverGameObject(-mouseButton.mouseButtonId - 1);

                        // Reset some stuff
                        mouseButton.totalMovement = 0;
                        mouseButton.isDrag = false;
                        mouseButton.wasHold = false;
                        mouseButton.isHold = false;
                        mouseButton.flickVelocity = Vector2.zero;

                        if (this.pressed != null)
                        {
                            this.pressed(mouseButton);
                        }
                    }
                    else
                    {
                        float moveDist = mouseButton.delta.magnitude;
                        // Dragging?
                        mouseButton.totalMovement += moveDist;
                        if (mouseButton.totalMovement > this.dragThresholdMouse)
                        {
                            bool wasDrag = mouseButton.isDrag;

                            mouseButton.isDrag = true;
                            if (mouseButton.isHold)
                            {
                                mouseButton.wasHold = mouseButton.isHold;
                                mouseButton.isHold = false;
                            }

                            // Did it just start now?
                            if (!wasDrag)
                            {
                                if (this.startedDrag != null)
                                {
                                    this.startedDrag(mouseButton);
                                }
                            }

                            if (this.dragged != null)
                            {
                                this.dragged(mouseButton);
                            }

                            // Flick?
                            if (moveDist > this.flickThreshold)
                            {
                                mouseButton.flickVelocity =
                                    (mouseButton.flickVelocity * (1 - k_FlickAccumulationFactor)) +
                                    (mouseButton.delta * k_FlickAccumulationFactor);
                            }
                            else
                            {
                                mouseButton.flickVelocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            // Stationary?
                            if (!mouseButton.isHold &&
                                !mouseButton.isDrag &&
                                Time.realtimeSinceStartup - mouseButton.startTime >= this.holdTime)
                            {
                                mouseButton.isHold = true;
                                if (this.startedHold != null)
                                {
                                    this.startedHold(mouseButton);
                                }
                            }
                        }
                    }
                }
                else // Mouse button not up
                {
                    if (mouseButton.isDown) // Released
                    {
                        mouseButton.isDown = false;
                        // Quick enough (with no drift) to be a tap?
                        if (!mouseButton.isDrag &&
                            Time.realtimeSinceStartup - mouseButton.startTime < this.tapTime)
                        {
                            if (this.tapped != null)
                            {
                                this.tapped(mouseButton);
                            }
                        }

                        if (this.released != null)
                        {
                            this.released(mouseButton);
                        }
                    }
                }
            }

            // Mouse wheel
            if (Mathf.Abs(UnityInput.GetAxis("Mouse ScrollWheel")) > Mathf.Epsilon)
            {
                if (this.spunWheel != null)
                {
                    this.spunWheel(new WheelInfo
                    {
                        zoomAmount = UnityInput.GetAxis("Mouse ScrollWheel") * this.mouseWheelSensitivity
                    });
                }
            }
        }

        /// <summary>
        /// Update all touches
        /// </summary>
        private void UpdateTouches()
        {
            this.touchPressedThisFrame = false;
            for (int i = 0; i < UnityInput.touchCount; ++i)
            {
                Touch touch = UnityInput.GetTouch(i);

                // Find existing touch, or create new one
                TouchInfo existingTouch = this.m_Touches.FirstOrDefault(t => t.touchId == touch.fingerId);

                if (existingTouch == null)
                {
                    existingTouch = new TouchInfo
                    {
                        touchId = touch.fingerId,
                        startPosition = touch.position,
                        currentPosition = touch.position,
                        previousPosition = touch.position,
                        startTime = Time.realtimeSinceStartup,
                        startedOverUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId)
                    };

                    this.m_Touches.Add(existingTouch);

                    // Sanity check
                    Debug.Assert(touch.phase == TouchPhase.Began);
                }

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        this.touchPressedThisFrame = true;
                        if (this.pressed != null)
                        {
                            this.pressed(existingTouch);
                        }

                        break;

                    case TouchPhase.Moved:
                        bool wasDrag = existingTouch.isDrag;
                        this.UpdateMovingFinger(touch, existingTouch);

                        // Is this a drag?
                        existingTouch.isDrag = existingTouch.totalMovement >= this.dragThresholdTouch;

                        if (existingTouch.isDrag)
                        {
                            if (existingTouch.isHold)
                            {
                                existingTouch.wasHold = existingTouch.isHold;
                                existingTouch.isHold = false;
                            }

                            // Did it just start now?
                            if (!wasDrag)
                            {
                                if (this.startedDrag != null)
                                {
                                    this.startedDrag(existingTouch);
                                }
                            }

                            if (this.dragged != null)
                            {
                                this.dragged(existingTouch);
                            }

                            if (existingTouch.delta.sqrMagnitude > this.flickThreshold * this.flickThreshold)
                            {
                                existingTouch.flickVelocity =
                                    (existingTouch.flickVelocity * (1 - k_FlickAccumulationFactor)) +
                                    (existingTouch.delta * k_FlickAccumulationFactor);
                            }
                            else
                            {
                                existingTouch.flickVelocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            this.UpdateHoldingFinger(existingTouch);
                        }

                        break;

                    case TouchPhase.Canceled:
                    case TouchPhase.Ended:
                        // Could have moved a bit
                        this.UpdateMovingFinger(touch, existingTouch);
                        // Quick enough (with no drift) to be a tap?
                        if (!existingTouch.isDrag &&
                            Time.realtimeSinceStartup - existingTouch.startTime < this.tapTime)
                        {
                            if (this.tapped != null)
                            {
                                this.tapped(existingTouch);
                            }
                        }

                        if (this.released != null)
                        {
                            this.released(existingTouch);
                        }

                        // Remove from track list
                        this.m_Touches.Remove(existingTouch);
                        break;

                    case TouchPhase.Stationary:
                        this.UpdateMovingFinger(touch, existingTouch);
                        this.UpdateHoldingFinger(existingTouch);
                        existingTouch.flickVelocity = Vector2.zero;
                        break;
                }
            }

            if (this.activeTouchCount >= 2 && (this.m_Touches[0].isDrag ||
                                          this.m_Touches[1].isDrag))
            {
                if (this.pinched != null)
                {
                    this.pinched(new PinchInfo
                    {
                        touch1 = this.m_Touches[0],
                        touch2 = this.m_Touches[1]
                    });
                }
            }
        }

        /// <summary>
        /// Update a TouchInfo that might be holding
        /// </summary>
        /// <param name="existingTouch"></param>
        private void UpdateHoldingFinger(PointerActionInfo existingTouch)
        {
            if (!existingTouch.isHold &&
                !existingTouch.isDrag &&
                Time.realtimeSinceStartup - existingTouch.startTime >= this.holdTime)
            {
                existingTouch.isHold = true;
                if (this.startedHold != null)
                {
                    this.startedHold(existingTouch);
                }
            }
        }

        /// <summary>
        /// Update a TouchInfo with movement
        /// </summary>
        /// <param name="touch">The Unity touch object</param>
        /// <param name="existingTouch">The object that's tracking Unity's touch</param>
        private void UpdateMovingFinger(Touch touch, PointerActionInfo existingTouch)
        {
            float dragDist = touch.deltaPosition.magnitude;

            existingTouch.previousPosition = existingTouch.currentPosition;
            existingTouch.currentPosition = touch.position;
            existingTouch.delta = touch.deltaPosition;
            existingTouch.totalMovement += dragDist;
        }
    }
}