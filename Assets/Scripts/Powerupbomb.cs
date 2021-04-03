// <copyright file="Powerupbomb.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerupbomb : Powerupbase
{
    public override void Activate ()
    {
        base.Activate();
        Invoke("explode", lifespan * .25f);
    }
    void explode()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 10);
        foreach (Collider collider in colliders)
        {
            // if it hits an enemy it damages it and reduces the amount of things it can still hit
            if (collider.tag == "Enemy")
            {
                collider.GetComponent<Enemy>().TakeDamage(75);
            }
        }
        Deactivate();
    }
}
