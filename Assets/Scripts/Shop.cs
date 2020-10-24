using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint standardTurret;
    public TurretBlueprint anotherTurret;

    BuildManager buildManager;

    void Start() {
        buildManager = BuildManager.instance;
    }
    public void SelectBasicTurret() {
        Debug.Log("Standard Turret Selected");
        buildManager.SelectTurretToBuild(standardTurret);
    }

    public void SelectCannonTurret() {
        Debug.Log("Another Turret Selected");
        buildManager.SelectTurretToBuild(anotherTurret);
    }
}
