// <copyright file="Enemy.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using DG.Tweening;
public class Enemy : MonoBehaviour
{
    public Animator anim;
    
    public float speed = 10f;

    public int health = 100;

    public int value = 50;

    public float splashModifier = 1;

    public GameObject deathEffect;

    private Transform target;
    protected int wavepointIndex = 0;

    public virtual void Start()
    {
        this.target = Waypoints.points[wavepointIndex];
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z), .25f);
        //anim.Play("Walk Forward Slow WO Root", -1, 0);
    }

    public void TakeDamage(int amount)
    {
        this.health -= amount;
        if (this.health <= 0)
        {
            this.Die();
        }
    }

    public virtual void Die()
    {
        PlayerStats.Money += this.value;
        GameObject effect = (GameObject)Instantiate(this.deathEffect, this.transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(this.gameObject);
        
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
        transform.DOLookAt(new Vector3(target.position.x, transform.position.y, target.position.z ), .25f);
    }

    public void SetWaypoint (int index)
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

<<<<<<< HEAD
    public virtual void EndPath() {

=======
    public virtual void EndPath()
    {
>>>>>>> parent of 2bbccf0... Merge branch 'master' into Chris
        PlayerStats.Lives -= 25;
        Destroy(this.gameObject);
    }
}
