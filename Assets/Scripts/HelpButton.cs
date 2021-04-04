// <copyright file="HelpButton.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Pauses time when the player clicks help and brings up the help menu
public class HelpButton : MenuFunctions
{
    public static bool GameIsPaused = false;

    public GameObject HelpMenuUI;
    //public GamerManager gamerManager;

    private void Start()
    {
        HelpMenuUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Escape) && !gamerManager.gameEnded)
        // {
        //     if(GameIsPaused)
        //     {
        //         Unpause();
        //     }
        //     else
        //     {
        //         Pause();
        //     }
        // }
    }

    // Stops time
    public void Pause()
    {
        Debug.Log("Game Paused");
        HelpMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // Resumes time
    public void Unpause()
    {
        Debug.Log("Game unpaused");
        HelpMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}

