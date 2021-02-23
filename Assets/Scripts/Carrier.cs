﻿using System.Collections;
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

    void spawn ()
    {
        Enemy enemy = EnemyPooler.self.getCarrierEnemy(spawnpoint, EnemyPooler.self.transform, false);
        enemy.Onpooled();
        enemy.SetWavepointIndex(wavepointIndex);
        enemy.gameObject.SetActive(true);
    }

    public override void Die()
    {
        base.Die();
        CancelInvoke();
    }

    void recordpose()
    {
        spawnpoint = transform.position;
        
    }
    
}