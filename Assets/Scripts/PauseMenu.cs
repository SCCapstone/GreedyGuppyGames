// <copyright file="PauseMenu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Pauses time when the player clicks pause
public class PauseMenu : MenuFunctions
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;
    public GamerManager gamerManager;

    private void Start()
    {
        PauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !gamerManager.gameEnded)
        {
            if(GameIsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    // Stops time
    public void Pause()
    {
        Debug.Log("Game Paused");
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // Resumes time
    public void Unpause()
    {
        Debug.Log("Game unpaused");
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
