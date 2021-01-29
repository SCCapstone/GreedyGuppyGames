using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MenuFunctions
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;


    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Debug.Log("Game Paused");
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Unpause()
    {
        Debug.Log("Game unpaused");
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
