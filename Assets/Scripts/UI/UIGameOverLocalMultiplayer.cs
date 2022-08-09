using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameOverLocalMultiplayer : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverWindow;
    [SerializeField] private TMP_Text[] _gameOverScore;
    [SerializeField] private UIManager2 _ui;
    [SerializeField] private TMP_Text _winner;

    public void GameOver(Player[] _playerSripts)
    {
        string winner = "";
        Color winnerColor = Color.white;
        int old = 0;
        _ui.DisableInteractions(false);
        _gameOverWindow.SetActive(true);

        for (int i = 0; i < _playerSripts.Length; i++)
        {
            if (_playerSripts[i].GetCellCounter().GetCounter() > old)
            {
                old = _playerSripts[i].GetCellCounter().GetCounter();
                winner = _playerSripts[i].GetName();
                winnerColor = _playerSripts[i].GetPlayerColor();
            }else if (_playerSripts[i].GetCellCounter().GetCounter() == old)
            {
                winner = "";
                winnerColor = Color.white;
            }
            _gameOverScore[i].text = _playerSripts[i].GetName() + ": " + _playerSripts[i].GetCellCounter().GetCounter();
            _gameOverScore[i].color = _playerSripts[i].GetPlayerColor();
            _gameOverScore[i].gameObject.SetActive(true);

        }
        if (winner == "")
        {
            _winner.text = "It's a tie";
            _winner.color = winnerColor;
        }
        else { 
        _winner.text = winner + " WON!";
        _winner.color = winnerColor;
        }

    }

    public void DisableGameWindow()
    {
        gameObject.SetActive(false);
        _ui.DisableInteractions(true);

    }

}
