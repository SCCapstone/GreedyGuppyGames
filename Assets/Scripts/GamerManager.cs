// <copyright file="GamerManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

// Controls everything in the scene, like a robot dungeon master
public class GamerManager : MonoBehaviour
{
    public bool gameEnded;
    public bool gameWon;

    private BuildManager buildManager;
    public GameObject gameOverUI;
    public PauseMenu pauseMenuUI;
    public UpgradeUI upgradeUI;
    public GameObject gameWonUI;

    // Runs before frame 1, sets up status
    private void Start()
    {
        this.gameEnded = false;
        this.gameWon = false;
        this.buildManager = BuildManager.instance;
        this.gameOverUI.SetActive(false);
        this.gameWonUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        // actually stops the game so it doesn't loop after ending
        if (this.gameEnded || this.gameWon)
        { 
            pauseMenuUI.Unpause();
            upgradeUI.Hide();
            Time.timeScale = 0;

            return;
        }

        // cancels the turret selection on right click
        if (Input.GetMouseButtonDown(1))
        {
            this.buildManager.ResetTurretToBuild();
        }

        // shortcut to end the game quickly for testing
        if (Input.GetKeyDown("e"))
        {
            this.EndGame();
        }

        // ends the game if player is dead
        if (PlayerStats.Lives <= 0)
        {
            this.EndGame();
        }
        if (WaveSpawner.gameWon)
        {
            this.WinGame();
        }
    }

    // Halts the game
    private void EndGame()
    {
        this.gameEnded = true;

        // Turns on the game over UI when game is over
        this.gameOverUI.SetActive(true);
    }

    // Determines what happens when the win condition is set
    public void WinGame()
    {
        this.gameWon = true;
        this.gameWonUI.SetActive(true);
    }
}
