using System;
using UnityEngine;

//Player manager for local multiplayer.

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameOfLifeManager _gameOfLifeManager;
    [SerializeField] private GameObject _firstPlayer;
    [SerializeField] private Player _firstPlayerScript;
    [SerializeField] private CardGameManager _cardGameManager;
    [SerializeField] private UIGameOverLocalMultiplayer _uiGameOver;
    [SerializeField] private Color[] _colors;
    
    private GameObject[] _players;
    private Player[] _playerSripts;
    private int _order = 0;  

    private void Awake()
    {
        _cardGameManager.onStartAction += StartGameofLife;
        _cardGameManager.onFinishedAction += SwitchPlayer;
        _firstPlayerScript.SetPlayerColor(_colors[0]);
    }

    //When choosing how many players it instantiates players and sets up for playing
    public void SetPlayers(int j, string[] names)
    {
        _players = new GameObject[j];
        _playerSripts = new Player[j];
        _playerSripts[0] = _firstPlayerScript;
        _players[0] = _firstPlayer;
        _playerSripts[0].SetName(names[0]);

        for (int i = 1; i < j; i++)
        {
            GameObject _player = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
            _player.transform.parent = gameObject.transform;
            _player.GetComponent<Player>().SetPlayerColor(_colors[i]);
            _players[i] = _player;
            _playerSripts[i] = _player.GetComponent<Player>();
            _playerSripts[i].SetName(names[i]);
        }
    }

    //Determining if game of life should start or not depending on if everyone played it's turn
    private void StartGameofLife(object sender, EventArgs e)
    {
        bool isSameTurn = false;
        for (int i = 0; i < _playerSripts.Length; i++)
        {
            if (_playerSripts[0].GetTurns() != _playerSripts[i].GetTurns())
            {
                isSameTurn = false;
                _cardGameManager.SetSeconds(0);
                _gameOfLifeManager.SetIsPaused(true);
                break;
            }
            else isSameTurn = true;
        }
        if (isSameTurn)
        {
            _cardGameManager.SetSeconds(2);
            _gameOfLifeManager.SetIsPaused(false);
        }
    }
    
    //Switching player on every finished turn
    public void SwitchPlayer(object sender, EventArgs e)
    {
        _players[_order].SetActive(false);
        if (_order < _players.Length - 1) _order++;
        else _order = 0;

        if (_playerSripts[_order].GetTurns() == 0)
        {
            for (int i = 0; i < _playerSripts.Length; i++) _playerSripts[i].GetCellCounter().CountCells(_cardGameManager.GetGameOfLifeManager().GetCellsArray());
            _uiGameOver.GameOver(_playerSripts);
            return;
        }

        _uiGameOver.DisableGameWindow();
        _playerSripts[_order].onPlayerEnable += _cardGameManager.onPlayerEnabled;
        _players[_order].SetActive(true);
        _playerSripts[_order].SetName(_playerSripts[_order].GetName());
    }

    public void NewGame()
    {
        _players[0].SetActive(false);
        _playerSripts[0].onPlayerEnable += _cardGameManager.onPlayerEnabled;
        _players[0].SetActive(true);
        for (int i = 1; i < _players.Length; i++) Destroy(_players[i]);
        _cardGameManager.NewGame();
    }
}
