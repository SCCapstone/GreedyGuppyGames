// <copyright file="TowerPlacementGhost.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Towers;
using UnityEngine;

namespace TowerDefense.UI.HUD
{
    /// <summary>
    /// Tower placement "ghost" that indicates the position of the tower to be placed and its validity for placement.
    /// This is built with mouse in mind for testing, but it should be possible to abstract a lot of this into a child
    /// class for the purposes of a touch UI.
    ///
    /// Should exist on its own layer to ensure best placement.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TowerPlacementGhost : MonoBehaviour
    {
        /// <summary>
        /// The tower we represent
        /// </summary>
        public Tower controller { get; private set; }

        /// <summary>
        /// Prefab used to visualize effect radius of tower
        /// </summary>
        public GameObject radiusVisualizer;

        /// <summary>
        /// Offset height for radius visualizer
        /// </summary>
        public float radiusVisualizerHeight = 0.02f;

        /// <summary>
        /// Movement damping factor
        /// </summary>
        public float dampSpeed = 0.075f;

        /// <summary>
        /// The two materials used to represent valid and invalid placement, respectively
        /// </summary>
        public Material material;

        public Material invalidPositionMaterial;

        /// <summary>
        /// The list of attached mesh renderers
        /// </summary>
        protected MeshRenderer[] m_MeshRenderers;

        /// <summary>
        /// Movement velocity for smooth damping
        /// </summary>
        protected Vector3 m_MoveVel;

        /// <summary>
        /// Target world position
        /// </summary>
        protected Vector3 m_TargetPosition;

        /// <summary>
        /// True if we're at a valid world position
        /// </summary>
        protected bool m_ValidPos;

        /// <summary>
        /// The attached the collider
        /// </summary>
        public Collider ghostCollider { get; private set; }

        /// <summary>
        /// Initialize this ghost
        /// </summary>
        /// <param name="tower">The tower controller we're a ghost of</param>
        public virtual void Initialize(Tower tower)
        {
            this.m_MeshRenderers = this.GetComponentsInChildren<MeshRenderer>();
            this.controller = tower;
            if (GameUI.instanceExists)
            {
                GameUI.instance.SetupRadiusVisualizer(this.controller, this.transform);
            }

            this.ghostCollider = this.GetComponent<Collider>();
            this.m_MoveVel = Vector3.zero;
            this.m_ValidPos = false;
        }

        /// <summary>
        /// Hide this ghost
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show this ghost
        /// </summary>
        public virtual void Show()
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
                this.m_MoveVel = Vector3.zero;

                this.m_ValidPos = false;
            }
        }

        /// <summary>
        /// Moves this ghost to a given world position
        /// </summary>
        /// <param name="worldPosition">The new position to move to in world space</param>
        /// <param name="rotation">The new rotation to adopt, in world space</param>
        /// <param name="validLocation">Whether or not this position is valid. Ghost may display differently
        /// over invalid locations</param>
        public virtual void Move(Vector3 worldPosition, Quaternion rotation, bool validLocation)
        {
            this.m_TargetPosition = worldPosition;

            if (!this.m_ValidPos)
            {
                // Immediately move to the given position
                this.m_ValidPos = true;
                this.transform.position = this.m_TargetPosition;
            }

            this.transform.rotation = rotation;
            foreach (MeshRenderer meshRenderer in this.m_MeshRenderers)
            {
                meshRenderer.sharedMaterial = validLocation ? this.material : this.invalidPositionMaterial;
            }
        }

        /// <summary>
        /// Damp the movement of the ghost
        /// </summary>
        protected virtual void Update()
        {
            Vector3 currentPos = this.transform.position;

            if (Vector3.SqrMagnitude(currentPos - this.m_TargetPosition) > 0.01f)
            {
                currentPos = Vector3.SmoothDamp(currentPos, this.m_TargetPosition, ref this.m_MoveVel, this.dampSpeed);

                this.transform.position = currentPos;
            }
            else
            {
                this.m_MoveVel = Vector3.zero;
            }
        }
    }
}