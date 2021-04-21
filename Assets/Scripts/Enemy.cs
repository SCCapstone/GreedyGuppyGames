// <copyright file="Enemy.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using DG.Tweening;
using System.Collections;
public class Enemy : MonoBehaviour, IEnemy
{
    // Setup fields and globals
    public Animator anim;
    public Waypoints waypoints;
    [HideInInspector]
    public Bullet bulletWhoShotMe;
    bool dead = false;

    public float speed = 10f;

    public int health = 100;
    public int damageToBaseValue = 1;

    int startingHealth;

    bool readytobepooled;

    public string deathSound;
    public string baseDamage;

    public int value = 50;
    public float turretValueBuff = 1.25f;
    [HideInInspector]
    public Turret turretThatShotMe;
    public Turret electricTowerAffectingMe;
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
    private Transform target, targetOne, targetTwo, targetThree;
    protected int wavepointIndex, indexOne, indexTwo, indexThree = 0;
    public float distanceLeft = 0f;

    

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

    // Runs when enemy spawns
    public virtual void Start()
    {
		//KEEP REGARDLESS OF MERGE
        this.originalSpeed = speed;
        //this.target = Waypoints.points[wavepointIndex];
        // Sets the appropriate array of waypoints index zero to the apporpriate target
        this.target = waypoints.points[wavepointIndex];
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
        //Debug.Log(this + " " + distanceLeft);
        //anim.Play("Walk Forward Slow WO Root", -1, 0);

        getTotalDistance();
    }

    // Reduces the enemy's health pool
    public void TakeDamage(int amount)
    {
        if(dead)
        {
            return;
        }
        this.health -= amount;
        if(bulletWhoShotMe != null)
        {
            bulletWhoShotMe.ReducePierce();
        }
        if (this.health <= 0 && !dead)
        {
            if(bulletWhoShotMe != null)
            {
                this.turretThatShotMe=bulletWhoShotMe.turretThatShotMe;
            }
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
            this.turretThatShotMe = this.electricTowerAffectingMe;
            this.Die();
            this.dead = true;
        }

    }

    // Rudeces enemy movement speed
    private IEnumerator Slow()
    {
        this.speed = originalSpeed * speedDebuff;
        yield return new WaitForSeconds(DoTTime);
        this.speed = originalSpeed;
        yield return null;
    }

    // Applies a damage over time effect that slowly hurts the enemy
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

    // Removes the enemy from the scene and gives the player money
    public virtual void Die()
    {
        if(turretThatShotMe != null)
        {
            this.turretThatShotMe.killCount++;
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
        FindObjectOfType<AudioManager>().PlayAudio(deathSound);
        
        Destroy(this.gameObject);

    }

    // Runs when enemy spawns, sets health
    void Awake()
    {
        startingHealth = health;
    }

    // Pools multiple enemies to be spawned
    public virtual void Onpooled()
    {
        if (!readytobepooled)
            return;
        dead = false;
        health = startingHealth;
        SetWavepointIndex(0);
        this.target = waypoints.points[wavepointIndex];
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);


    }

    // Runs every frame, handles where the enemy needs to go/look
    private void Update()
    {
        // Direction pointing to waypoint
        Vector3 dir = this.target.position - this.transform.position;
        this.transform.Translate(dir.normalized * this.speed * Time.deltaTime, Space.World);
        //Debug.Log(Vector3.Magnitude(dir.normalized * this.speed * Time.deltaTime));
        distanceLeft -= Vector3.Magnitude(dir.normalized * this.speed * Time.deltaTime);
        //getTotalDistance();
        //Debug.Log(this + " " + distanceLeft);


        // Checks if we are verrrrry close to a waypoint
        if (Vector3.Distance(this.transform.position, this.target.position) <= 0.4f)
        {
            this.GetNextWaypoint();
        }
    }

    private void GetNextWaypoint()
    {
        getTotalDistance();
        // Enemy has reached the end
        if (this.wavepointIndex >= waypoints.points.Length - 1)
        {
            this.EndPath();
            return; // makes sure the code doesn't skip into next node segment (yes this happens)
        }

        // Not at the end, find next waypoint
        this.wavepointIndex++;
        this.target = waypoints.points[this.wavepointIndex];
        // Look at waypoint, rotation stuff
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
    }

    public void SetWaypoint(int index)
    {
        getTotalDistance();
        // Enemy has reached the end
        if (this.wavepointIndex >= waypoints.points.Length - 1)
        {
            this.EndPath();
            return; // makes sure the code doesn't skip into next node segment (yes this happens)
        }

        // Not at the end, find next waypoint
        this.wavepointIndex = index;
        this.target = waypoints.points[this.wavepointIndex];
        // Look at waypoint, rotation stuff
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
    }

    // Runs when enemy hits the player's base, reduces player lives
    public virtual void EndPath()
    {

        PlayerStats.Lives -= damageToBaseValue;
        FindObjectOfType<AudioManager>().PlayAudio(baseDamage);
        Destroy(this.gameObject);
    }

    // Determines how far an enemy is from the player base, used for turret targeting
    public void getTotalDistance()
    {
        this.distanceLeft = Vector3.Distance(this.transform.position,waypoints.points[wavepointIndex].position);
        //Debug.Log(this.transform.position);
        //Debug.Log(waypoints.points[wavepointIndex].position);
        //Debug.Log("I did this" + distanceLeft);

        //Debug.Log("wpi: " + wavepointIndex + " length: " + waypoints.points.Length);
        for (int i = wavepointIndex; i < waypoints.points.Length; ++i)
        {
            if(i +1 < waypoints.points.Length)
            {
                //Debug.Log("distance left " + distanceLeft);
                this.distanceLeft += Vector3.Distance(waypoints.points[i].position,waypoints.points[i+1].position);
            }
        }
        //Debug.Log("starting distance left " + distanceLeft);
    }
}