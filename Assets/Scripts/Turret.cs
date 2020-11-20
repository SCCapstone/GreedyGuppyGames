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

    //Audio file to be played when turret is firing a bullet
    public AudioSource GunShot;

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
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= this.range)
        {
            this.target = nearestEnemy.transform;
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

        //Audio for when a "bullet" is fired
        GunShot.Play();
        Debug.Log("Audio should have been played");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.range);
    }
}
