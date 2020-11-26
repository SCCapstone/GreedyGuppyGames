// <copyright file="BuildManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;



    public GameObject buildEffect;

    private TurretBlueprint turretToBuild;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BuildManager in scene!");
            return;
        }

        instance = this;
    }

    public bool CanBuild
    {
        get { return this.turretToBuild != null; }
    }

    public bool HasMoney
    {
        get { return PlayerStats.Money >= this.turretToBuild.cost; }
    }

    public void BuildTurretOn(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= this.turretToBuild.cost;

        // Build a turret
        // GameObject turretToBuild = buildManager.GetTurretToBuild();
        GameObject turret = (GameObject)Instantiate(this.turretToBuild.prefab, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turret;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret build! Money left: " + PlayerStats.Money);
    }

    public void UpgradeTurret01(MyNode node)
    {
        if(PlayerStats.Money < this.turretToBuild.upgradeCost01)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost01;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade01, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void UpgradeTurret02(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.upgradeCost01)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost02;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade02, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void UpgradeTurret03(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.upgradeCost03)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost03;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade03, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void UpgradeTurret10(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.upgradeCost10)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost10;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade10, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void UpgradeTurret20(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.upgradeCost20)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost20;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade20, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }

    public void UpgradeTurret30(MyNode node)
    {
        if (PlayerStats.Money < this.turretToBuild.upgradeCost30)
        {
            Debug.Log("Not enough money for upgrade");
            return;
        }

        node.DeleteTurret();

        PlayerStats.Money -= this.turretToBuild.upgradeCost30;

        GameObject turretUpgrade = (GameObject)Instantiate(this.turretToBuild.prefabUpgrade30, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turretUpgrade;

        GameObject effect = (GameObject)Instantiate(this.buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
    }


    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        this.turretToBuild = turret;
    }

    public void ResetTurretToBuild()
    {
        Debug.Log("Turret Unselected");
        this.SelectTurretToBuild(null);
    }
}
