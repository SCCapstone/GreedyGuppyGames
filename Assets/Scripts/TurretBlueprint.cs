// <copyright file="TurretBlueprint.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
// A central location for settings up and housing the info about a tower
public class TurretBlueprint
{
    [Header("Turret Prefabs")]
    public GameObject prefab;
    public GameObject prefabUpgrade01;
    public GameObject prefabUpgrade02;
    public GameObject prefabUpgrade03;
    public GameObject prefabUpgrade10;
    public GameObject prefabUpgrade20;
    public GameObject prefabUpgrade30;


    [Header("Turret Costs")]
    public int cost;
    public int upgradeCost01;
    public int upgradeCost02;
    public int upgradeCost03;
    public int upgradeCost10;
    public int upgradeCost20;
    public int upgradeCost30;

    [Header("Turret Descriptions")]
    public string upgrade10Text;
    public string upgrade20Text;
    public string upgrade30Text;
    public string upgrade01Text;
    public string upgrade02Text;
    public string upgrade03Text;

    [Header("Turret Sprite")]
    public Sprite turretImage;
}