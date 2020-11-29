// <copyright file="WaveSpawner.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform grubPrefab;
    public Transform mamaPrefab;

    public Transform spawnPoint;


    public float timeBetweenWaves = 5f;
    public float countdown = 11f;
    public float timeBetweenRounds = 13f;

    public Text waveCountdownText;

    private int waveIndex = 0;
    private int mamaIndex = 0;

    private int round1 = 0;
    private int round2 = 0;
    private int round3 = 0;
    private int TEN = 10;

    private void Update()
    {
        if (this.countdown <= 0f)
        {
            this.StartCoroutine(this.SpawnWave());
            this.countdown = this.timeBetweenWaves;
        }

        this.countdown -= Time.deltaTime;

        this.countdown = Mathf.Clamp(this.countdown, 0f, Mathf.Infinity);

        this.waveCountdownText.text = string.Format("{0:00.00}", this.countdown);
    }

    private IEnumerator SpawnWave()
    {
        if (round1 < TEN)  //Round 1: 10 waves
        {
            ++round1;
            ++this.waveIndex; //adds a single enemy(Grub) per wave
            PlayerStats.Rounds++;
            // Debug.Log("Grubs to spawn: " + this.waveIndex);

            //spawns 1 Grub per loop
            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);
            }

            if (round1 == TEN)
            {
                // Debug.Log("Spawning a single mama");
                this.SpawnEnemy(mamaPrefab); //spawns a single mama
                // Debug.Log("Round 1 over");
                this.waveIndex = 5;  //will spawn 5 enemies (after waveIndex increments) at the start of round 2
                //pause the game here, wait for player to resume
            }
        }

        //need to pause the spawning of new enemies until all the enemies of the previous round have been spawned

        else if (round2 < TEN)  //Round 2: 20 waves  (way too long, will shorten for now)
        {
            PlayerStats.Rounds++;
            ++round2;
            if(round2 == 1)
            {
                yield return new WaitForSeconds(timeBetweenRounds);
            }
            // Debug.Log("Grubs to spawn: " + this.waveIndex);

            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);
            }

            this.waveIndex += 2; //adds 2 enemies(Grub) per wave

            if (round2 >= 5)
            {
                ++mamaIndex; //adds a single mama to spawn per wave
                // Debug.Log("Spawn "+mamaIndex+" mama(s)");
                for (int i = 0; i < mamaIndex; i++)
                {
                    this.SpawnEnemy(mamaPrefab);
                    yield return new WaitForSeconds(1f);
                    
                }
            }

            if (round2 == TEN)
            {
                // Debug.Log("Round 2 over");
                this.waveIndex = 10;
                this.mamaIndex = 3;
                //pause the game here, wait for player to resume
            }
        }
        else if (round3 < TEN)  //Round 3: 30 waves  (way too long, will shorten for now)
        {
            PlayerStats.Rounds++;
            ++round3;
            if(round3 == 1)
            {
                yield return new WaitForSeconds(timeBetweenRounds);
            }
            // Debug.Log("Grubs to spawn: " + this.waveIndex);

            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);
            }

            this.waveIndex += 3; //adds 3 enemies(Grub) per wave

            if (round2 >= 5)
            {
                this.mamaIndex += 2; //adds 2 more mamas to spawn per wave
                // Debug.Log("Spawn " + mamaIndex + " mama(s)");
                for (int i = 0; i < mamaIndex; i++)
                {
                    this.SpawnEnemy(mamaPrefab);
                    yield return new WaitForSeconds(1f);

                }
            }

            if (round3 == TEN)
            {
                // Debug.Log("Round 3 over");
                //pause the game here, wait for player to resume
            }
        }
    }

    private void SpawnEnemy(Transform enemy)
    {
        Instantiate(enemy, this.spawnPoint.position, this.spawnPoint.rotation);
    }
}
