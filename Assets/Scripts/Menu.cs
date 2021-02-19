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
    public void ShowSettingsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
    } 
    public void ShowAchievementsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(true);
        LevelSelectPanel.SetActive(false);
    }
    public void ShowLevelSelectPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
    }










    //Clicking Play will advance the player to the Level select scene(when it is finished, currently advances to the play scene)
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}