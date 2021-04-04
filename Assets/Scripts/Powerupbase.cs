// <copyright file="Powerupbase.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerupbase : MonoBehaviour
{
    public PowerupManager.PowerUpType type;
    public GameObject explosionEffect;
    [SerializeField] protected float lifespan = 5;
    public virtual void Activate()
    {
        Debug.Log("activated");
        Invoke("Deactivate", lifespan);
    }
    public virtual void Deactivate()
    {
        CancelInvoke();
        Destroy(gameObject);
    }
}
