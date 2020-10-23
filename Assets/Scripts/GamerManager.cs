using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamerManager : MonoBehaviour
{

    public bool gameEnded;

    public GameObject gameOverUI;

    void Start() {
        gameEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        // actually stops the game so it doesn't loop after ending
        if (gameEnded)
            return;

        //shortcut to end the game quickly for testing
        if(Input.GetKeyDown("e")) {
            EndGame();
        }

        // ends the game if player is dead
        if(PlayerStats.Lives <= 0) {
            EndGame();
        }
    }

    void EndGame() {
        gameEnded = true;

        // Turns on the game over UI when game is over
        gameOverUI.SetActive(true);
    }
}
