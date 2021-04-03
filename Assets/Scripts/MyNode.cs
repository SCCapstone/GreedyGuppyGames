// <copyright file="MyNode.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.EventSystems;

// The logic that is house within every node. If an action involves a node, it comes through here.
public class MyNode : MonoBehaviour
{
    // Setup fields and globals
    public Color hoverColor;
    public Color nodeSelectedToUpgradeColor;
    public Color errorColor;
    public bool leftNode = true;
    
    public Vector3 positionOffset;
    public float supportTowerReduction;
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
    public GameObject rangeObjectPrefab;
    private GameObject rangeObject;


    private Renderer rend;
    private Color startColor;
    private BuildManager buildManager;

    // Runs before frame 1, setup each node
    private void Start()
    {
        this.rend = this.GetComponent<Renderer>();
        this.startColor = this.rend.material.color;
        this.buildManager = BuildManager.instance;
    }

    // Decides where on the node to build stuff
    public Vector3 GetBuildPosition()
    {
        return this.transform.position + this.positionOffset;
    }

    // What happens when the player clicks on a node
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

    // What happens when a mouse enters a node (just goes over it, not click)
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

    // Changes the color of the node when you click one with a turret on it
    public void SelectForUpgradeColor()
    {
        this.rend.material.color = this.nodeSelectedToUpgradeColor;
    }

    // Returns the color of the node to the default
    public void ResetColor()
    {
        this.rend.material.color = this.startColor;
    }

    // What happens when a mouse leaves node, see OnMouseEnter for more info
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

    // Just here for deselecting towers by clicking on them
    private void OnMouseOver()
    {
        // changes the color back if right clicked to deselect tower on the node
        if (Input.GetMouseButtonDown(1))
        {
            this.ResetColor();
        }
    }

    // Builds a turret on a node
    private void BuildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            // Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= blueprint.cost;
        moneySpentOnTurret += blueprint.cost;
        // refund money back to the player if a lefTier2 tower is in range
        GameObject[] supportTowers = GameObject.FindGameObjectsWithTag("supportTower");
        foreach (GameObject supportTower in supportTowers)
        {
            float distanceToTower = Vector3.Distance(this.transform.position, supportTower.transform.position);
            SupportTurret supportTurret = supportTower.GetComponent<SupportTurret>();
            if (distanceToTower <= supportTurret.range && supportTurret.leftTier2 == true) 
            {
                PlayerStats.Money += (int)(blueprint.cost * supportTowerReduction);
            }
        }
        // Build a turret
        // GameObject turretToBuild = buildManager.GetTurretToBuild();
        GameObject aTurret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = aTurret;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        turretBlueprint = blueprint;
    }

    // Upgrades the turret on this node
    public void UpgradeTurret()
    {
        GameObject upgradePrefab;
        int upgradePrice;

        // Controls what upgraded tower gets built based off of what the player chose
        if (upgradePathOne == 0 && upgradePathTwo == 1)
        {
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

        // saves the kill count of the turret
        int tempKillCount = 0;
        if (this.turret.gameObject.CompareTag("Tower")) 
        {
            tempKillCount = this.turret.gameObject.GetComponent<Turret>().killCount;
        }
        DeleteTurret();
        PlayerStats.Money -= upgradePrice;
        moneySpentOnTurret += upgradePrice;

        GameObject turretUpgrade = (GameObject)Instantiate(upgradePrefab, GetBuildPosition(), Quaternion.identity);
        //Debug.Log("I made it here");
        turret = turretUpgrade;
        // resets the kill count of the turret
        if (this.turret.gameObject.CompareTag("Tower")) 
        {
            this.turret.gameObject.GetComponent<Turret>().killCount = tempKillCount;
        }
        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
        ShowRangeIndicator();
    }

    // Removes the turret and gives the player some money
    public void SellTurret()
    {
        
        ResetNode();
        PlayerStats.Money += (int)(moneySpentOnTurret * Shop.sellPercent);
        moneySpentOnTurret = 0;        
    }

    //deletes the turret then sets its blueprint and upgrade paths path to default values
    public void ResetNode()
    {
        if (this.turret.gameObject.CompareTag("supportTower")) 
        {
            SupportTurret tower = turret.GetComponent<SupportTurret>();
            tower.Cleanup();

        }
        DeleteTurret();
        this.turretBlueprint = null;
        this.upgradePathOne = 0;
        this.upgradePathTwo = 0;
        this.ResetColor();
        HideRangeIndicator();
    }

    // Renders a ring around the node to show the range of the tower that is on it
    public void ShowRangeIndicator()
    {
        if(turret == null)
        {
            return;
        }
        Vector3 pos = new Vector3(0f,4f,0f);
        pos.x = this.turret.transform.position.x;
        pos.z = this.turret.transform.position.z;
        float scaleSize = this.turret.GetComponent<Turret>().range * 2;
        if(rangeObject == null)
        {
            rangeObject = GameObject.Instantiate(rangeObjectPrefab, pos, Quaternion.identity);
        }
        rangeObject.gameObject.transform.localScale = new Vector3(scaleSize,2f,scaleSize);
    }

    // Hides the above range indicator
    public void HideRangeIndicator()
    {
        Destroy(rangeObject);
    }
}
