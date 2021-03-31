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
    //Locally used
    //Bools for help keeping track of which levels the player has won
    public bool levelOne, levelTwo, levelThree;
    public static bool completeLevelOne, completeLevelTwo, completeLevelThree = false;

    //Global variables
    public static bool globalLevel1, globalLevel2, globalLevel3;

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
            globalLevel1 = globalLevel2 = globalLevel3 = false;
        }

        //Testing
        // Debug.Log("Level one complete: " +levelOne);
        // Debug.Log("Level two complete: " +levelTwo);
        // Debug.Log("Level three complete: " +levelThree);
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
        levelOne = globalLevel1 = saveData.level1;
        levelTwo = globalLevel2 = saveData.level2;
        levelThree = globalLevel3 = saveData.level3;
        Debug.Log("Loading player data...");
    }

    //Deletes player data
    public void Delete()
    {
        Debug.Log("Deleting save data...");
        //Deletes save
        Save.DeletePlayerData();
        //Resets varaibles that are not updated when save is deleted
        Reset();
    }

    //Resets variables to defaults
    public void Reset()
    {
        Debug.Log("Resetting data...");
        levelOne = completeLevelOne = globalLevel1 = false;
        levelTwo = completeLevelTwo = globalLevel2 = false;
        levelThree = completeLevelThree = globalLevel3 = false;
    }
}
