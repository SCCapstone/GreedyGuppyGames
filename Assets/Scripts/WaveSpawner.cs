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
    public static Transform spawnPoint;

    //Array[15,4] for Spawning enemies(0:grub, 1:scorp, 2:drone, 3:mama)
    private int[,] spawnerIndex = { {1,1,1,0},
                                    {5,1,1,0},
                                    {7,2,1,0},
                                    {7,1,2,0},
                                    {9,2,1,1},
                                    {10,3,2,0},
                                    {10,5,3,1},
                                    {15,7,5,1},
                                    {20,10,5,3},
                                    {15,10,5,5},
                                    {10,5,1,1},
                                    {15,10,3,2},
                                    {20,15,5,3},
                                    {15,10,7,3},
                                    {15,10,10,10} };

    public float timeBetweenWaves = 5f;
    public float countdown = 11f;

    public Text roundText;
    private int round = 1;

    //testing
    private int numGroobers = 0;
    private int numMilfs = 0;

    private int totalSpawned = 0;

    private void Start()
    {
        // Locates START(green block) in the scene
        Transform spawnTransform = GameObject.Find("START").transform;
        // Declares that the static spawnPoint takes on the transform of START(green block)
        spawnPoint = spawnTransform;
    }

    private void Update()
    {
        if (this.countdown <= 0f)
        {
            this.StartCoroutine(this.SpawnWave());
            this.countdown = this.timeBetweenWaves;
        }

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
            for(int j=0; j < spawnerIndex.GetLength(1); ++j)
            {
                int amountSpanwed = spawnerIndex[i,j];
                totalSpawned += amountSpanwed;
                switch(j)
                {
                    case 0: //grub
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            Debug.Log(amountSpanwed+ " Grubs Spawning");
                            SpawnEnemy(grubPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        
                        break;
                    case 1: //scorp
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            Debug.Log(amountSpanwed+ " Scorps Spawning");
                            SpawnEnemy(scorpPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        break;
                    case 2: //drone
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            Debug.Log(amountSpanwed+ " Drones Spawning");
                            SpawnEnemy(dronePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        break;
                    case 3: //mama
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            Debug.Log(amountSpanwed+ " Mamas Spawning");
                            SpawnEnemy(mamaPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        break;
                }
                Debug.Log("Total spawned: "+totalSpawned);
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

