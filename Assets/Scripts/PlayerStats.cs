// <copyright file="PlayerStats.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public int startMoney = 400;

    public static int Lives;
    public int startLives = 20;

    public static int Rounds;

    //Bools for help keeping track of which levels the player has won
    public bool levelOne, levelTwo, levelThree;
    public static bool completeLevelOne, completeLevelTwo, completeLevelThree = false;

    private void Start()
    {
        Money = this.startMoney;
        Lives = this.startLives;
        Rounds = 0;

        if(Save.LoadPlayerData() != null){
            LoadPlayerData();
        }
        else
        {
            levelOne = levelTwo = levelThree = false;
        }

        //Testing
        Debug.Log("Level one complete: " +levelOne);
        Debug.Log("Level two complete: " +levelTwo);
        Debug.Log("Level three complete: " +levelThree);
    }

    //Saves data when player wins a level
    public static void CompleteLevel(int level)
    {
        switch(level)
        {
            case 1:
                completeLevelOne = true;
                break;
            case 2:
                completeLevelTwo = true;
                break;
            case 3:
                completeLevelThree = true;
                break;
        }
        Save.SavePlayerData(completeLevelOne, completeLevelTwo, completeLevelThree);
        Debug.Log("Saving player data...");
    }

    //Loads player data
    public void LoadPlayerData()
    {
        SaveData saveData = Save.LoadPlayerData();
        levelOne = saveData.level1;
        levelTwo = saveData.level2;
        levelThree = saveData.level3;
        Debug.Log("Loading player data...");
    }

    //Deletes player data
    public void Delete()
    {
        Debug.Log("Deleting save data...");
        Save.DeletePlayerData();
    }
}
