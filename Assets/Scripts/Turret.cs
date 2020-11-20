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

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    private void Start()
    {
        this.InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
<<<<<<< HEAD
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
=======
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position); // turret to enemy distance
            float enemyDistanceToEnd = Vector3.Distance(enemy.transform.position, Waypoints.points[finalWaypointIndex].position); // enemy to end distance
            //Debug.Log("Distance "+enemyDistanceToEnd);
            /* Old targeting, targest closest enemy to tower
            if (distanceToEnemy < shortestDistanceToTurret)
            {
                shortestDistanceToTurret = distanceToEnemy;
                nearestEnemy = enemy;
            } */

            // Targets enemy closest to last waypoint
            if (enemyDistanceToEnd < shortestDistanceToEnd || shortestDistanceToTurret >= this.range)
>>>>>>> parent of f130251... Minor comment updates/formatting
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
<<<<<<< HEAD
            }
        }

        if (nearestEnemy != null && shortestDistance <= this.range)
        {
            this.target = nearestEnemy.transform;
=======
                //Debug.Log("Shortest Distance " + shortestDistance);
                //Debug.Log("nearest enemy "+nearestEnemy);
            }
        }

        //Debug.Log("nearest enemy " + nearestEnemy);
        //Debug.Log("Shortest Distance to end" + shortestDistanceToEnd);
        //Debug.Log("Range " + this.range);
        if (nearestEnemy != null && shortestDistanceToTurret <= this.range)
        {
            this.target = nearestEnemy.transform;
            Debug.Log(this.target);
>>>>>>> parent of f130251... Minor comment updates/formatting
        }
        else
        {
            this.target = null;
        }
    }

    // Update is called once per frame
    private void Update()
    {
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

        this.fireCountdown -= Time.deltaTime;
    }

    private void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(this.bulletPrefab, this.firePoint.position, this.firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(this.target);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.range);
    }
}
