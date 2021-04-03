// <copyright file="Waypoints.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

// An array that holds all the waypoints that enemies use to navigate
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