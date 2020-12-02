// <copyright file="LivesUI.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Text livesText;

    // Update is called once per frame
    private void Update()
    {
        if (PlayerStats.Lives >= 0)
        {
            this.livesText.text = "Lives: " + PlayerStats.Lives;
        }
    }
}
