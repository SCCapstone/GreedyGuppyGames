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
        this.roundsText.text = PlayerStats.Rounds.ToString();
    }

    
}
