// <copyright file="GamerManager.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class GamerManager : MonoBehaviour
{
    public bool gameEnded;

    private BuildManager buildManager;
    public GameObject gameOverUI;

    private void Start()
    {
        this.gameEnded = false;
        this.buildManager = BuildManager.instance;
    }

    // Update is called once per frame
    private void Update()
    {
        // actually stops the game so it doesn't loop after ending
        if (this.gameEnded)
        {
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
    }

    private void EndGame()
    {
        this.gameEnded = true;

        // Turns on the game over UI when game is over
        this.gameOverUI.SetActive(true);
    }
}
