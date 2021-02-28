// <copyright file="WaveSpawner.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static bool gameWon = false;
    //Enemy Prefabs
    public Transform grubPrefab;
    public Transform scorpPrefab;
    public Transform dronePrefab;
    public Transform beetlePrefab;
    public Transform mamaPrefab;
    public Transform carrierPrefab;

    public static Transform spawnPoint, spawn1, spawn2, spawn3;
    private Transform spawnTransform, start1, start2, start3;


    //Array[15,4] for Spawning enemies(0:grub, 1:scorp, 2:drone, 3:beetle, 4:mama, 5:carrier)
    private int[,] spawnerIndex = { 
                                    {1,1,1,0,0,0},
                                    {5,1,1,1,0,0},
                                    {7,2,1,1,0,0},
                                    {7,1,2,2,0,0},
                                    {9,2,1,1,1,0},
                                    {10,3,2,2,0,0},
                                    {10,5,3,3,1,1},
                                    {15,7,5,3,1,1},
                                    {20,10,5,4,3,1},
                                    {15,10,5,4,5,1},
                                    {10,5,1,3,1,2},
                                    {15,10,3,4,2,3},
                                    {20,15,5,6,3,2},
                                    {15,10,7,10,3,3},
                                    {15,10,10,10,10,5} 
                                    };
    
    

    private int maxRounds = 0;
    public float timeBetweenRounds = 5f;
    public float countdown = 11f;

    public Text roundText;

    private int round = 0;


    //testing
    // private int grub = 0;
    // private int scorp = 0;
    // private int drone = 0;
    // private int mamas = 0;
    // private int carrier = 0;
    // private bool allSpawned, grubSpawned, scorpSpawned, droneSpawned, mamaSpawned, carrierSpawned = false;
    // private int totalSpawned = 0;

    private void Start()
    {
        // Trys to locate the START(green block) in the scene
        try
        {
            spawnTransform = GameObject.Find("START").transform;
        }
        // If not found trys to locate multiple START blocks
        catch
        {
            //Debug.Log("START block not found, searching for START 1, 2, and 3");
            try
            {
                start1 = GameObject.Find("START 1").transform;
                start2 = GameObject.Find("START 2").transform;
                start3 = GameObject.Find("START 3").transform;
            }
            catch
            {
                Debug.LogError("No START block/s found");
            }
        }
        // Declares that the static spawnPoint takes on the transform of START(green block) as long as spawnTransform is not null
        if(spawnTransform != null)
        {
            spawnPoint = spawnTransform;
            single = true;
            //Debug.Log("Assigning single as true");
        }
        // If spawnTransform is null, declares spawn1, 2, and 3 to their respective transforms
        else
        {
            spawn1 = start1;
            spawn2 = start2;
            spawn3 = start3;
            single = false;
            //Debug.Log("Assigning single as false");
        }
        //Debug.Log("Single value: " +single);
    }
    private void Update()
    {
        this.countdown -= Time.deltaTime;
        this.countdown = Mathf.Clamp(this.countdown, 0f, Mathf.Infinity);
        this.roundText.text = ("Round: " + this.round);

    }

    //Pressing the play button calls the SpawnWave function
    public void StartWave()
    {
        //Debug.Log("Play button pressed, starting SpawnWave");
        if(this.checkForEnemies())
        {
            ++this.index;
            ++this.round;
            this.roundText.text = ("Round: " + this.round);
            ++PlayerStats.Rounds;
            this.StartCoroutine(this.SpawnWave(index));
        }
    }

    private IEnumerator SpawnWave(int index)
    {
        //Extracts the amount of the coresponding enemy to spawn
        //15 waves(i), 6 enemy types(j)
        //for(int i=0; i < spawnerIndex.GetLength(0); ++i)
        //{
            spawning = true;
            //Debug.Log("Wave "+(index+1));
            for(int j=0; j < spawnerIndex.GetLength(1); ++j)
            {
                int amountSpanwed = spawnerIndex[index,j];
                //totalSpawned += amountSpanwed;
                switch(j)
                {
                    case 0: //grub
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //++grub;
                            //Debug.Log(amountSpanwed+ " Grubs Spawning");
                            SpawnEnemy(grubPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //grubSpawned = true;
                        break;
                    case 1: //scorp
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //++scorp;
                            //Debug.Log(amountSpanwed+ " Scorps Spawning");
                            SpawnEnemy(scorpPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //scorpSpawned = true;
                        break;
                    case 2: //drone
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //++drone;
                            //Debug.Log(amountSpanwed+ " Drones Spawning");
                            SpawnEnemy(dronePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //droneSpawned = true;
                        break;
                    case 3: //beetle
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //Debug.Log(amountSpanwed+ " Beetle Spawning");
                            SpawnEnemy(beetlePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                            //carrierSpawned = true;
                        break;   
                    case 4: //mama
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //++mamas;
                            //Debug.Log(amountSpanwed+ " Mamas Spawning");
                            SpawnEnemy(mamaPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //mamaSpawned = true;
                        break;
                    case 5: //carrier
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            //++carrier;
                            //Debug.Log(amountSpanwed+ " Carrier Spawning");
                            SpawnEnemy(carrierPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                            //carrierSpawned = true;
                        break;
                }
            }
            spawning = false;
            //Testing
            // Debug.Log("Total spawned: "+totalSpawned
            //     +"\n Grubs: "+grub
            //     +"\n Scorps: "+scorp
            //     +"\n Drones: "+drone
            //     +"\n Mamas: "+mamas
            //     +"\n Carriers: "+carrier);


            // Time between rounds
            yield return new WaitForSeconds(2f);
            // Increments the round counter
            ++this.round;
            ++PlayerStats.Rounds;
        }

    public static void SpawnEnemy(Transform enemy)
    {
        // If single is true there is only one START block
        if(single)
        {
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
        }
        // If single is false there are multiple START blocks
        else 
        {
            Instantiate(enemy, spawn1.position, spawn1.rotation);
            Instantiate(enemy, spawn2.position, spawn2.rotation);
            Instantiate(enemy, spawn3.position, spawn3.rotation);
        }
    }
    // returns true if no enemies in scene
    public bool checkForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
        if(enemies.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}