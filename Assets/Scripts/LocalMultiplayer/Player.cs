using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Player for card game. Used in all 3 of the card game scenes

public class Player : MonoBehaviour
{
    private TMP_Text _playerNameText;
    private CellCounter _cellCounter = new();
    private List<Card> _currentCards = new();
    private List<Card> _deckOfCards = new();
    [SerializeField] private Color _playerColor;
    private int _turns = 6;
    private string _playerName;

    public int _score = 0;
    public event EventHandler<Player> onPlayerEnable;
    public event EventHandler<Player> onPlayerChangedScore;

    private void Awake() => Initialize();

    public void Initialize()
    {
        _turns = 6;
        GameObject _deck = Resources.Load("Prefabs/DeckOfCards") as GameObject;
        _deckOfCards.Clear();
        _deckOfCards.AddRange(_deck.GetComponent<Deck>().GetDeckOfCards());
        _currentCards.Clear();
        _cellCounter.SetCellColor(_playerColor);
        _cellCounter.SetCounter(0);
        GameObject gameObjectText = GameObject.FindGameObjectWithTag("PlayerName");
        if (gameObjectText != null) _playerNameText = gameObjectText.GetComponent<TMP_Text>();
    }

    private void OnEnable() => onPlayerEnable?.Invoke(this, this);

    public void SetScore(int score)
    {
        _score = score;
        onPlayerChangedScore?.Invoke(this, this);
    }
    public void DrawCard(int i)
    {
        _currentCards.Add(_deckOfCards[i]);
        _deckOfCards.RemoveAt(i);
    }
    public void DroppedCard(Card card)
    {
        for (int i = 0; i < _currentCards.Count; i++)
        {
            if (_currentCards[i] == card) _currentCards.RemoveAt(i);
        }
    }
    public void SetPlayerColor(Color color)
    {
        _playerColor = color;
        _cellCounter.SetCellColor(_playerColor);
    }
    public void SetName(string name)
    {
        _playerName = name;
        if (_playerNameText != null)
        {
            _playerNameText.text = "Player: " + _playerName;
            _playerNameText.color = _playerColor;
        }
    }

    #region BasicGettersAndSetters

    public CellCounter GetCellCounter() => _cellCounter;
    public List<Card> GetCurrentCards() => _currentCards;
    public List<Card> GetDeck() => _deckOfCards;
    public Color GetPlayerColor() => _playerColor;
    public int GetTurns() => _turns;
    public void SetTurns(int turns) => _turns = turns;
    public string GetName() => _playerName;
    
    #endregion BasicGettersAndSetters

}
