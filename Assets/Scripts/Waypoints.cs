// <copyright file="Waypoints.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Transform[] points;
    public GameObject startingPoint;

    private void Awake()
    {
        // Creating an array of all waypoints
        points = new Transform[this.transform.childCount];
        int pointsLength = points.GetLength(0);
        Debug.Log(gameObject.name+": "+pointsLength+" waypoints");
        points[0] = startingPoint.transform;
        if(pointsLength > 1)
        {
            for (int i = 1; i < points.Length; i++)
            {
                points[i] = this.transform.GetChild(i);
            }
        }
    }
}
