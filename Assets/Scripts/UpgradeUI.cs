using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public GameObject ui;

    private MyNode nodeToUpgrade;

    public void SetTurret(MyNode node)
    {
        nodeToUpgrade = node;
        transform.position = nodeToUpgrade.GetBuildPosition();
        ui.SetActive(true);
    }

    public void Hide()
    {
        ui.SetActive(false);
    }

    public void UpgradePathOne()
    {
        nodeToUpgrade.upgradePathOne++;
        nodeToUpgrade.UpgradeTurret();
    }

    public void UpgradePathTwo()
    {
        nodeToUpgrade.upgradePathTwo++;
        nodeToUpgrade.UpgradeTurret();
    }
}
