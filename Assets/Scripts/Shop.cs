// <copyright file="Shop.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint standardTurret;
    public TurretBlueprint anotherTurret;
    private BuildManager buildManager;

    private void Start()
    {
        this.buildManager = BuildManager.instance;
    }

    public void SelectBasicTurret()
    {
        Debug.Log("Standard Turret Selected");
        this.buildManager.SelectTurretToBuild(this.standardTurret);
    }

    public void SelectCannonTurret()
    {
        Debug.Log("Another Turret Selected");
        this.buildManager.SelectTurretToBuild(this.anotherTurret);
    }
}
