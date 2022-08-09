using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using System.Linq;
public class ScoreBoard : NetworkBehaviour
{
    public SyncDictionary<Color, int> playerStats = new SyncDictionary<Color, int>();
    public SyncDictionary<Color, string> playerNames = new SyncDictionary<Color, string>();

    private Dictionary<Color, TMP_Text> _scoreTexts = new Dictionary<Color, TMP_Text>();
    [SerializeField] private GameObject prefab;

    [Server]
    public void Remove(Color color)
    {
        if (playerNames.ContainsKey(color))
        {
            playerStats.Remove(color);
            playerNames.Remove(color);
        }
        DeleteScore(color);
        if (_scoreTexts.ContainsKey(color))
        {
            Destroy(_scoreTexts[color].gameObject);
            _scoreTexts.Remove(color);
        }
    }
    [ClientRpc]
    public void DeleteScore(Color color)
    {
        if (_scoreTexts.ContainsKey(color))
        {
            Destroy(_scoreTexts[color].gameObject);
            _scoreTexts.Remove(color);
        }
    }
    
    public override void OnStartClient()
    {
        _scoreTexts.Clear();
        foreach (Transform child in gameObject.transform) Destroy(child.gameObject);

        if (playerStats.Count != _scoreTexts.Count)
        {
            foreach (var item in playerStats)
            {

                GameObject scoreGameObject = Instantiate(prefab);
                scoreGameObject.transform.SetParent(gameObject.transform);
                TMP_Text scoreText = scoreGameObject.GetComponent<TMP_Text>();
                scoreText.text = playerNames[item.Key] + ":   " + item.Value;
                scoreText.color = item.Key;
                _scoreTexts.Add(item.Key, scoreText);

            }
        }
    }
    public void AddPlayer(Player player)
    {
        if (!playerStats.ContainsKey(player.GetPlayerColor()))
        {
            CmdCreateScoreText(player.GetPlayerColor(), player.GetName(), player._score);
            player.onPlayerChangedScore += UpdateScore;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdCreateScoreText(Color color, string name, int score)
    {
        playerStats.Add(color, score);
        playerNames.Add(color, name);
        RPCCreateScoreText(color, name);
        if (isServerOnly) CreateScoreText(color, name);
    }

    [ClientRpc]
    public void RPCCreateScoreText(Color color, string name) => CreateScoreText(color,name);

    private void CreateScoreText(Color color, string name)
    {
        if (_scoreTexts.ContainsKey(color))
        {
            GameObject.Destroy(_scoreTexts[color].gameObject);
            _scoreTexts.Remove(color);
        }
        GameObject scoreGameObject = Instantiate(prefab);
        scoreGameObject.transform.SetParent(gameObject.transform);
        TMP_Text scoreText = scoreGameObject.GetComponent<TMP_Text>();
        scoreText.text = name + ":   0";
        scoreText.color = color;
        _scoreTexts.Add(color, scoreText);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateScore(Color color, string name, int score)
    {
        playerStats[color] = score;
        if (isServerOnly) _scoreTexts[color].text = name + ":   " + score;
        RPCupdateScore(color, name, score);
    }

    [ClientRpc]
    public void RPCupdateScore(Color color, string name, int score)=> _scoreTexts[color].text = name + ":   " + score;
    private void UpdateScore(object sender, Player player)
    {

        if (playerStats.ContainsKey(player.GetPlayerColor()))
        {
            if (isServerOnly)
            {
                playerStats[player.GetPlayerColor()] = player._score;
                RPCupdateScore(player.GetPlayerColor(), player.GetName(), player._score);
            }
            else CmdUpdateScore(player.GetPlayerColor(), player.GetName(), player._score);
        }
    }

    public void NewGame()
    {
        foreach (var item in playerNames)
        {
            if (isServerOnly)
            {
                playerStats[item.Key] = 0;
                RPCupdateScore(item.Key, item.Value, 0);
            }
            else CmdUpdateScore(item.Key, item.Value, 0);
        }
    }

}
