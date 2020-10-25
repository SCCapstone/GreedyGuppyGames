// <copyright file="TouchInput.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Core.Input
{
    /// <summary>
    /// Base control scheme for touch devices, which performs CameraRig control
    /// </summary>
    public class TouchInput : CameraInputScheme
    {
        /// <summary>
        /// Configuration of the pan speed
        /// </summary>
        public float panSpeed = 5;

        /// <summary>
        /// How quickly flicks decay
        /// </summary>
        public float flickDecayFactor = 0.2f;

        /// <summary>
        /// Flick direction
        /// </summary>
        private Vector3 m_FlickDirection;

        /// <summary>
        /// Gets whether the scheme should be activated or not
        /// </summary>
        public override bool shouldActivate
        {
            get { return UnityInput.touchCount > 0; }
        }

        /// <summary>
        /// This default scheme on IOS and Android devices
        /// </summary>
        public override bool isDefault
        {
            get
            {
#if UNITY_IOS || UNITY_ANDROID
				return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Register input events
        /// </summary>
        protected virtual void OnEnable()
        {
            if (!InputController.instanceExists)
            {
                Debug.LogError("[UI] Keyboard and Mouse UI requires InputController");
                return;
            }

            // Register drag event
            InputController inputController = InputController.instance;
            inputController.pressed += this.OnPress;
            inputController.released += this.OnRelease;
            inputController.dragged += this.OnDrag;
            inputController.pinched += this.OnPinch;
        }

        /// <summary>
        /// Deregister input events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (!InputController.instanceExists)
            {
                return;
            }

            if (InputController.instanceExists)
            {
                InputController inputController = InputController.instance;
                inputController.pressed -= this.OnPress;
                inputController.released -= this.OnRelease;
                inputController.dragged -= this.OnDrag;
                inputController.pinched -= this.OnPinch;
            }
        }

        /// <summary>
        /// Perform flick and zoom
        /// </summary>
        protected virtual void Update()
        {
            if (this.cameraRig != null)
            {
                this.UpdateFlick();
                this.DecayZoom();
            }
        }

        /// <summary>
        /// Called on input press
        /// </summary>
        protected virtual void OnPress(PointerActionInfo pointer)
        {
            if (this.cameraRig != null)
            {
                this.DoFlickCatch(pointer);
            }
        }

        /// <summary>
        /// Called on input release
        /// </summary>
        protected virtual void OnRelease(PointerActionInfo pointer)
        {
            if (this.cameraRig != null)
            {
                this.DoReleaseFlick(pointer);
            }
        }

        /// <summary>
        /// Called when we drag
        /// </summary>
        protected virtual void OnDrag(PointerActionInfo pointer)
        {
            // Drag panning for touch input
            if (this.cameraRig != null)
            {
                this.DoDragPan(pointer);
            }
        }

        /// <summary>
        /// Called on pinch gestures
        /// </summary>
        protected virtual void OnPinch(PinchInfo pinch)
        {
            if (this.cameraRig != null)
            {
                this.DoPinchZoom(pinch);
            }
        }

        /// <summary>
        /// Update current flick velocity
        /// </summary>
        protected void UpdateFlick()
        {
            // Flick?
            if (this.m_FlickDirection.sqrMagnitude > Mathf.Epsilon)
            {
                this.cameraRig.PanCamera(this.m_FlickDirection * Time.deltaTime);
                this.m_FlickDirection *= this.flickDecayFactor;
            }
        }

        /// <summary>
        /// Decay the zoom if no touches are active
        /// </summary>
        protected void DecayZoom()
        {
            if (InputController.instance.activeTouchCount == 0)
            {
                this.cameraRig.ZoomDecay();
            }
        }

        /// <summary>
        /// "Catch" flicks on press, to stop the panning momentum
        /// </summary>
        /// <param name="pointer">The press pointer event</param>
        protected void DoFlickCatch(PointerActionInfo pointer)
        {
            var touchInfo = pointer as TouchInfo;
            // Stop flicks on touch
            if (touchInfo != null)
            {
                this.m_FlickDirection = Vector2.zero;
                this.cameraRig.StopTracking();
            }
        }

        /// <summary>
        /// Do flicks, on release only
        /// </summary>
        /// <param name="pointer">The release pointer event</param>
        protected void DoReleaseFlick(PointerActionInfo pointer)
        {
            var touchInfo = pointer as TouchInfo;

            if (touchInfo != null && touchInfo.flickVelocity.sqrMagnitude > Mathf.Epsilon)
            {
                // We have a flick!
                // Work out velocity from motion
                Ray prevRay = this.cameraRig.cachedCamera.ScreenPointToRay(pointer.currentPosition -
                                                                        pointer.flickVelocity);
                Ray currRay = this.cameraRig.cachedCamera.ScreenPointToRay(pointer.currentPosition);

                Vector3 startPoint = Vector3.zero;
                Vector3 endPoint = Vector3.zero;
                float dist;

                if (this.cameraRig.floorPlane.Raycast(prevRay, out dist))
                {
                    startPoint = prevRay.GetPoint(dist);
                }

                if (this.cameraRig.floorPlane.Raycast(currRay, out dist))
                {
                    endPoint = currRay.GetPoint(dist);
                }

                // Work out that movement in units per second
                this.m_FlickDirection = (startPoint - endPoint) / Time.deltaTime;
            }
        }

        /// <summary>
        /// Controls the pan with a drag
        /// </summary>
        protected void DoDragPan(PointerActionInfo pointer)
        {
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null)
            {
                // Work out movement amount by raycasting onto floor plane from delta positions
                // and getting that distance
                Ray currRay = this.cameraRig.cachedCamera.ScreenPointToRay(touchInfo.currentPosition);

                Vector3 endPoint = Vector3.zero;
                float dist;
                if (this.cameraRig.floorPlane.Raycast(currRay, out dist))
                {
                    endPoint = currRay.GetPoint(dist);
                }

                // Pan
                Ray prevRay = this.cameraRig.cachedCamera.ScreenPointToRay(touchInfo.previousPosition);
                Vector3 startPoint = Vector3.zero;

                if (this.cameraRig.floorPlane.Raycast(prevRay, out dist))
                {
                    startPoint = prevRay.GetPoint(dist);
                }

                Vector3 panAmount = startPoint - endPoint;
                // If this is a touch, we divide the pan amount by the number of touches
                if (UnityInput.touchCount > 0)
                {
                    panAmount /= UnityInput.touchCount;
                }

                this.PanCamera(panAmount);
            }
        }

        /// <summary>
        /// Perform a zoom with the given pinch
        /// </summary>
        protected void DoPinchZoom(PinchInfo pinch)
        {
            float currentDistance = (pinch.touch1.currentPosition - pinch.touch2.currentPosition).magnitude;
            float prevDistance = (pinch.touch1.previousPosition - pinch.touch2.previousPosition).magnitude;

            float zoomChange = prevDistance / currentDistance;
            float prevZoomDist = this.cameraRig.zoomDist;

            this.cameraRig.SetZoom(zoomChange * this.cameraRig.rawZoomDist);

            // Calculate actual zoom change after clamping
            zoomChange = this.cameraRig.zoomDist / prevZoomDist;

            // First get floor position of middle of gesture
            Vector2 averageScreenPos = (pinch.touch1.currentPosition + pinch.touch2.currentPosition) * 0.5f;
            Ray ray = this.cameraRig.cachedCamera.ScreenPointToRay(averageScreenPos);

            Vector3 worldPos = Vector3.zero;
            float dist;

            if (this.cameraRig.floorPlane.Raycast(ray, out dist))
            {
                worldPos = ray.GetPoint(dist);
            }

            // Vector from our current look pos to this point
            Vector3 offsetValue = worldPos - this.cameraRig.lookPosition;

            // Pan towards or away from our zoom center
            this.PanCamera(offsetValue * (1 - zoomChange));
        }

        /// <summary>
        /// Pans the camera
        /// </summary>
        /// <param name="panAmount">
        /// The vector to pan
        /// </param>
        protected void PanCamera(Vector3 panAmount)
        {
            this.cameraRig.StopTracking();
            this.cameraRig.PanCamera(panAmount);
        }
    }
}