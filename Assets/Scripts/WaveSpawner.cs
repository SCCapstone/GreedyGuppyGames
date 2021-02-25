// <copyright file="WaveSpawner.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    //Enemy Prefabs
    public Transform grubPrefab;
    public Transform scorpPrefab;
    public Transform dronePrefab;
    public Transform mamaPrefab;
    public Transform carrierPrefab;
    public static Transform spawnPoint;

    //Array[15,4] for Spawning enemies(0:grub, 1:scorp, 2:drone, 3:mama, 4:carrier)
    private int[,] spawnerIndex = { {1,1,1,0,0},
                                    {5,1,1,0,0},
                                    {7,2,1,0,0},
                                    {7,1,2,0,0},
                                    {9,2,1,1,0},
                                    {10,3,2,0,0},
                                    {10,5,3,1,1},
                                    {15,7,5,1,1},
                                    {20,10,5,3,1},
                                    {15,10,5,5,1},
                                    {10,5,1,1,2},
                                    {15,10,3,2,3},
                                    {20,15,5,3,2},
                                    {15,10,7,3,3},
                                    {15,10,10,10,5} };
    
    private bool allSpawned, grubSpawned, scorpSpawned, droneSpawned, mamaSpawned, carrierSpawned = false;

    public float timeBetweenWaves = 5f;
    public float countdown = 11f;

    public Text roundText;
    private int round = 1;

    //testing
    private int grub = 0;
    private int scorp = 0;
    private int drone = 0;
    private int mamas = 0;
    private int carrier = 0;

    private int totalSpawned = 0;

    private void Start()
    {
        // Locates START(green block) in the scene
        Transform spawnTransform = GameObject.Find("START").transform;
        // Declares that the static spawnPoint takes on the transform of START(green block)
        spawnPoint = spawnTransform;
        this.StartCoroutine(this.SpawnWave());
    }
    private void Update()
    {
        //  ***TODO*** : Need to put for loop here and not in SpawnWave as SpawnWave is only supposed to spwan 1 wave not the whole 15 waves
        this.countdown -= Time.deltaTime;

        this.countdown = Mathf.Clamp(this.countdown, 0f, Mathf.Infinity);
        this.roundText.text = ("Round: " + this.round);
    }

    private IEnumerator SpawnWave()
    {
        //Extracts the amount of the coresponding enemy to spawn
        //15 waves(i), 4 enemy types(j)
        for(int i=0; i < spawnerIndex.GetLength(0); ++i)
        {
            Debug.Log("Wave "+(i+1));
            for(int j=0; j < spawnerIndex.GetLength(1); ++j)
            {
                int amountSpanwed = spawnerIndex[i,j];
                totalSpawned += amountSpanwed;
                switch(j)
                {
                    case 0: //grub
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            ++grub;
                            Debug.Log(amountSpanwed+ " Grubs Spawning");
                            SpawnEnemy(grubPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        grubSpawned = true;
                        break;
                    case 1: //scorp
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            ++scorp;
                            Debug.Log(amountSpanwed+ " Scorps Spawning");
                            SpawnEnemy(scorpPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        scorpSpawned = true;
                        break;
                    case 2: //drone
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            ++drone;
                            Debug.Log(amountSpanwed+ " Drones Spawning");
                            SpawnEnemy(dronePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        droneSpawned = true;
                        break;
                    case 3: //mama
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            ++mamas;
                            Debug.Log(amountSpanwed+ " Mamas Spawning");
                            SpawnEnemy(mamaPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        mamaSpawned = true;
                        break;
                    case 4: //carrier
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            ++carrier;
                            Debug.Log(amountSpanwed+ " Carrier Spawning");
                            SpawnEnemy(carrierPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        carrierSpawned = true;
                        break;
                }
                
            }
            
            Debug.Log("Total spawned: "+totalSpawned
                +"\n Grubs: "+grub
                +"\n Scorps: "+scorp
                +"\n Drones: "+drone
                +"\n Mamas: "+mamas
                +"\n Carriers: "+carrier);
            //grub = scorp = drone = mamas = carrier = 0;
            

            //Increments the round counter
            
            if(grubSpawned == true && scorpSpawned == true && droneSpawned == true && mamaSpawned == true && carrierSpawned == true)
            {
                allSpawned = true;
            }
            yield return new WaitUntil(() => allSpawned == true);
            if(allSpawned == true)
            {
                allSpawned = grubSpawned = scorpSpawned = droneSpawned = mamaSpawned = carrierSpawned = false;
                ++this.round;
                ++PlayerStats.Rounds;
            }
        }
    
        // Debug.Log("milfs = " + numMilfs);
        // Debug.Log("groobers = " + numGroobers);
        // Debug.Log("total enemies = " + (numGroobers + (numMilfs * 2)));
    }

    public static void SpawnEnemy(Transform enemy)
    {
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
    //Code originally from SpawnWave()
        //if (round1 < TEN)  //Round 1: 10 waves
        // {
        //     ++round1;
        //     ++this.waveIndex; //adds a single enemy(Grub) per wave
        //     PlayerStats.Rounds++;
        //     // Debug.Log("Grubs to spawn: " + this.waveIndex);

        //     //spawns 1 Grub per loop
        //     for (int i = 0; i < this.waveIndex; i++)
        //     {
        //         SpawnEnemy(grubPrefab);

        //         numGroobers++;

        //         yield return new WaitForSeconds(0.5f);
        //     }

        //     if (round1 == TEN)
        //     {
        //         // Debug.Log("Spawning a single mama");
        //         SpawnEnemy(mamaPrefab); //spawns a single mama

        //         numMilfs++;

        //         // Debug.Log("Round 1 over");
        //         this.waveIndex = 5;  //will spawn 5 enemies (after waveIndex increments) at the start of round 2
        //         //pause the game here, wait for player to resume
        //         //Moves the round text to round 2
        //         ++round;
        //     }
        // }

        // //need to pause the spawning of new enemies until all the enemies of the previous round have been spawned

        // else if (round2 < TEN)  //Round 2: 20 waves  (way too long, will shorten for now)
        // {
        //     PlayerStats.Rounds++;
        //     ++round2;
        //     if (round2 == 1)
        //     {
        //         yield return new WaitForSeconds(timeBetweenRounds);
        //     }
        //     // Debug.Log("Grubs to spawn: " + this.waveIndex);

        //     for (int i = 0; i < this.waveIndex; i++)
        //     {
        //         SpawnEnemy(grubPrefab);

        //         numGroobers++;

        //         yield return new WaitForSeconds(0.5f);
        //     }

        //     this.waveIndex += 2; //adds 2 enemies(Grub) per wave

        //     if (round2 >= 5)
        //     {
        //         ++mamaIndex; //adds a single mama to spawn per wave
        //         // Debug.Log("Spawn "+mamaIndex+" mama(s)");
        //         for (int i = 0; i < mamaIndex; i++)
        //         {
        //             SpawnEnemy(mamaPrefab);

        //             numMilfs++;

        //             yield return new WaitForSeconds(1f);

        //         }
        //     }

        //     if (round2 == TEN)
        //     {
        //         // Debug.Log("Round 2 over");
        //         this.waveIndex = 10;
        //         this.mamaIndex = 3;
        //         //pause the game here, wait for player to resume
        //         //Moves the round text to round 3
        //         ++round;
        //     }
        // }
        // else if (round3 < TEN)  //Round 3: 30 waves  (way too long, will shorten for now)
        // {
        //     PlayerStats.Rounds++;
        //     ++round3;
        //     if (round3 == 1)
        //     {
        //         yield return new WaitForSeconds(timeBetweenRounds);
        //     }
        //     // Debug.Log("Grubs to spawn: " + this.waveIndex);

        //     for (int i = 0; i < this.waveIndex; i++)
        //     {
        //         SpawnEnemy(grubPrefab);

        //         numGroobers++;

        //         yield return new WaitForSeconds(0.5f);
        //     }

        //     this.waveIndex += 3; //adds 3 enemies(Grub) per wave

        //     if (round2 >= 5)
        //     {
        //         this.mamaIndex += 2; //adds 2 more mamas to spawn per wave
        //         // Debug.Log("Spawn " + mamaIndex + " mama(s)");
        //         for (int i = 0; i < mamaIndex; i++)
        //         {
        //             SpawnEnemy(mamaPrefab);

        //             numMilfs++;

        //             yield return new WaitForSeconds(1f);

        //         }
        //     }

        //     if (round3 == TEN)
        //     {
        //         // Debug.Log("Round 3 over");
        //         //pause the game here, wait for player to resume
        //     }
        // }

