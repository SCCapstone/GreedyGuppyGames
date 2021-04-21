using UnityEngine;
using System.Collections;

public class PortalsFX_RandomMoves : MonoBehaviour {
	public float TimeMultipler = 0.2f;
	public float RangeMultipler = 0.2f;

	Transform t;
	Vector3 StartPosition;
	Vector3 RndArg1;
	Vector3 RndArg2;
	Vector3 RndArg3;

	// Use this for initialization
	void Start () {
		t = GetComponent<Transform> ();
		StartPosition = t.position;
		RndArg1 = Random.insideUnitSphere * 2;
		RndArg2 = Random.insideUnitSphere * 2;
		RndArg3 = Random.insideUnitSphere * 2;
	}

	// Update is called once per frame
	void Update () {
		var x = Time.time * TimeMultipler;
		var xPos = Mathf.Sin (Mathf.Sin (x*RndArg1.x) + x * RndArg1.y) + Mathf.Cos (Mathf.Cos (x) * RndArg1.z + x);
		var yPos = Mathf.Sin (Mathf.Sin (x*RndArg2.x) + x * RndArg2.y) + Mathf.Cos (Mathf.Cos (x) * RndArg2.z + x);
		var zPos = Mathf.Sin (Mathf.Sin (x*RndArg3.x) + x * RndArg3.y) + Mathf.Cos (Mathf.Cos (x) * RndArg3.z + x);
		t.position = StartPosition + new Vector3(xPos, yPos, zPos) * RangeMultipler;
		//r.AddForce((StartPosition - new Vector3(xPos, yPos, zPos)) * RangeMultipler);
	}
}
