// <copyright file="SupportTurret.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTurret : MonoBehaviour
{ 
    public float range = 15f;
    public float fireRateMultiplier = 2f;
    [HideInInspector]
    public string towerTag = "Tower";
    public bool leftTier1 = false;
    public bool leftTier2 = false;
    public bool leftTier3 = false;
    public bool rightTier1 = false;
    public bool rightTier2 = false;
    public bool rightTier3 = false;
    // Start is called before the first frame update
    void Start()
    {
        this.InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    void UpdateTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(this.towerTag);
        
        foreach (GameObject tower in towers)
        {
            // find all towers within range
            float distanceToTower = Vector3.Distance(this.transform.position, tower.transform.position);
                if (distanceToTower <= this.range) 
                {
                    Turret turret = tower.GetComponent<Turret>();
                    if (leftTier1 == true) 
                    {
                        this.range = this.range * 1.25f;
                    }
                    //Cheaper towers handled in MyNode
                    //Enemies giving more money is handled in the enemies script
                    if (rightTier1 == true) 
                    {
                        turret.range = turret.originalRange * 1.25f;
                    }
                    if (rightTier2 == true) 
                    {
                        turret.firerate = turret.originalFireRate * 1.25f;
                    }
                    if (rightTier3 == true) 
                    {

                    }
                }
        }

    }
    // Runs when called from MyNode.cs in BuildTurret()
    private void buffLeftTier2()
    {
        
    }
    //This is to be used when the tower is sold, it will clean out all the buffs it is currently applying
    private void Cleanup()
    {

    }
}
