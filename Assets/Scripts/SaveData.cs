// <copyright file="SaveData.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

[System.Serializable]
public class SaveData
{
    //Bools determining if the player has cleared a level
    public bool level1;
    public bool level2;
    public bool level3;
    
    //Constructor for SaveData
    public SaveData(bool one, bool two, bool three) 
    {
        level1 = one;
        level2 = two;
        level3 = three;
    }
}
