using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public Text roundsText;

    private void OnEnable() {
        roundsText.text = PlayerStats.Rounds.ToString();
    }

    public void Retry () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu () {
        Debug.Log("Go to menu.");
    }

    public void Quit () {
        Application.Quit();
    }
}
