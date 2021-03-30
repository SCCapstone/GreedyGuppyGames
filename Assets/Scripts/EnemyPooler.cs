// <copyright file="EnemyPooler.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler self;
    [SerializeField] ObjectPooler carrierPooler;
    void Awake()
    {
        self = this;
    }

    public Enemy getCarrierEnemy(Vector3 pos, Transform parent, bool enableOnPooled)
    {
        return carrierPooler.GetObj(pos, parent, enableOnPooled).GetComponent<Enemy>();
    }
}
