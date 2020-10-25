// <copyright file="WaveSpawner.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;

    public Transform spawnPoint;

    public float timeBetweenWaves = 5f;
    public float countdown = 11f;

    public Text waveCountdownText;

    private int waveIndex = 0;

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
        this.waveIndex++;
        PlayerStats.Rounds++;

        for (int i = 0; i < this.waveIndex; i++)
        {
            this.SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(this.enemyPrefab, this.spawnPoint.position, this.spawnPoint.rotation);
    }
}
