using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuFunctions : MonoBehaviour
{

    // reloads and makes sure the game is not paused
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    //goes to the main menu
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        FindObjectOfType<AudioManager>().FadeToMenuMusic();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
