// <copyright file="Turret.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;

    [Header("Attributes")]

    public float range = 15f;

    public float firerate = 1f;
    private float fireCountdown = 0f;

    //Audio file name to be played when turret is firing a bullet
    public string gunShotAudio;

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    private void Start()
    {
        this.InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
        float shortestDistanceToEnd = Mathf.Infinity;
        //float shortestDistanceToTurret = Mathf.Infinity;
        GameObject nearestEnemy = null;
        int finalWaypointIndex = Waypoints.points.Length - 1;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position); // turret to enemy distance
            float enemyDistanceToEnd = Vector3.Distance(enemy.transform.position, Waypoints.points[finalWaypointIndex].position); // enemy to end distance
            // Debug.Log("Distance "+enemyDistanceToEnd);

            /* Old targeting, targest closest enemy to tower
            if (distanceToEnemy < shortestDistanceToTurret)
            {
                shortestDistanceToTurret = distanceToEnemy;
                nearestEnemy = enemy;
            }
            */

            // Targets enemy closest to last waypoint
            if (enemyDistanceToEnd < shortestDistanceToEnd && distanceToEnemy <= this.range)
            {
                //shortestDistanceToTurret = distanceToEnemy;
                shortestDistanceToEnd = enemyDistanceToEnd;
                nearestEnemy = enemy;
                // Debug.Log("Shortest Distance " + shortestDistance);
                // Debug.Log("nearest enemy "+nearestEnemy);
            }
        }

        // Debug.Log("nearest enemy " + nearestEnemy);
        // Debug.Log("Shortest Distance to end" + shortestDistanceToEnd);
        // Debug.Log("Range " + this.range);

        // Sets the target to the chosen enemy
        if (nearestEnemy != null)
        {
            this.target = nearestEnemy.transform;
            // Debug.Log(this.target);
        }
        else
        {
            this.target = null;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this.fireCountdown -= Time.deltaTime;
        if (this.target == null)
        {
            return;
        }

        // Target lockon
        Vector3 dir = this.target.position - this.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(this.partToRotate.rotation, lookRotation, Time.deltaTime * this.turnSpeed).eulerAngles;
        this.partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (this.fireCountdown <= 0f)
        {
            this.Shoot();
            this.fireCountdown = 1f / this.firerate;
        }

        
    }

    private void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(this.bulletPrefab, this.firePoint.position, this.firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(this.target);
        }

        //Audio for when a "bullet" is fired
        FindObjectOfType<AudioManager>().PlayAudio(gunShotAudio);
        // Debug.Log("Audio should have been played");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.range);
    }
}
