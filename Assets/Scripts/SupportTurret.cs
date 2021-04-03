// <copyright file="SupportTurret.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Special logic just for the support turret, see Turret.cs for further details
public class SupportTurret : Turret
{ 
    public float fireRateMultiplier = 1.25f;
    public float rangeMultiplier = 1.25f;
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

    // Updates what tower this is buffing
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
                //Cheaper towers handled in MyNode
                //Enemies giving more money is handled in the enemies script
                if (rightTier1 == true && turret.buffedRange == false) 
                {
                    turret.range = turret.originalRange * rangeMultiplier;
                    turret.buffedRange = true;
                }
                if (rightTier2 == true && turret.buffedFireRate == false) 
                {
                    turret.firerate = turret.originalFireRate * fireRateMultiplier;
                    turret.buffedFireRate = true;
                }

            }
        }

    }
    //This is to be used when the tower is sold, it will clean out all the buffs it is currently applying
    public void Cleanup()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(this.towerTag);
        
        foreach (GameObject tower in towers)
        {
            // find all towers within range
            float distanceToTower = Vector3.Distance(this.transform.position, tower.transform.position);
            if (distanceToTower <= this.range) 
            {
                Turret turret = tower.GetComponent<Turret>();
                if (turret.buffedRange == true)                     {
                    turret.range = turret.originalRange;
                    turret.buffedRange = false;
                }
                if (turret.buffedFireRate == false) 
                {
                    turret.firerate = turret.originalFireRate;
                    turret.buffedFireRate = false;
                }
            }
        }
    }
}
