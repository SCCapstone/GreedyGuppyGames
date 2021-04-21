using UnityEngine;
using System.Collections;

public class PortalDemoGUI: MonoBehaviour
{

	public GameObject[] Prefabs;

	private int currentNomber;
	private GameObject currentInstance;
	private GUIStyle guiStyleHeader = new GUIStyle();
    float dpiScale;

	void Start () {
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;
        guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = new Color(0.15f,0.15f,0.15f);
		currentInstance = Instantiate(Prefabs[currentNomber], transform.position, new Quaternion()) as GameObject;
	}

	private void OnGUI()
	{
        if (GUI.Button(new Rect(10 * dpiScale, 15 * dpiScale, 135 * dpiScale, 37 * dpiScale), "PREVIOUS"))
        {
			ChangeCurrent(-1);
		}
        if (GUI.Button(new Rect(160 * dpiScale, 15 * dpiScale, 135 * dpiScale, 37 * dpiScale), "NEXT"))
		{
			ChangeCurrent(+1);
		}
	}
	
	void ChangeCurrent(int delta) {
		currentNomber+=delta;
		if (currentNomber> Prefabs.Length - 1)
			currentNomber = 0;
		else if (currentNomber < 0)
			currentNomber = Prefabs.Length - 1;
		if(currentInstance!=null) Destroy(currentInstance);
		currentInstance = Instantiate(Prefabs[currentNomber], transform.position, new Quaternion()) as GameObject;
	}
}
