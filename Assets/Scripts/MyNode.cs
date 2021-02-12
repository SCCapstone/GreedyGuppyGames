// <copyright file="MyNode.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.EventSystems;

public class MyNode : MonoBehaviour
{
    public Color hoverColor;
    public Color nodeSelectedToUpgradeColor;
    public Color errorColor;
    public bool leftNode = true;
    
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public int upgradePathOne = 0;
    [HideInInspector]
    public int upgradePathTwo = 0;
    [HideInInspector]
    public int moneySpentOnTurret = 0;


    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;

    private void Start()
    {
        this.rend = this.GetComponent<Renderer>();
        this.startColor = this.rend.material.color;
        this.buildManager = BuildManager.instance;
    }
    public Vector3 GetBuildPosition()
    {
        return this.transform.position + this.positionOffset;
    }

    private void OnMouseDown()
    {
        //Debug.Log("I ran onmousedown");
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!this.buildManager.CanBuild)
        {
            return;
        }

        if (this.turret != null)
        {
            // Debug.Log("Can't build there! - TODO: Dispaly on screen.");
            return;
        }

        this.BuildTurret(buildManager.GetTurretBlueprint());
        this.buildManager.ResetTurretToBuild();
        this.rend.material.color = this.startColor;
    }

    private void OnMouseEnter()
    {
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!this.buildManager.CanBuild)
        {
            return;
        }

        if (this.buildManager.HasMoney)
        {
            if(this.turret == null)
            {
                this.rend.material.color = this.hoverColor;
            }
            else
            {
                this.rend.material.color = this.errorColor;
            }
        }
        else
        {
            this.rend.material.color = this.errorColor;
        }
    }

    // destroys the turret in the game then sets the turret to null
    public void DeleteTurret()
    {
        Destroy(turret);
        turret = null;
    }
    public void SelectForUpgradeColor()
    {
        this.rend.material.color = this.nodeSelectedToUpgradeColor;
    }
    public void ResetColor()
    {
        this.rend.material.color = this.startColor;
    }

    private void OnMouseExit()
    {
        // If the MyNode script is unchecked then we return
        if (this.gameObject.GetComponent<MyNode>().enabled == false)
        {
            return;
        }
        if (this.rend.material.color == this.nodeSelectedToUpgradeColor)
        {
            return;
        }
        this.ResetColor();
    }

    private void OnMouseOver()
    {
        // changes the color back if right clicked to deselect tower on the node
        if (Input.GetMouseButtonDown(1))
        {
            this.ResetColor();
        }
    }


    private void BuildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            // Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= blueprint.cost;
        moneySpentOnTurret += blueprint.cost;

        // Build a turret
        // GameObject turretToBuild = buildManager.GetTurretToBuild();
        GameObject aTurret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = aTurret;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        turretBlueprint = blueprint;
        // Debug.Log("Turret build! Money left: " + PlayerStats.Money);
    }

    public void UpgradeTurret()
    {
        
        Debug.Log("CalledUpgrade");
        GameObject upgradePrefab;
        int upgradePrice;

        Debug.Log("x:" + upgradePathOne);
        Debug.Log("y:" + upgradePathTwo);

        if (upgradePathOne == 0 && upgradePathTwo == 1)
        {
            Debug.Log("01 Path Upgrade");
            upgradePrefab = turretBlueprint.prefabUpgrade01;
            upgradePrice = turretBlueprint.upgradeCost01;
        }
        else if(upgradePathOne == 0 && upgradePathTwo == 2)
        {
            upgradePrefab = turretBlueprint.prefabUpgrade02;
            upgradePrice = turretBlueprint.upgradeCost02;
        }
        else if (upgradePathOne == 0 && upgradePathTwo == 3)
        {
            upgradePrefab = turretBlueprint.prefabUpgrade03;
            upgradePrice = turretBlueprint.upgradeCost03;
        }
        else if (upgradePathOne == 1 && upgradePathTwo == 0)
        {
            upgradePrefab = turretBlueprint.prefabUpgrade10;
            upgradePrice = turretBlueprint.upgradeCost10;
        }
        else if (upgradePathOne == 2 && upgradePathTwo == 0)
        {
            upgradePrefab = turretBlueprint.prefabUpgrade20;
            upgradePrice = turretBlueprint.upgradeCost20;
        }
        else if (upgradePathOne == 3 && upgradePathTwo == 0)
        {
            upgradePrefab = turretBlueprint.prefabUpgrade30;
            upgradePrice = turretBlueprint.upgradeCost30;
        }
        else
        {
            Debug.Log("Error while upgrading turret. Upgrade Path values are incorrect");
            return;
        }

        if (PlayerStats.Money < upgradePrice)
        {
            // Debug.Log("Not enough money for upgrade");
            return;
        }

        
        DeleteTurret();
        PlayerStats.Money -= upgradePrice;
        moneySpentOnTurret += upgradePrice;

        GameObject turretUpgrade = (GameObject)Instantiate(upgradePrefab, GetBuildPosition(), Quaternion.identity);
        //Debug.Log("I made it here");
        turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void SellTurret()
    {
        ResetNode();
        PlayerStats.Money += (int)(moneySpentOnTurret * Shop.sellPercent);
        moneySpentOnTurret = 0;
        
    }

    //deletes the turret then sets its blueprint and upgrade paths path to default values
    public void ResetNode()
    {
        DeleteTurret();
        this.turretBlueprint = null;
        this.upgradePathOne = 0;
        this.upgradePathTwo = 0;
        this.ResetColor();
    }
}
