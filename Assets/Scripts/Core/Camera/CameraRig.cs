// <copyright file="CameraRig.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.Input;
using UnityEngine;

namespace Core.Camera
{
    /// <summary>
    /// Class to control the camera's behaviour. Camera rig currently operates best on terrain that is mostly on
    /// a single plane
    /// </summary>
    public class CameraRig : MonoBehaviour
    {
        /// <summary>
        /// Look dampening factor
        /// </summary>
        public float lookDampFactor;

        /// <summary>
        /// Movement dampening factor
        /// </summary>
        public float movementDampFactor;

        /// <summary>
        /// Nearest zoom level - can go a bit further than this on touch, for springiness
        /// </summary>
        public float nearestZoom = 15;

        /// <summary>
        /// Furthest zoom level - can go a bit further than this on touch, for springiness
        /// </summary>
        public float furthestZoom = 40;

        /// <summary>
        /// True maximum zoom level
        /// </summary>
        public float maxZoom = 60;

        /// <summary>
        /// Logarithm used to decay zoom beyond furthest
        /// </summary>
        public float zoomLogFactor = 10;

        /// <summary>
        /// How fast zoom recovers to normal
        /// </summary>
        public float zoomRecoverSpeed = 20;

        /// <summary>
        /// Y-height of the floor the camera is assuming
        /// </summary>
        public float floorY;

        /// <summary>
        /// Camera angle when fully zoomed in
        /// </summary>
        public Transform zoomedCamAngle;

        /// <summary>
        /// Map size, edited through the CameraRigEditor script in edit mode
        /// </summary>
        [HideInInspector]
        public Rect mapSize = new Rect(-10, -10, 20, 20);

        /// <summary>
        /// Is the zoom able to exceed its normal zoom extents with a rubber banding effect
        /// </summary>
        public bool springyZoom = true;

        /// <summary>
        /// Current look velocity of camera
        /// </summary>
        private Vector3 m_CurrentLookVelocity;

        /// <summary>
        /// Rotations of camera at various zoom levels
        /// </summary>
        private Quaternion m_MinZoomRotation;
        private Quaternion m_MaxZoomRotation;

        /// <summary>
        /// Current camera velocity
        /// </summary>
        private Vector3 m_CurrentCamVelocity;

        /// <summary>
        /// Current reusable floor plane
        /// </summary>
        private Plane m_FloorPlane;

        public Plane floorPlane
        {
            get { return this.m_FloorPlane; }
        }

        /// <summary>
        /// Target position on the grid that we're looking at
        /// </summary>
        public Vector3 lookPosition { get; private set; }

        /// <summary>
        /// Current look position of camera
        /// </summary>
        public Vector3 currentLookPosition { get; private set; }

        /// <summary>
        /// Target position of the camera
        /// </summary>
        public Vector3 cameraPosition { get; private set; }

        /// <summary>
        /// Bounds of our look area, related to map size, zoom level and aspect ratio/screen size
        /// </summary>
        public Rect lookBounds { get; private set; }

        /// <summary>
        /// Gets our current zoom distance
        /// </summary>
        public float zoomDist { get; private set; }

        /// <summary>
        /// Gets our current internal zoom distance, before clamping and scaling is applied
        /// </summary>
        public float rawZoomDist { get; private set; }

        /// <summary>
        /// Gets the unit we're tracking if any
        /// </summary>
        public GameObject trackingObject { get; private set; }

        /// <summary>
        /// Cached camera component
        /// </summary>
        public UnityEngine.Camera cachedCamera { get; private set; }

        /// <summary>
        /// Initialize references and floor plane
        /// </summary>
        protected virtual void Awake()
        {
            this.cachedCamera = this.GetComponent<UnityEngine.Camera>();
            this.m_FloorPlane = new Plane(Vector3.up, new Vector3(0.0f, this.floorY, 0.0f));

            // Set initial values
            var lookRay = new Ray(this.cachedCamera.transform.position, this.cachedCamera.transform.forward);

            float dist;
            if (this.m_FloorPlane.Raycast(lookRay, out dist))
            {
                this.currentLookPosition = this.lookPosition = lookRay.GetPoint(dist);
            }

            this.cameraPosition = this.cachedCamera.transform.position;

            this.m_MinZoomRotation = Quaternion.FromToRotation(Vector3.up, -this.cachedCamera.transform.forward);
            this.m_MaxZoomRotation = Quaternion.FromToRotation(Vector3.up, -this.zoomedCamAngle.transform.forward);
            this.rawZoomDist = this.zoomDist = (this.currentLookPosition - this.cameraPosition).magnitude;
        }

        /// <summary>
        /// Setup initial zoom level and camera bounds
        /// </summary>
        protected virtual void Start()
        {
            this.RecalculateBoundingRect();
        }

        /// <summary>
        /// Handle camera behaviour
        /// </summary>
        protected virtual void Update()
        {
            this.RecalculateBoundingRect();

            // Tracking?
            if (this.trackingObject != null)
            {
                this.PanTo(this.trackingObject.transform.position);

                if (!this.trackingObject.activeInHierarchy)
                {
                    this.StopTracking();
                }
            }

            // Approach look position
            this.currentLookPosition = Vector3.SmoothDamp(this.currentLookPosition, this.lookPosition, ref this.m_CurrentLookVelocity,
                                                     this.lookDampFactor);

            Vector3 worldPos = this.transform.position;
            worldPos = Vector3.SmoothDamp(worldPos, this.cameraPosition, ref this.m_CurrentCamVelocity,
                                          this.movementDampFactor);

            this.transform.position = worldPos;
            this.transform.LookAt(this.currentLookPosition);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Debug bounds area gizmo
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // We dont want to display this in edit mode
            if (!Application.isPlaying)
            {
                return;
            }

            if (this.cachedCamera == null)
            {
                this.cachedCamera = this.GetComponent<UnityEngine.Camera>();
            }

            this.RecalculateBoundingRect();

            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                new Vector3(this.lookBounds.xMin, 0.0f, this.lookBounds.yMin),
                new Vector3(this.lookBounds.xMax, 0.0f, this.lookBounds.yMin));
            Gizmos.DrawLine(
                new Vector3(this.lookBounds.xMin, 0.0f, this.lookBounds.yMin),
                new Vector3(this.lookBounds.xMin, 0.0f, this.lookBounds.yMax));
            Gizmos.DrawLine(
                new Vector3(this.lookBounds.xMax, 0.0f, this.lookBounds.yMax),
                new Vector3(this.lookBounds.xMin, 0.0f, this.lookBounds.yMax));
            Gizmos.DrawLine(
                new Vector3(this.lookBounds.xMax, 0.0f, this.lookBounds.yMax),
                new Vector3(this.lookBounds.xMax, 0.0f, this.lookBounds.yMin));

            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(this.transform.position, this.currentLookPosition);
        }
#endif

        /// <summary>
        /// Pans the camera to a specific position
        /// </summary>
        /// <param name="position">The look target</param>
        public void PanTo(Vector3 position)
        {
            Vector3 pos = position;

            // Look position is floor height
            pos.y = this.floorY;

            // Clamp to look bounds
            pos.x = Mathf.Clamp(pos.x, this.lookBounds.xMin, this.lookBounds.xMax);
            pos.z = Mathf.Clamp(pos.z, this.lookBounds.yMin, this.lookBounds.yMax);
            this.lookPosition = pos;

            // Camera position calculated from look position with view vector and zoom dist
            this.cameraPosition = this.lookPosition + (this.GetToCamVector() * this.zoomDist);
        }

        /// <summary>
        /// Cause the camera to follow a unit
        /// </summary>
        /// <param name="objectToTrack"></param>
        public void TrackObject(GameObject objectToTrack)
        {
            this.trackingObject = objectToTrack;
            this.PanTo(this.trackingObject.transform.position);
        }

        /// <summary>
        /// Stop tracking a unit
        /// </summary>
        public void StopTracking()
        {
            this.trackingObject = null;
        }

        /// <summary>
        /// Pan the camera
        /// </summary>
        /// <param name="panDelta">How far to pan the camera, in world space units</param>
        public void PanCamera(Vector3 panDelta)
        {
            Vector3 pos = this.lookPosition;
            pos += panDelta;

            // Clamp to look bounds
            pos.x = Mathf.Clamp(pos.x, this.lookBounds.xMin, this.lookBounds.xMax);
            pos.z = Mathf.Clamp(pos.z, this.lookBounds.yMin, this.lookBounds.yMax);
            this.lookPosition = pos;

            // Camera position calculated from look position with view vector and zoom dist
            this.cameraPosition = this.lookPosition + (this.GetToCamVector() * this.zoomDist);
        }

        /// <summary>
        /// Zoom the camera by a specified value
        /// </summary>
        /// <param name="zoomDelta">How far to zoom the camera</param>
        public void ZoomCameraRelative(float zoomDelta)
        {
            this.SetZoom(this.rawZoomDist + zoomDelta);
        }

        /// <summary>
        /// Zoom the camera to a specified value
        /// </summary>
        /// <param name="newZoom">The absolute zoom value</param>
        public void SetZoom(float newZoom)
        {
            if (this.springyZoom)
            {
                this.rawZoomDist = newZoom;

                if (newZoom > this.furthestZoom)
                {
                    this.zoomDist = this.furthestZoom;
                    this.zoomDist += Mathf.Log((Mathf.Min(this.rawZoomDist, this.maxZoom) - this.furthestZoom) + 1, this.zoomLogFactor);
                }
                else if (this.rawZoomDist < this.nearestZoom)
                {
                    this.zoomDist = this.nearestZoom;
                    this.zoomDist -= Mathf.Log((this.nearestZoom - this.rawZoomDist) + 1, this.zoomLogFactor);
                }
                else
                {
                    this.zoomDist = this.rawZoomDist;
                }
            }
            else
            {
                this.zoomDist = this.rawZoomDist = Mathf.Clamp(newZoom, this.nearestZoom, this.furthestZoom);
            }

            // Update bounding rectangle, which is based on our zoom level
            this.RecalculateBoundingRect();

            // Force recalculated CameraPosition
            this.PanCamera(Vector3.zero);
        }

        /// <summary>
        /// Calculates the ray for a specified pointer in 3d space
        /// </summary>
        /// <param name="pointer">The pointer info</param>
        /// <returns>The ray representing a screen-space pointer in 3D space</returns>
        public Ray GetRayForPointer(PointerInfo pointer)
        {
            return this.cachedCamera.ScreenPointToRay(pointer.currentPosition);
        }

        /// <summary>
        /// Gets the screen position of a given world position
        /// </summary>
        /// <param name="worldPos">The world position</param>
        /// <returns>The screen position of that point</returns>
        public Vector3 GetScreenPos(Vector3 worldPos)
        {
            return this.cachedCamera.WorldToScreenPoint(worldPos);
        }

        /// <summary>
        /// Decay the zoom if it's beyond its zoom limits, for springiness
        /// </summary>
        public void ZoomDecay()
        {
            if (this.springyZoom)
            {
                if (this.rawZoomDist > this.furthestZoom)
                {
                    float recover = this.rawZoomDist - this.furthestZoom;
                    this.SetZoom(Mathf.Max(this.furthestZoom, this.rawZoomDist - (recover * this.zoomRecoverSpeed * Time.deltaTime)));
                }
                else if (this.rawZoomDist < this.nearestZoom)
                {
                    float recover = this.nearestZoom - this.rawZoomDist;
                    this.SetZoom(Mathf.Min(this.nearestZoom, this.rawZoomDist + (recover * this.zoomRecoverSpeed * Time.deltaTime)));
                }
            }
        }

        /// <summary>
        /// Returns our normalized zoom ratio
        /// </summary>
        public float CalculateZoomRatio()
        {
            return Mathf.Clamp01(Mathf.InverseLerp(this.nearestZoom, this.furthestZoom, this.zoomDist));
        }

        /// <summary>
        /// Gets the to camera vector based on our current zoom level
        /// </summary>
        private Vector3 GetToCamVector()
        {
            float t = Mathf.Clamp01((this.zoomDist - this.nearestZoom) / (this.furthestZoom - this.nearestZoom));
            t = 1 - ((1 - t) * (1 - t));
            Quaternion interpolatedRotation = Quaternion.Slerp(
                this.m_MaxZoomRotation, this.m_MinZoomRotation,
                t);
            return interpolatedRotation * Vector3.up;
        }

        /// <summary>
        /// Update the size of our camera's bounding rectangle
        /// </summary>
        private void RecalculateBoundingRect()
        {
            Rect mapsize = this.mapSize;

            // Get some world space projections at this zoom level
            // Temporarily move camera to final look position
            Vector3 prevCameraPos = this.transform.position;
            this.transform.position = this.cameraPosition;
            this.transform.LookAt(this.lookPosition);

            // Project screen corners and center
            var bottomLeftScreen = new Vector3(0, 0);
            var topLeftScreen = new Vector3(0, Screen.height);
            var centerScreen = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);

            Vector3 bottomLeftWorld = Vector3.zero;
            Vector3 topLeftWorld = Vector3.zero;
            Vector3 centerWorld = Vector3.zero;
            float dist;

            Ray ray = this.cachedCamera.ScreenPointToRay(bottomLeftScreen);
            if (this.m_FloorPlane.Raycast(ray, out dist))
            {
                bottomLeftWorld = ray.GetPoint(dist);
            }

            ray = this.cachedCamera.ScreenPointToRay(topLeftScreen);
            if (this.m_FloorPlane.Raycast(ray, out dist))
            {
                topLeftWorld = ray.GetPoint(dist);
            }

            ray = this.cachedCamera.ScreenPointToRay(centerScreen);
            if (this.m_FloorPlane.Raycast(ray, out dist))
            {
                centerWorld = ray.GetPoint(dist);
            }

            Vector3 toTopLeft = topLeftWorld - centerWorld;
            Vector3 toBottomLeft = bottomLeftWorld - centerWorld;

            this.lookBounds = new Rect(
                mapsize.xMin - toBottomLeft.x,
                mapsize.yMin - toBottomLeft.z,
                Mathf.Max(mapsize.width + (toBottomLeft.x * 2), 0),
                Mathf.Max((mapsize.height - toTopLeft.z) + toBottomLeft.z, 0));

            // Restore camera position
            this.transform.position = prevCameraPos;
            this.transform.LookAt(this.currentLookPosition);
        }
    }
}