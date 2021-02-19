// <copyright file="Menu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MenuFunctions
{

    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject AchievementsPanel;
    public GameObject LevelSelectPanel;

    void Start()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
    }

    //Shows the Main menu panel
    public void ShowMainPanel()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
    } 

    //Shows the Settings panel
    public void ShowSettingsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
    }

    //Shows the Achievements panel
    public void ShowAchievementsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(true);
        LevelSelectPanel.SetActive(false);
    }

    //Shows Level Select panel
    public void ShowLevelSelectPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
    }

    //Loads Level One
    public void StartGame()
    {
        //Change to LevelOne when naming convention for levels is implemented
        SceneManager.LoadScene("MainScene");
    }
}