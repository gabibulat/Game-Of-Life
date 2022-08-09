using UnityEngine;
using TMPro;
using Mirror;
using System.Collections.Generic;

public class OnlineGameOver : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverWindow;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parentContent;
    [SerializeField] private UIManager2 _ui;
    [SerializeField] private TMP_Text _winner;
    private List<TMP_Text> _scoreTexts = new();

    public void GameOver(SyncDictionary<Color, int> playerScore, SyncDictionary<Color, string> playerNames)
    {
        string winner = "";
        Color winnerColor = Color.white;
        int old = 0;
        _ui.DisableInteractions(false);
        _gameOverWindow.SetActive(true);

        for (int i = 0; i < _scoreTexts.Count; i++) Destroy(_scoreTexts[i].gameObject);
        _scoreTexts.Clear();

        foreach (var item in playerScore)
        {
            if (item.Value > old)
            {
                old = item.Value;
                winner = playerNames[item.Key];
                winnerColor = item.Key;
            }
            else if (item.Value == old)
            {
                winner = "";
                winnerColor = Color.white;
            }
            GameObject scoreGameObject = Instantiate(prefab);
            scoreGameObject.transform.SetParent(parentContent.transform);
            TMP_Text scoreText = scoreGameObject.GetComponent<TMP_Text>();
            scoreText.text = playerNames[item.Key] + ": " + item.Value;
            scoreText.color = item.Key;
            _scoreTexts.Add(scoreText);

        }
        if (winner == "")
        {
            _winner.text = "It's a tie";
            _winner.color = winnerColor;
        }
        else
        {
            _winner.text = winner + " WON!";
            _winner.color = winnerColor;
        }
    }


    public void DisableGameWindow()
    {
        for (int i = 0; i < _scoreTexts.Count; i++) Destroy(_scoreTexts[i].gameObject);
        _scoreTexts.Clear();
        _ui.DisableInteractions(true);
    }

}
