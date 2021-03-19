using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerupbase : MonoBehaviour
{
    public PowerupManager.PowerUpType type;
    [SerializeField] protected float lifespan = 5;
    public virtual void Activate()
    {
        Debug.Log("activated");
        Invoke("Deactivate", lifespan);
    }
    public virtual void Deactivate()
    {
        CancelInvoke();
        Destroy(gameObject);
    }
}
