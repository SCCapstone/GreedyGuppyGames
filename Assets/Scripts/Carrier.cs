// <copyright file="Carrier.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Enemy
{    
    [SerializeField] int spawnRate = 10;
    Vector3 spawnpoint;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        InvokeRepeating("recordpose", .1f, spawnRate);
        InvokeRepeating("spawn", 1, spawnRate);
    }

    // Spawns enemies continuously out of the carrier
    void spawn ()
    {
        Enemy enemy = EnemyPooler.self.getCarrierEnemy(spawnpoint, EnemyPooler.self.transform, false);
        enemy.Onpooled();
        enemy.SetWavepointIndex(wavepointIndex);
        enemy.waypoints = this.waypoints;
        enemy.gameObject.SetActive(true);
    }

    // Removes the carrier
    public override void Die()
    {
        base.Die();
        CancelInvoke();
    }

    // Sets where the children spawn from
    void recordpose()
    {
        spawnpoint = transform.position;
    }
    
}
