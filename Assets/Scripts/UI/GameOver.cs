using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TMP_Text winnerText;
    private bool gameOver;

    public bool getGameOver()
    {
        return gameOver;
    }

    void Start()
    {
        gameOver = false;
    }

    public void setup(bool whitesTurn)
    {
        gameOver = true;
        gameObject.SetActive(true);

        if (whitesTurn)
        {
            winnerText.text = "BLACK WON!";
        }
        else
        {
            winnerText.text = "WHITE WON!";
        }
    }

    public void restartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void loadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
