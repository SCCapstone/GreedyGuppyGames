// <copyright file="Bullet.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Bullet : MonoBehaviour, IBullet
{
    private Transform target;
    [HideInInspector]
    public Turret turretThatShotMe;
    public float speed = 70f;
    public int damage = 50;
    public float explosionRadius = 0f;
    public int pierce = 1;
    public int explosionPierce = 10;
    public float lifeSpan = 10f;
    public GameObject impactEffect;
    public GameObject shrapnelGameObject;
    public bool makeShrapnel = false;
    public bool tracking = false;
    public float sprayAmount = 0f;
    
    private Quaternion rotate;
    private Vector3 directionOfTravel;


    // Update is called once per frame
    private void Start()
    {
        rotate = this.transform.rotation;
    }
    private void Update()
    {
        this.CheckOutOfBounds();
        this.CheckLifeSpan();
        if(tracking)
        {
            Track();
        }
        Vector3 dir = this.directionOfTravel;
        float distanceThisFrame = this.speed * Time.deltaTime;

        dir = checkY(dir);
        //dir = addSpray(dir);

        this.transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        this.transform.LookAt(this.directionOfTravel);
        this.transform.rotation = rotate;
    }
        public void Track()
    {
        if(this.target == null)
        {
            tracking = false;
            return;
        }
        this.SetBulletDirection();
    }


    public Vector3 addSpray(Vector3 dir)
    {
        dir = dir.normalized;
        float randDegree = Random.Range(-sprayAmount,sprayAmount);
        //Debug.Log(randDegree);
        float radians = randDegree * Mathf.Deg2Rad;
        float x = dir.x * Mathf.Cos(radians) - dir.z * Mathf.Sin(radians);
        float z = dir.x * Mathf.Sin(radians) + dir.z * Mathf.Cos(radians);

        dir.x = x;
        dir.z = z;
        return dir;
    }
    public void Seek(Transform aTarget)
    {
        this.target = aTarget;
    }


    // if y of a bullet is too high or low it causes the bullet to not change y direction anymore
    private Vector3 checkY(Vector3 dir)
    {
        if (this.transform.position.y <= 4)
        {
            dir.y = 0;
        }
        else if (this.transform.position.y >= 7)
        {
            dir.y = 0;
        }
        return dir;
    }

    // hits the target or explodes 
    private void HitTarget()
    {
        if (this.explosionRadius > 0f)
        {
            this.Explode();
        }
        else
        {
            this.Damage(this.target);
        }
    }

    // makes an explosion
    private void Explode()
    {
        //initilizes how many enemies can be hit in the explosion
        int explosionPierceLeft = this.explosionPierce;

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius);
        foreach (Collider collider in colliders)
        {
            // dont loop anymore if cannot hit anymore enemies
            if(explosionPierceLeft <= 0)
            {
                break;
            }
            // if it hits an enemy it damages it and reduces the amount of things it can still hit
            if (collider.tag == "Enemy")
            {
                explosionPierceLeft--;
                this.Damage(collider.transform);
            }
        }
        if (makeShrapnel)
        {
            MakeShrapnel();
        }
    }

    //damages an enemy
    private void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.bulletWhoShotMe = this;
            e.TakeDamage(this.damage);
        }
    }

    //checks if bullet is outside of the map and then despawns it if it is
    private void CheckOutOfBounds()
    {
        if(this.transform.position.x >200 || this.transform.position.x < -200 || this.transform.position.z >200 || this.transform.position.z <-200 || this.transform.position.y > 100 || this.transform.position.y < -100)
        {
            this.DespawnThisBullet();
        }
    }

    //checks if bullet has run out of time
    private void CheckLifeSpan() 
    {
        lifeSpan -= Time.deltaTime;
        if (lifeSpan < 0) {
            DespawnThisBullet();
        }
    }

    // deletes the bullet with no effects
    private void DespawnThisBullet()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
    }

    // What happens when the bullet hits something (yes, this mostly replaces HitTarget)
    void OnCollisionEnter(Collision col)
    {
        /*
        if (col.gameObject.tag == "ammo")
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), col.collider);
        }
        */
        if (this.explosionRadius > 0f)
        {
            this.Explode();
        }
        else
        {
            this.Damage(col.gameObject.GetComponent<Transform>()); // gets the transform of the gameObject of what was hit and hurts it (the spaghetti is ready)
        }
    }

    //sets all the stats for the bullet based on the tower
    public void SetBulletStats(float speed, int damage, float explosionRadius, int pierce, Turret turretThatShotMe, int explosionPierce, bool makeShrapnel, bool tracking, float sprayAmount)
    {
        this.speed = speed;
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.pierce = pierce;
        this.turretThatShotMe = turretThatShotMe;
        this.explosionPierce = explosionPierce;
        this.makeShrapnel = makeShrapnel;
        this.tracking = tracking;
        this.sprayAmount = sprayAmount;
    }

    //destroys the bullet when it would die normally 
    public void DestroyThisBullet()
    {
        Destroy(this.gameObject);
        GameObject effectIns = (GameObject)Instantiate(this.impactEffect, this.transform.position, this.transform.rotation);
        Destroy(effectIns, 2f);
    }

    // reduces the pierce by 1
    public void ReducePierce()
    {
        this.pierce--;
        if(pierce <= 0)
        {
            DestroyThisBullet();
        }
    }

    // sets direction the bullet goes
    public void SetBulletDirection()
    {
        this.directionOfTravel = addSpray(this.target.position - this.transform.position);
    }

    // makes shrapnel to be fired from an explosion
    public void MakeShrapnel()
    {
        // direction the shrapnel travels too
        Vector3 travelDirection = new Vector3(this.transform.position.x + 500, this.transform.position.y, this.transform.position.z);
        // where it spawns
        Vector3 spawnLocation = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
        //make bullet and set it to shit
        GameObject bulletGO1 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet1 = bulletGO1.GetComponent<Bullet>();
        bullet1.directionOfTravel = travelDirection;
        bullet1.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x + 500, this.transform.position.y, this.transform.position.z+500);
        spawnLocation = new Vector3(this.transform.position.x + .5f, this.transform.position.y, this.transform.position.z + .5f);
        GameObject bulletGO2 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet2 = bulletGO2.GetComponent<Bullet>();
        bullet2.directionOfTravel = travelDirection;
        bullet2.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 500);
        spawnLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);
        GameObject bulletGO3 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet3 = bulletGO3.GetComponent<Bullet>();
        bullet3.directionOfTravel = travelDirection;
        bullet3.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x -500, this.transform.position.y, this.transform.position.z + 500);
        spawnLocation = new Vector3(this.transform.position.x - .5f, this.transform.position.y, this.transform.position.z + .5f);
        GameObject bulletGO4 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet4 = bulletGO4.GetComponent<Bullet>();
        bullet4.directionOfTravel = travelDirection;
        bullet4.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x - 500, this.transform.position.y, this.transform.position.z);
        spawnLocation = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
        GameObject bulletGO5 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet5 = bulletGO5.GetComponent<Bullet>();
        bullet5.directionOfTravel = travelDirection;
        bullet5.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x - 500, this.transform.position.y, this.transform.position.z -500);
        spawnLocation = new Vector3(this.transform.position.x - .5f, this.transform.position.y, this.transform.position.z - .5f);
        GameObject bulletGO6 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet6 = bulletGO6.GetComponent<Bullet>();
        bullet6.directionOfTravel = travelDirection;
        bullet6.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 500);
        spawnLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
        GameObject bulletGO7 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet7 = bulletGO7.GetComponent<Bullet>();
        bullet7.directionOfTravel = travelDirection;
        bullet7.turretThatShotMe = this.turretThatShotMe;

        travelDirection = new Vector3(this.transform.position.x + 500, this.transform.position.y, this.transform.position.z - 500);
        spawnLocation = new Vector3(this.transform.position.x + .5f, this.transform.position.y, this.transform.position.z - .5f);
        GameObject bulletGO8 = (GameObject)Instantiate(this.shrapnelGameObject, spawnLocation, this.transform.rotation);
        Bullet bullet8 = bulletGO8.GetComponent<Bullet>();
        bullet8.directionOfTravel = travelDirection;
        bullet8.turretThatShotMe = this.turretThatShotMe;


    }
}
