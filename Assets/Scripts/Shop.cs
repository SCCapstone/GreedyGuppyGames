// <copyright file="Shop.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint BasicTower;
    public TurretBlueprint CannonTower;
    public TurretBlueprint MachineTower;
    private BuildManager buildManager;

    private void Start()
    {
        this.buildManager = BuildManager.instance;
    }

    public void SelectBasicTurret()
    {
        Debug.Log("Standard Turret Selected");
        this.buildManager.SelectTurretToBuild(this.BasicTower);
    }

    public void SelectCannonTurret()
    {
        Debug.Log("Cannon Turret Selected");
        this.buildManager.SelectTurretToBuild(this.CannonTower);
    }

    public void SelectMachineTurret()
    {
        Debug.Log("Machine Turret Selected");
        this.buildManager.SelectTurretToBuild(this.MachineTower);
    }
}
