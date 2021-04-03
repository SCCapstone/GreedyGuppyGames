// <copyright file="Menu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Renders all the menu components
public class Menu : MenuFunctions
{

    public GameObject MainPanel, SettingsPanel, AchievementsPanel, LevelSelectPanel, Level1Panel, Level2Panel, Level3Panel, ErasePanel;
    public Button level1Button, level2Button, level3Button;
    public bool level1, level2, level3;


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
    
    // Shows the Main menu panel
    void Update()
    {
        UpdateLevels();
        UpdateButtons();
    }

    // Updates the bools keeping track of which levels 
    void UpdateLevels()
    {
        level1 = PlayerStats.globalLevel1;
        level2 = PlayerStats.globalLevel2;
        level3 = PlayerStats.globalLevel3;
    }

    // Updates the locked/unlocked levels on level select
    void UpdateButtons()
    {   
        // Player has not beaten level 1
        // Levels 2 and 3 are locked
        if(level1 == false)
        {
            level1Button.interactable = true;
            level2Button.interactable = false;
            level3Button.interactable = false;
            
        }
        // Player has beaten level 1
        // Level 3 is locked
        else if(level1 == true && level2 == false)
        {
            level1Button.interactable = true;
            level2Button.interactable = true;
            level3Button.interactable = false;
        }
        // Player has beaten level 1 and 2
        // All levels are now unlocked
        else if(level1 == true && level2 == true)
        {
            level1Button.interactable = true;
            level2Button.interactable = true;
            level3Button.interactable = true;
        }
    }

    // Shows the Main menu panel
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

    // Shows the Settings panel
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

    // Shows the Achievements panel
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

    // Shows Level Select panel
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

    // Shows Level One panel
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

    // Shows Level Two panel
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

    // Shows Level Three panel
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

    // Shows the erase save panel
    public void ShowErasePanel()
    {
        MainPanel.SetActive(false);
        AchievementsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
        Level1Panel.SetActive(false);
        Level2Panel.SetActive(false);
        Level3Panel.SetActive(false);
        ErasePanel.SetActive(true);
    }

    // Loads Level One
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    // Loads Level Two
    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    // Loads Level Three
    public void LoadLevelThree()
    {
        // Actually Level 4
        SceneManager.LoadScene("Level4");
    }
}