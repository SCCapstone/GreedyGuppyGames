// <copyright file="PlayerStats.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

// Initializes everything that defines a player (their stats!)
public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public static int endMoneyStatic = 3000;
    public int startMoney = 400;

    public static int Lives;
    public static int startLivesStatic;
    public int startLives = 20;

    public static int Rounds;

    //Global save variables
    //Bools for help keeping track of which levels the player has won
    public static bool globalLevel1, globalLevel2, globalLevel3;
    //Bools keeping track of wining a level taking no damage
    public static bool globalLevel1NoDam, globalLevel2NoDam, globalLevel3NoDam;
    //Bool keeping track of winning a level with $3,000 remaining
    public static bool globalGreedyGuppy;

    private void Start()
    {
        Money = this.startMoney;
        Lives = this.startLives;
        startLivesStatic = this.startLives;
        Rounds = 0;

        if(Save.LoadPlayerData() != null){
            //If save file found, loads player data
            LoadPlayerData();
        }
        else
        {
            //Sets all player data to false
            Reset();
        }
    }

    //Saves data when player wins a level and checks achievements
    public static void CompleteLevel(int level)
    {
        switch(level)
        {
            case 1:
                globalLevel1 = true;
                //Takes no damage
                if(Lives == startLivesStatic)
                {
                    globalLevel1NoDam = true;
                }
                //Win with at least $3,000 remaining
                if(Money >= endMoneyStatic)
                {
                    globalGreedyGuppy = true;
                }
                break;
            case 2:
                globalLevel2 = true;
                //Takes no damage
                if(Lives == startLivesStatic)
                {
                    globalLevel2NoDam = true;
                }
                //Win with at least $3,000 remaining
                if(Money >= endMoneyStatic)
                {
                    globalGreedyGuppy = true;
                }
                break;
            case 3:
                globalLevel3 = true;
                //Takes no damage
                if(Lives == startLivesStatic)
                {
                    globalLevel3NoDam = true;
                }
                //Win with at least $3,000 remaining
                if(Money >= endMoneyStatic)
                {
                    globalGreedyGuppy = true;
                }
                break;
        }
        Save.SavePlayerData(globalLevel1, globalLevel1NoDam, globalLevel2, globalLevel2NoDam, globalLevel3, globalLevel3NoDam, globalGreedyGuppy);
        Debug.Log("Saving player data...");
    }

    //Loads player data
    public void LoadPlayerData()
    {
        SaveData saveData = Save.LoadPlayerData();

        //Loads level completion
        globalLevel1 = saveData.level1;
        globalLevel2 = saveData.level2;
        globalLevel3 = saveData.level3;

        //Loads completing a level taking no damage
        globalLevel1NoDam = saveData.level1NoDam;
        globalLevel2NoDam = saveData.level2NoDam;
        globalLevel3NoDam = saveData.level3NoDam;

        //Loads wining a level with $3,000 remaining
        globalGreedyGuppy = saveData.greedyGuppyBag;
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

    //Resets variables to defaults of false
    public void Reset()
    {
        Debug.Log("Resetting data...");
        //Resets level completion
        globalLevel1 = false;
        globalLevel2 = false;
        globalLevel3 = false;

        //Resets completing a level taking no damage
        globalLevel1NoDam = false;
        globalLevel2NoDam = false;
        globalLevel3NoDam = false;

        //Resets completing a level with $3,000 remaining
        globalGreedyGuppy = false;
    }
}
