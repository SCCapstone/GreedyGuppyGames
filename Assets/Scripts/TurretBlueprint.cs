// <copyright file="TurretBlueprint.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

[System.Serializable]
public class TurretBlueprint
{
    public GameObject prefab;
    public GameObject prefabUpgrade01;
    public GameObject prefabUpgrade02;
    public GameObject prefabUpgrade03;
    public GameObject prefabUpgrade10;
    public GameObject prefabUpgrade20;
    public GameObject prefabUpgrade30;

    public int cost;
    public int upgradeCost01;
    public int upgradeCost02;
    public int upgradeCost03;
    public int upgradeCost10;
    public int upgradeCost20;
    public int upgradeCost30;
}