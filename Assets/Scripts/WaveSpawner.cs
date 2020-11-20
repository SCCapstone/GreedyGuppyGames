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

    public Text waveCountdownText;

    private int waveIndex = 0;

    private int round1 = 0;
    private int TEN = 10;
    private bool waveOneDone = false;

    private int round2 = 0;
    private int TWENTY = 20;
    private bool waveTwoDone = false;

    private int round3 = 0;
    private int THIRTY = 30;
    private bool waveThreeDone = false;

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
            ++this.waveIndex;
            PlayerStats.Rounds++;
            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);

                Debug.Log("Enemies to spawn: " + this.waveIndex);
            }
            
            if (round1 == TEN)
            {
                this.SpawnEnemy(mamaPrefab);
                yield return new WaitForSeconds(7.0f);
                Debug.Log("Round 1 over");
                this.waveIndex = 5;  //will spawn 5 enemies (after waveIndex increments) at the start of round 2
                //pause the game here, wait for player to resume
            }
        }

        //need to pause the spawning of new enemies until all the enemies of the previous round have been spawned

        else if (round2 < TEN)  //Round 2: 20 waves  (way too long, will shorten for now)
        {
            PlayerStats.Rounds++;
            ++round2;
            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);

                Debug.Log("Enemies to spawn: " + this.waveIndex);
            }
            this.waveIndex += 2; //adds 2 enemeis per wave
            if (round2 == 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.SpawnEnemy(mamaPrefab);
                    yield return new WaitForSeconds(0.5f);
                    
                }
            }
            if (round2 == TEN)
            {
                this.SpawnEnemy(mamaPrefab);
                yield return new WaitForSeconds(7.0f);
                Debug.Log("Round 2 over");
                this.waveIndex = 10;
                //pause the game here, wait for player to resume
            }
        }
        else if (round3 < TEN)  //Round 3: 30 waves  (way too long, will shorten for now)
        {
            PlayerStats.Rounds++;
            ++round3;
            for (int i = 0; i < this.waveIndex; i++)
            {
                this.SpawnEnemy(grubPrefab);
                yield return new WaitForSeconds(0.5f);

                Debug.Log("Enemies to spawn: " + this.waveIndex);
            }
            this.waveIndex += 3; //adds 3 enemeis per wave
            if (round3 == TEN)
            {
                Debug.Log("Round 3 over");
                //pause the game here, wait for player to resume
            }
        }
    }

    private void SpawnEnemy(Transform enemy)
    {
        Instantiate(enemy, this.spawnPoint.position, this.spawnPoint.rotation);
    }
}
