using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverWindow;
    [SerializeField] private TMP_Text _gameOverScore;
    [SerializeField] private UIManager2 _ui;
    public void GameOver(int score)
    {
        _gameOverWindow.SetActive(true);
        _gameOverScore.text = "Score " + score;
        _ui.DisableInteractions(false);

    }
    public void DisableGameWindow()
    {
        gameObject.SetActive(false);
        _ui.DisableInteractions(true);
        _ui.DisableUndoInteractions(false);
    }

}
