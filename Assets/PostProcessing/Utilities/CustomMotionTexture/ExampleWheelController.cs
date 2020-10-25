// <copyright file="ExampleWheelController.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class ExampleWheelController : MonoBehaviour
{
    public float acceleration;
    public Renderer motionVectorRenderer; // Reference to the custom motion vector renderer

    private Rigidbody m_Rigidbody;

    private static class Uniforms
    {
        internal static readonly int _MotionAmount = Shader.PropertyToID("_MotionAmount");
    }

    private void Start()
    {
        this.m_Rigidbody = this.GetComponent<Rigidbody>(); // Get reference to rigidbody
        this.m_Rigidbody.maxAngularVelocity = 100; // Set max velocity for rigidbody
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) // Rotate forward
        {
            this.m_Rigidbody.AddRelativeTorque(new Vector3(-1 * this.acceleration, 0, 0), ForceMode.Acceleration); // Add forward torque to mesh
        }
        else if (Input.GetKey(KeyCode.DownArrow)) // Rotate backward
        {
            this.m_Rigidbody.AddRelativeTorque(new Vector3(1 * this.acceleration, 0, 0), ForceMode.Acceleration); // Add backward torque to mesh
        }

        float m = -this.m_Rigidbody.angularVelocity.x / 100; // Calculate multiplier for motion vector texture

        if (this.motionVectorRenderer) // If the custom motion vector texture renderer exists
        {
            this.motionVectorRenderer.material.SetFloat(Uniforms._MotionAmount, Mathf.Clamp(m, -0.25f, 0.25f)); // Set the multiplier on the renderer's material
        }
    }
}
