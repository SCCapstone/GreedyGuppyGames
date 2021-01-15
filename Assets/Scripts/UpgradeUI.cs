﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject ui;
    public RectTransform transformUI;

    [Header("Upgrade Text")]
    public Text upgrade10Text;
    public Text upgrade20Text;
    public Text upgrade30Text;
    public Text upgrade01Text;
    public Text upgrade02Text;
    public Text upgrade03Text;

    [Header("Upgrade Buttons")]
    public Button upgrade10Button;
    public Button upgrade20Button;
    public Button upgrade30Button;
    public Button upgrade01Button;
    public Button upgrade02Button;
    public Button upgrade03Button;

    private MyNode nodeToUpgrade;

    public void SetTurret(MyNode node)
    {
        //Debug.Log("Set Turret");
        nodeToUpgrade = node;
        // transform.position = nodeToUpgrade.GetBuildPosition();
        if(!node.leftNode)
        {
            transformUI.anchorMin = new Vector2(0, 0);
            transformUI.anchorMax = new Vector2(.25f, 1);
            this.Activate();
        }
        else
        {
            transformUI.anchorMin = new Vector2(.75f, 0);
            transformUI.anchorMax = new Vector2(1, 1);
            this.Activate();
        }

        upgrade01Text.text = nodeToUpgrade.turretBlueprint.upgrade01Text;
        upgrade02Text.text = nodeToUpgrade.turretBlueprint.upgrade02Text;
        upgrade03Text.text = nodeToUpgrade.turretBlueprint.upgrade03Text;
        upgrade10Text.text = nodeToUpgrade.turretBlueprint.upgrade10Text;
        upgrade20Text.text = nodeToUpgrade.turretBlueprint.upgrade20Text;
        upgrade30Text.text = nodeToUpgrade.turretBlueprint.upgrade30Text;

        if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 0)
        {
            upgrade01Button.interactable = true;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = true;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = false;
        }
        else if(nodeToUpgrade.upgradePathOne == 1 && nodeToUpgrade.upgradePathTwo == 0)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = true;
            upgrade30Button.interactable = false;
        }
        else if (nodeToUpgrade.upgradePathOne == 2 && nodeToUpgrade.upgradePathTwo == 0)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = true;
        }
        else if (nodeToUpgrade.upgradePathOne == 3 && nodeToUpgrade.upgradePathTwo == 0)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = false;
        }
        else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 1)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = true;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = false;
        }
        else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 2)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = true;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = false;
        }
        else if (nodeToUpgrade.upgradePathOne == 0 && nodeToUpgrade.upgradePathTwo == 3)
        {
            upgrade01Button.interactable = false;
            upgrade02Button.interactable = false;
            upgrade03Button.interactable = false;
            upgrade10Button.interactable = false;
            upgrade20Button.interactable = false;
            upgrade30Button.interactable = false;
        }
        else
        {
            Debug.LogError("upgrade settings wrong");
        }
    }

    public void Activate()
    {
        //Debug.Log("activating");
        ui.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("Hiding");
        ui.SetActive(false);
    }

    public void UpgradePathOne()
    {
        if(nodeToUpgrade.upgradePathTwo > 0)
        {
            return;
        }
        //Debug.Log("UpgradePath1");
        nodeToUpgrade.upgradePathOne++;
        nodeToUpgrade.UpgradeTurret();

        this.SetTurret(nodeToUpgrade);
    }

    public void UpgradePathTwo()
    {
        if (nodeToUpgrade.upgradePathOne > 0)
        {
            return;
        }
        //Debug.Log("UpgradePath2");
        nodeToUpgrade.upgradePathTwo++;
        nodeToUpgrade.UpgradeTurret();

        this.SetTurret(nodeToUpgrade);
    }
}
