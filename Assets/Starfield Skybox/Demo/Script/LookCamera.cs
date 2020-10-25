// <copyright file="LookCamera.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class LookCamera : MonoBehaviour
{
    public float speedNormal = 10.0f;
    public float speedFast = 50.0f;

    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;
    private float rotY = 0.0f;

    private void Start()
    {
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    private void Update()
    {
        // rotation
        if (Input.GetMouseButton(1))
        {
            float rotX = this.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.mouseSensitivityX;
            this.rotY += Input.GetAxis("Mouse Y") * this.mouseSensitivityY;
            this.rotY = Mathf.Clamp(this.rotY, -89.5f, 89.5f);
            this.transform.localEulerAngles = new Vector3(-this.rotY, rotX, 0.0f);
        }

        if (Input.GetKey(KeyCode.U))
        {
            this.gameObject.transform.localPosition = new Vector3(0.0f, 3500.0f, 0.0f);
        }
    }
}
