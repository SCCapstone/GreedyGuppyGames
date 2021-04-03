// <copyright file="MenuFunctions.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Handles what the buttons on the pause screen actually do
public class MenuFunctions : MonoBehaviour
{

    // Reloads and makes sure the game is not paused
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    // Goes to the main menu
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Destroys the application
    public void Quit()
    {
        Application.Quit();
    }
}
