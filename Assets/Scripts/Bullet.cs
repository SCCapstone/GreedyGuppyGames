// <copyright file="Bullet.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 dir;

    public float speed = 70f;
    public int damage = 50;
    public float explosionRadius = 0f;
    public GameObject impactEffect;

    public void Seek(Transform aTarget)
    {
        this.target = aTarget;
        this.dir = Dumbfire(dir);
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        float distanceThisFrame = this.speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            this.HitTarget();
            return;
        }

        this.transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        this.transform.LookAt(this.target);
    }

    private void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(this.impactEffect, this.transform.position, this.transform.rotation);
        Destroy(effectIns, 2f);

        if (this.explosionRadius > 0f)
        {
            this.Explode();
        }
        else
        {
            this.Damage(this.target);
        }

        Destroy(this.gameObject);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                this.Damage(collider.transform);
            }
        }
    }

    private void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.TakeDamage(this.damage);
        }
    }

    // for bullets that don't track their targets after being fired
    private Vector3 Dumbfire(Vector3 aDir) 
    {
        aDir = this.target.position - this.transform.position;
        return aDir;
    }

    // When called inside of an Update function this causes bullets to home in on their target
    private Vector3 Homing(Vector3 aDir) {
        aDir = this.target.position - this.transform.position;
        return aDir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
    }
}
