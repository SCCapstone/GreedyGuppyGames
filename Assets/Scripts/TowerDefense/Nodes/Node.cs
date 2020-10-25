// <copyright file="Node.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using TowerDefense.Agents;
using TowerDefense.MeshCreator;
using UnityEngine;

namespace TowerDefense.Nodes
{
    /// <summary>
    /// A point along the path which agents will navigate towards before recieving the next instruction from the NodeSelector
    /// Requires a collider to be added manually.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Node : MonoBehaviour
    {
        /// <summary>
        /// Reference to the MeshObject created by an AreaMeshCreator
        /// </summary>
        [HideInInspector]
        public AreaMeshCreator areaMesh;

        /// <summary>
        /// Selection weight of the node
        /// </summary>
        public int weight = 1;

        /// <summary>
        /// Gets the next node from the selector
        /// </summary>
        /// <returns>Next node, or null if this is the terminating node</returns>
        public Node GetNextNode()
        {
            var selector = this.GetComponent<NodeSelector>();
            if (selector != null)
            {
                return selector.GetNextNode();
            }

            return null;
        }

        /// <summary>
        /// Gets a random point inside the area defined by a node's meshcreator
        /// </summary>
        /// <returns>A random point within the MeshObject's area</returns>
        public Vector3 GetRandomPointInNodeArea()
        {
            // Fallback to our position if we have no mesh
            return this.areaMesh == null ? this.transform.position : this.areaMesh.GetRandomPointInside();
        }

        /// <summary>
        /// When agent enters the node area, get the next node
        /// </summary>
        public virtual void OnTriggerEnter(Collider other)
        {
            var agent = other.gameObject.GetComponent<Agent>();
            if (agent != null)
            {
                agent.GetNextNode(this);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Ensure the collider is a trigger
        /// </summary>
        protected void OnValidate()
        {
            var trigger = this.GetComponent<Collider>();
            if (trigger != null)
            {
                trigger.isTrigger = true;
            }

            // Try and find AreaMeshCreator
            if (this.areaMesh == null)
            {
                this.areaMesh = this.GetComponentInChildren<AreaMeshCreator>();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(this.transform.position + Vector3.up, "movement_node.png", true);
        }
#endif
    }
}