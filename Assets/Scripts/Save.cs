// <copyright file="Save.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save
{
    //Saves player data
    public static void SavePlayerData(bool one, bool oneNoDam, bool two, bool twoNoDam, bool three, bool threeNoDam, bool greedyGuppy)
    {   
        //formatter here turns our save data into a binary file
        BinaryFormatter formatter = new BinaryFormatter();
        //Where the save data will be saved on the machine
        string savePath = Application.persistentDataPath + "/player";
        //Creating a stream for the save data
        FileStream fileStream = new FileStream(savePath, FileMode.Create);
        //"Gathering" the save data
        SaveData saveData = new SaveData(one, oneNoDam, two, twoNoDam, three, threeNoDam, greedyGuppy);
        //Sending the save data to file
        formatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    //Loads player data
    public static SaveData LoadPlayerData()
    {
        //Where the save data was be saved on the machine (if it exists)
        string savePath = Application.persistentDataPath + "/player";
        if(File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            //Creating a stream for the save data
            FileStream fileStream = new FileStream(savePath, FileMode.Open);
            //formatter here turns the binary file into SaveData
            SaveData saveData = formatter.Deserialize(fileStream) as SaveData;
            fileStream.Close();
            return saveData;
        }
        //No save file exists
        else
        {
            Debug.LogError("Save file not found in " + savePath);
            return null;
        }
    }

    //Deletes player data
    public static void DeletePlayerData()
    {   
        //Path where save file is located
        string savePath = Application.persistentDataPath + "/player";
        try
        {   
            //Attempts to delete save file
            File.Delete(savePath);
        }
        catch
        {   
            //Logs error if failed
            Debug.LogError("Save file not found in " + savePath);
        }
    }
}
