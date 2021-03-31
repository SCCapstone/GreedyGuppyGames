// <copyright file="SaveData.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

[System.Serializable]
public class SaveData
{
    //Bools for determining achievements
    public bool level1;
    public bool level1NoDam;
    public bool level2;
    public bool level2NoDam;
    public bool level3;
    public bool level3NoDam;
    public bool greedyGuppyBag;
    
    //Constructor for SaveData
    public SaveData(bool one, bool oneNoDamage, bool two, bool twoNoDamage, bool three, bool threeNoDamage, bool greedyGuppy) 
    {
        level1 = one;
        level1NoDam = oneNoDamage;

        level2 = two;
        level2NoDam = twoNoDamage;

        level3 = three;
        level3NoDam = threeNoDamage;

        greedyGuppyBag = greedyGuppy;
    }
}
