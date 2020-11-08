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

    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        this.turretToBuild = turret;
    }
}
