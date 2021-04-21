using System;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class PortalFX_AxisRotateByTime : MonoBehaviour {

    public Vector3 RotateAxis = new Vector3(1, 5, 10);

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        transform.Rotate(RotateAxis * Time.deltaTime);
	}
}
