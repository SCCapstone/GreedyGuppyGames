using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerupspike : Powerupbase
{
    [SerializeField] int life = 3;
    [SerializeField] SphereCollider radius;
    List<Enemy> Enemieswhodamaged = new List<Enemy>();
    public override void Activate()
    {
        base.Activate();
        InvokeRepeating("Checkforenemies", 0, .25f);
        InvokeRepeating("Clearlist", 0, 5f);
    }

    public override void Deactivate()
    {
        CancelInvoke("Checkforenemies");
        CancelInvoke("Clearlist");
        base.Deactivate();
    }
    void Checkforenemies()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius.radius);
        foreach (Collider collider in colliders)
        {
            // if it hits an enemy it damages it and reduces the amount of things it can still hit
            if (collider.tag == "Enemy" && ! Enemieswhodamaged.Contains(collider.GetComponent<Enemy>()))
            {
                Debug.Log("I hit");
                collider.GetComponent<Enemy>().TakeDamage(75);
                Enemieswhodamaged.Add(collider.GetComponent<Enemy>());
                life--;
            }
        }
        
        if (life <= 0)
            Deactivate();
    }

    void Clearlist()
    {
        Enemieswhodamaged.Clear();
    }

    
}
