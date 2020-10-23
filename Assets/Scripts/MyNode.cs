using UnityEngine;
using UnityEngine.EventSystems;

public class MyNode : MonoBehaviour
{

    public Color hoverColor;
    public Color noteEnoughMoneyColor;
    public Vector3 positionOffset;

    [Header("Optional")]
    public GameObject turret;

    private Renderer rend;
    private Color startColor;

    BuildManager buildManager;

    private void Start() {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition () {
        return transform.position + positionOffset;
    }

    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (!buildManager.CanBuild)
            return;

        if (turret != null) {
            Debug.Log("Can't build there! - TODO: Dispaly on screen.");
            return;
        }

        buildManager.BuildTurretOn(this);
    }

    private void OnMouseEnter() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney) {
            rend.material.color = hoverColor;
        } else {
            rend.material.color = noteEnoughMoneyColor;
        }
        
    }

    private void OnMouseExit() {
        rend.material.color = startColor;
    }
}
