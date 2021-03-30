// <copyright file="Save.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save
{
    public static void SavePlayerData(bool one, bool two, bool three)
    {   
        // formatter here turns our save data into a binary file
        BinaryFormatter formatter = new BinaryFormatter();
        //Where the save data will be saved on the machine
        string savePath = Application.persistentDataPath + "/player";
        //Creating a stream for the save data
        FileStream fileStream = new FileStream(savePath, FileMode.Create);
        //"Gathering" the save data
        SaveData saveData = new SaveData(one, two, three);
        //Sending the save data to file
        formatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

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
            Debug.LogError("Save file not found in "+ savePath);
            return null;
        }
    }
}
