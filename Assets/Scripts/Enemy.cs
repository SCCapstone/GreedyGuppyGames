// <copyright file="Enemy.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using DG.Tweening;
public class Enemy : MonoBehaviour, IEnemy
{
    public Animator anim;
    [HideInInspector]
    public Bullet bulletWhoShotMe;
    bool dead = false;

    public float speed = 10f;

    public int health = 100;

    int startingHealth;

    bool readytobepooled;

    public int value = 50;

    public GameObject deathEffect;

    private Transform target;
    protected int wavepointIndex = 0;

    // Getters
    public float GetSpeed()
    {
        return this.speed;
    }

    public int GetHealth()
    {
        return this.health;
    }

    public int GetValue()
    {
        return this.value;
    }

    // Setters
    public void SetSpeed(float _speed)
    {
        this.speed = _speed;
    }

    public void SetHealth(int _health)
    {
        this.health = _health;
    }

    public void SetValue(int _value)
    {
        this.value = _value;
    }

    public void SetWavepointIndex(int _index)
    {
        this.wavepointIndex = _index;
    }

    public virtual void Start()
    {
        this.target = Waypoints.points[wavepointIndex];
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
        //anim.Play("Walk Forward Slow WO Root", -1, 0);
    }

    public void TakeDamage(int amount)
    {
        if(dead)
        {
            return;
        }
        this.health -= amount;
        bulletWhoShotMe.ReducePierce();
        if (this.health <= 0 && !dead)
        {
            this.Die();
            this.dead = true;
        }
    }

    public virtual void Die()
    {
        bulletWhoShotMe.turretThatShotMe.killCount++;
        PlayerStats.Money += this.value;
        GameObject effect = (GameObject)Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        readytobepooled = true;
        gameObject.SetActive(false);
        //Destroy(this.gameObject);

    }

    void Awake()
    {
        startingHealth = health;
    }

    public virtual void Onpooled()
    {
        if (!readytobepooled)
            return;
        dead = false;
        health = startingHealth;
        SetWavepointIndex(0);
        this.target = Waypoints.points[wavepointIndex];
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);


    }

    private void Update()
    {
        // Direction pointing to waypoint
        Vector3 dir = this.target.position - this.transform.position;
        this.transform.Translate(dir.normalized * this.speed * Time.deltaTime, Space.World);

        // Checks if we are verrrrry close to a waypoint
        if (Vector3.Distance(this.transform.position, this.target.position) <= 0.4f)
        {
            this.GetNextWaypoint();
        }
    }

    private void GetNextWaypoint()
    {
        // Enemy has reached the end
        if (this.wavepointIndex >= Waypoints.points.Length - 1)
        {
            this.EndPath();
            return; // makes sure the code doesn't skip into next node segment (yes this happens)
        }

        // Not at the end, find next waypoint
        this.wavepointIndex++;
        this.target = Waypoints.points[this.wavepointIndex];
        // Look at waypoint, rotation stuff
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
    }

    public void SetWaypoint(int index)
    {
        // Enemy has reached the end
        if (this.wavepointIndex >= Waypoints.points.Length - 1)
        {
            this.EndPath();
            return; // makes sure the code doesn't skip into next node segment (yes this happens)
        }

        // Not at the end, find next waypoint
        this.wavepointIndex = index;
        this.target = Waypoints.points[this.wavepointIndex];
        // Look at waypoint, rotation stuff
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
    }

    public virtual void EndPath()
    {

        PlayerStats.Lives -= 25;
        Destroy(this.gameObject);
    }
}
