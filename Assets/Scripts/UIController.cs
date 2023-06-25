using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{


    public GameManager gameManager;
    public GameObject menuUI;
    public GameObject gameUI;
    public TextMeshProUGUI scoreText;

    private int _gameSize = 10;

    private bool _gameStarted = false;


    private int _lastScore = 0;

    private void Update()
    {
        if(_lastScore != gameManager.Score)
        {
            UpdateScore();
        }
    }


    public void StartGame()
    {
        if(_gameStarted) return;
        _gameStarted = true;
        gameManager.StartGame(_gameSize);

        menuUI.SetActive(false);
        gameUI.SetActive(true);

    }



    public void ChangeGameSize(int i)
    {
        if (_gameStarted) return;
        Debug.Log("game size :"+i);
        _gameSize = (i+1) * 10;
    }



    public void ChangeGameSpeed(int i)
    {
        Debug.Log("game speed : "+i);
        gameManager.GameSpeed = Mathf.Pow(2,i);
    }

    public void OpenClosePathFinder(bool b)
    {
        gameManager.UsePathFinder = b;
    }


    public void UpdateScore()
    {
        scoreText.text = gameManager.Score+"";
        _lastScore = gameManager.Score;
    }

}
