﻿// <copyright file="Menu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //Clicking Play will advance the player to the Level select scene(when it is finished, currently advances to the play scene)
    public void StartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Closes out the application
    public void QuitGame()
    {
        Application.Quit();
    }
}
