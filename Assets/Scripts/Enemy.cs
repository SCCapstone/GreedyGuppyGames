using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;

    public int health = 100;

    public int value = 50;

    public GameObject deathEffect;

    private Transform target;
    private int wavepointIndex = 0;

    private void Start()
    {
        target = Waypoints.points[0];
    }

    public void TakeDamage (int amount) {
        health -= amount;
        if (health <= 0) {
            Die();
        }
    }

    void Die () {
        PlayerStats.Money += value;
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }

    private void Update()
    {
        // Direction pointing to waypoint
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized*speed*Time.deltaTime, Space.World);

        // Checks if we are verrrrry close to a waypoint
        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        // Enemy has reached the end
        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return; // makes sure the code doesn't skip into next node segment (yes this happens)
        }

        // Not at the end, find next waypoint
        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }

    void EndPath () {
        PlayerStats.Lives-=25;
        Destroy(gameObject);
    }
}
