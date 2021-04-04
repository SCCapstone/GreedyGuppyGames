// <copyright file="PowerupManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager self;
    public enum PowerUpType { bomb, spiketrap };
    public GameObject explosionEffect;
    public int powerUpPrice = 50;
  
    [SerializeField] public Powerupbase bombpowerup;
    [SerializeField] Powerupbase spikepowerup;
    private Powerupbase activepowerup;
    void Awake()
    {
        self = this;
        
    }
    public void selectpowerup(int type)
    {
        switch (type)
        {
            case 0: 
                activepowerup = Instantiate(bombpowerup).GetComponent<Powerupbase>();
                activepowerup.explosionEffect = explosionEffect;
                break;

            case 1: 
                activepowerup = Instantiate(spikepowerup).GetComponent<Powerupbase>();
                
                break;
        }
        Vector3 spawnpose = new Vector3(0, -1000 ,0);
        activepowerup.transform.position = spawnpose;
       
    }
    bool powerupisactive()
    {
        if (activepowerup != null)
            return true;
        else
            return false;

       
    }
    void Update()
    {
        if (powerupisactive() && Input.GetMouseButtonDown(0))
        {
            int layer_mask = LayerMask.GetMask("Raycast");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, layer_mask) && (hit.collider.tag == "Road" ))
            {
                
                placepowerup(hit.point);
            }
        }
    }
    void placepowerup(Vector3 position)
    {
        PlayerStats.Money -= powerUpPrice;
        position.y = position.y + 1;
        activepowerup.transform.position = position;
        activepowerup.Activate();
        activepowerup = null;
    }

}
