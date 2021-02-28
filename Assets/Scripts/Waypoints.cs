// <copyright file="Waypoints.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public Transform[] points;

    private void Awake()
    {
        // Creating an array of all waypoints
        points = new Transform[this.transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = this.transform.GetChild(i);
        }
    }
}
