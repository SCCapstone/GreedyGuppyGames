using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public GameObject ui;

    private MyNode nodeToUpgrade;

    public void SetTurret(MyNode node)
    {
        Debug.Log("Set Turret");
        nodeToUpgrade = node;
        transform.position = nodeToUpgrade.GetBuildPosition();
        this.Activate();
    }

    public void Activate()
    {
        Debug.Log("activating");
        ui.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("Hiding");
        ui.SetActive(false);
    }

    public void UpgradePathOne()
    {
        Debug.Log("UpgradePath1");
        nodeToUpgrade.upgradePathOne++;
        nodeToUpgrade.UpgradeTurret();
    }

    public void UpgradePathTwo()
    {
        Debug.Log("UpgradePath2");
        nodeToUpgrade.upgradePathTwo++;
        nodeToUpgrade.UpgradeTurret();
    }
}
