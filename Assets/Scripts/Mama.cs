// <copyright file="Mama.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mama : Enemy
{
    public Enemy bugToSpawn;
    int numberToSpawn = 1;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
    public override void Die()
    {
        SpawnGrub();
        base.Die();
        

    }
    public void SpawnGrub()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Enemy newEnemy = Instantiate(bugToSpawn).GetComponent<Enemy>();
            newEnemy.transform.forward = transform.forward;
            newEnemy.waypoints = this.waypoints;
            newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (i * .25f));
            Debug.Log("Wavepoint index is " + wavepointIndex);
            newEnemy.SetWaypoint(wavepointIndex);

        }
    }

    public override void EndPath()
    {
        PlayerStats.Lives -= 30;
        Destroy(this.gameObject);
    }


}
