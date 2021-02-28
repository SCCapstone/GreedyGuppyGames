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
        //points = new Transform[this.transform.childCount];
        try
        {
            waypointsTransform = GameObject.Find("Waypoints").transform;
        }
        catch
        {
            Debug.Log("No 'Waypoints' game object, searching for 'Path' game objects");
            try
            {
                pathOneTrans = GameObject.Find("PathOne").transform;
                pathTwoTrans = GameObject.Find("PathTwo").transform;
                pathThreeTrans = GameObject.Find("PathThree").transform;
            }
            catch
            {
                Debug.LogError("No 'Paths' game objects found");
            }
        }
        if(waypointsTransform)
        {
            points = new Transform[waypointsTransform.childCount];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = waypointsTransform.GetChild(i);
            }
        }
        else
        {
            // Arrays holding the waypoints for their respecive paths
            pointsOne = new Transform[pathOneTrans.childCount];
            pointsTwo = new Transform[pathTwoTrans.childCount];
            pointsThree = new Transform[pathThreeTrans.childCount];

            // First array
            for (int j = 0; j < pathOneTrans.childCount; j++)
            {
                pointsOne[j] = pathOneTrans.GetChild(j);
            }
            // Second array
            for (int j = 0; j < pathTwoTrans.childCount; j++)
            {
                pointsTwo[j] = pathTwoTrans.GetChild(j);
            }
            // Third array
            for (int j = 0; j < pathThreeTrans.childCount; j++)
            {
                pointsThree[j] = pathThreeTrans.GetChild(j);
            }
        }
        
    }
}
