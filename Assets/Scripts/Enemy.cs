// <copyright file="Enemy.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using DG.Tweening;
using System.Collections;
public class Enemy : MonoBehaviour, IEnemy
{
    public Animator anim;
    [HideInInspector]
    public Bullet bulletWhoShotMe;
    bool dead = false;

    public float speed = 10f;

    public int health = 100;
    public int damageToBaseValue = 1;

    int startingHealth;

    bool readytobepooled;

    public int value = 50;
    public float turretValueBuff = 1.25f;
    public GameObject deathEffect;
    [HideInInspector]
    public float originalSpeed;
    [HideInInspector]
    public bool electricDoT = false;
    [HideInInspector]
    public bool DoTRunning = false;
    [HideInInspector]
    public bool slow = false;
    [HideInInspector]
    public bool permaSlow;
    [HideInInspector]
    public bool alreadySlowed = false;
    [HideInInspector]
    public int DoTDamage;
    [HideInInspector]
    public float DoTTime;
    [HideInInspector]
    public float speedDebuff;
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
        this.originalSpeed = speed;
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
    // Electric damage (since I'm not using a bullet, just subtracting the damage directly)
    public void ElectricDamage(int amount)
    {
        if(dead)
        {
            return;
        }
        this.health -= amount;
        if (electricDoT && !DoTRunning) 
        {
            this.DoTRunning = true;
            StartCoroutine(ElectricDoT());
        }
        if (permaSlow && !alreadySlowed)
        {
            this.originalSpeed = this.originalSpeed * speedDebuff;
            this.speed = this.originalSpeed;
            permaSlow = false;
            alreadySlowed = true;
        }
        if (slow)
        {
            StartCoroutine(Slow());
        }
        if (this.health <= 0 && !dead)
        {
            this.Die();
            this.dead = true;
        }

    }
    private IEnumerator Slow()
    {
        this.speed = originalSpeed * speedDebuff;
        yield return new WaitForSeconds(DoTTime);
        this.speed = originalSpeed;
        yield return null;
    }
    private IEnumerator ElectricDoT()
    {
        float timePassed = 0f;
        while (timePassed < DoTTime) 
        {
            yield return new WaitForSeconds(1f);
            ElectricDamage(DoTDamage);
            if(this.dead == true)
            {
                yield return null;
            }
            timePassed++;
        }
        this.electricDoT = false;
        this.DoTRunning = false;
        yield return null;
    }

    public virtual void Die()
    {
        if(bulletWhoShotMe != null)
        {
            bulletWhoShotMe.turretThatShotMe.killCount++;
        }
        PlayerStats.Money += this.value;
        GameObject[] supportTowers = GameObject.FindGameObjectsWithTag("supportTower");
        foreach (GameObject supportTower in supportTowers)
        {
            float distanceToTower = Vector3.Distance(this.transform.position, supportTower.transform.position);
            SupportTurret supportTurret = supportTower.GetComponent<SupportTurret>();
            if (distanceToTower <= supportTurret.range && supportTurret.leftTier3 == true) 
            {
                PlayerStats.Money -= this.value;
                PlayerStats.Money += (int)(this.value * turretValueBuff);
            }
        }
        GameObject effect = (GameObject)Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        readytobepooled = true;
        gameObject.SetActive(false);
        Destroy(this.gameObject);

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

        PlayerStats.Lives -= damageToBaseValue;
        Destroy(this.gameObject);
    }
}
