// <copyright file="GamerManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;

// Controls everything in the scene, like a robot dungeon master
public class GamerManager : MonoBehaviour
{
    public bool gameEnded;
    public bool gameWon;
    public GameObject playButton;
    public GameObject fastForwardButton;
    public GameObject regularSpeedButton;

    private BuildManager buildManager;
    public GameObject gameOverUI;
    public PauseMenu pauseMenuUI;
    public UpgradeUI upgradeUI;
    public GameObject gameWonUI;
    private Scene activeScene;

    // Runs before frame 1, sets up status
    private void Start()
    {
        this.gameEnded = false;
        this.gameWon = false;
        this.buildManager = BuildManager.instance;
        this.gameOverUI.SetActive(false);
        this.gameWonUI.SetActive(false);
        Time.timeScale = 1f;
        activeScene = SceneManager.GetActiveScene();
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

        //comparing the name of the active scene
        string name = activeScene.name;
        if(name == "Level1")
        {
            PlayerStats.CompleteLevel(1);
        }
        else if(name == "Level2")
        {
            PlayerStats.CompleteLevel(2);
        }
        // Our naming convention is weird, renaming scenes leads to huge merge conflicts
        else if(name == "Level4")
        {
            PlayerStats.CompleteLevel(3);
        }
    }

    public void fastForward()
    {
        Time.timeScale = 1.5f;
        regularSpeedButton.SetActive(true);
        fastForwardButton.SetActive(false);
        playButton.SetActive(false);
    }
    public void changeToRegularSpeed()
    {
        Time.timeScale = 1f;
        fastForwardButton.SetActive(true);
        regularSpeedButton.SetActive(false);
        playButton.SetActive(false);

    }

    public void resetPlayButton()
    {
        Time.timeScale = 1f;
        fastForwardButton.SetActive(false);
        regularSpeedButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void resetFastForwardButton()
    {
        playButton.SetActive(false);
        regularSpeedButton.SetActive(false);
        fastForwardButton.SetActive(true);
    }
    
}
