// <copyright file="Menu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MenuFunctions
{

    public GameObject MainPanel, SettingsPanel, AchievementsPanel, LevelSelectPanel, Level1Panel, Level2Panel, Level3Panel, ErasePanel;

    void Start()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows the Main menu panel
    public void ShowMainPanel()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    } 

    //Shows the Settings panel
    public void ShowSettingsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows the Achievements panel
    public void ShowAchievementsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(true);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows Level Select panel
    public void ShowLevelSelectPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows Level One panel
    public void ShowLevel1Panel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(true);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows Level Two panel
    public void ShowLevel2Panel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(true);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(false);
    }

    //Shows Level Three panel
    public void ShowLevel3Panel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(true);
        ErasePanel.SetActive(false);
    }

    public void ShowErasePanel()
    {
        MainPanel.SetActive(false);
        //SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(true);
    }

    //Loads Level One
    public void LoadLevelOne()
    {
        //SceneManager.LoadScene("MainScene");
        SceneManager.LoadScene("Level1");
    }

    //Loads Level Two
    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    //Loads Level Three
    public void LoadLevelThree()
    {
        //Actually Level 4
        SceneManager.LoadScene("Level4");
    }
}