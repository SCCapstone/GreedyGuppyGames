// <copyright file="Bullet.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Bullet : MonoBehaviour, IBullet {
    private Transform target;
    [HideInInspector]
    public Turret turretThatShotMe;

    public float speed = 70f;
    public int damage = 50;
    public float explosionRadius = 0f;
    public GameObject impactEffect;

    public void Seek(Transform aTarget) {
        this.target = aTarget;
    }

    // Update is called once per frame
    private void Update() {
        if (this.target == null) {
            Destroy(this.gameObject);
            return;
        }

        Vector3 dir = this.target.position - this.transform.position;
        float distanceThisFrame = this.speed * Time.deltaTime;

        //old hit detection
        /*if (dir.magnitude <= distanceThisFrame)
        {
            this.HitTarget();
            return;
        }*/

        this.transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        this.transform.LookAt(this.target);
    }

    private void HitTarget() {
        GameObject effectIns = (GameObject)Instantiate(this.impactEffect, this.transform.position, this.transform.rotation);
        Destroy(effectIns, 2f);

        if (this.explosionRadius > 0f) {
            this.Explode();
        }
        else {
            this.Damage(this.target);
        }

        // Destroys the bullet (not the enemy)
        Destroy(this.gameObject);
    }

    private void Explode() {
        // Debug.Log("I'm exploding!!!!");
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius);
        foreach (Collider collider in colliders) {
            if (collider.tag == "Enemy") {
                this.Damage(collider.transform);
            }
        }
    }

    private void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null) {
            e.TakeDamage(this.damage);
            e.TakeDamage(this.damage);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
    }

    // What happens when the bullet hits something (yes, this mostly replaces HitTarget)
    void OnCollisionEnter(Collision col) {
        // Debug.Log("I'm colliding with something!");
        GameObject effectIns = (GameObject)Instantiate(this.impactEffect, this.transform.position, this.transform.rotation);
        Destroy(effectIns, 2f);

        if (this.explosionRadius > 0f) {
            this.Explode();
        }
        else {
            this.Damage(col.gameObject.GetComponent<Transform>()); // gets the transform of the gameObject of what was hit and hurts it (the spaghetti is ready)
            // Debug.Log("I'm hitting the enemy!");
        }

        // Destroys the bullet (not the enemy)
        Destroy(this.gameObject);
    }
}
