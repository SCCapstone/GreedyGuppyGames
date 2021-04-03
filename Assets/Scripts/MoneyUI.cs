// <copyright file="MoneyUI.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.UI;

// Renders the player's money
public class MoneyUI : MonoBehaviour
{
    public Text moneyText;

    // Update is called once per frame
    private void Update()
    {
        this.moneyText.text = "$" + PlayerStats.Money;
    }
}
