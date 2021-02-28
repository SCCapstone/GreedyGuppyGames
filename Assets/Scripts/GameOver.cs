// <copyright file="GameOver.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MenuFunctions
{
    public Text roundsText;

    private void OnEnable()
    {
        PlayerStats.Rounds--;
        this.roundsText.text = "You survived for " + PlayerStats.Rounds.ToString() + " rounds";
    }

    
}
