using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTurret : MonoBehaviour
{ 
    public float range = 15f;
    public string towerTag = "Tower";
    // Start is called before the first frame update
    void Start()
    {
        this.InvokeRepeating("UpdateTargetTower", 0f, 0.2f);
    }

    // Update is called once per frame
    void UpdateTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(this.towerTag);
        
        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(this.transform.position, tower.transform.position);
        }

    }
}
