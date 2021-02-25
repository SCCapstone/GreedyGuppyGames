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

    private Vector3 directionOfTravel;

    public void Seek(Transform aTarget)
    {
        this.target = aTarget;
    }

    // Update is called once per frame
    private void Update()
    {
        this.CheckOutOfBounds();
        this.CheckLifeSpan();
        Vector3 dir = this.directionOfTravel;
        float distanceThisFrame = this.speed * Time.deltaTime;

        //old hit detection
        /*if (dir.magnitude <= distanceThisFrame)
        {
            this.HitTarget();
            return;
        }*/
        dir = checkY(dir);

        this.transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        this.transform.LookAt(this.directionOfTravel);
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
    public void SetBulletStats(float speed, int damage, float explosionRadius, int pierce, Turret turretThatShotMe, int explosionPierce)
    {
        this.speed = speed;
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.pierce = pierce;
        this.turretThatShotMe = turretThatShotMe;
        this.explosionPierce = explosionPierce;
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
        this.directionOfTravel = this.target.position - this.transform.position;
    }
    
}
