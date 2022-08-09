using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections;
using TMPro;

//Player manager for online multiplayer. Function is to switch players and manage things that follow (start game of life, gameOver).

public class PlayerManagerOnlineMultiplayer : NetworkBehaviour
{
    [SerializeField] private GameOfLifeManager _gameOfLifeManager;
    [SerializeField] private DropCard _dropCard;
    [SerializeField] private Button _undoButton;
    [SerializeField] private TMP_Text _currentlyPlaying;
    [SerializeField] private CardGameManager _cardGameManager;
    [SerializeField] private NetworkManagerHUD _networkManagerHUD;
    [SerializeField] private ScoreBoard _scoreBoard;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private OnlineGameOver _onlineGameOver;

    private Player _localPlayerScript;
    private SyncList<Color> _usedColors = new SyncList<Color>();
    private List<Player> _playerSripts = new List<Player>();
    private int _order;

    [SyncVar] public bool _syncGameStarted = false;
    //_syncLastPlayer => if its the last player in order and _syncTurns0 => if its the last players last turn (for calling game over)
    [SyncVar] public bool _syncLastPlayer = false;
    [SyncVar] public bool _syncTurns0 = false;


    private void Awake()
    {
        _cardGameManager.onFinishedAction += SwitchPlayer;
        _cardGameManager.onStartAction += StartGameofLife;
    }

    #region SettingUpPlayer

    //Gives unique color to player (the player name doesn't have to be unique but the color is asigned random and unique)
    //Sets player name, player score on scoreboard and disables ui 
    public void InitializePlayer(Player player)
    {
        Color randomPlayerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        while (_usedColors.Contains(randomPlayerColor)) randomPlayerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        if (isServer) _usedColors.Add(randomPlayerColor);
        else CmdAddColor(randomPlayerColor);

        _localPlayerScript = player;
        _localPlayerScript.Initialize();
        _localPlayerScript.SetPlayerColor(randomPlayerColor);
        _localPlayerScript.SetName(_networkManagerHUD.playerName);

        _scoreBoard.AddPlayer(_localPlayerScript);

        _cardGameManager.SetPlayer(_localPlayerScript);
        _cardGameManager.GetUI().DisableInteractions(false);
        _cardGameManager.SetSeconds(0);

        CmdSetTurn();
    }

    [Command(requiresAuthority = false)] public void CmdAddColor(Color color) => _usedColors.Add(color);

    [Command(requiresAuthority = false)]
    public void CmdSetTurn()
    {
        for (int i = 0; i < _playerSripts.Count; i++)
        {
            NetworkIdentity opponentIdentity = _playerSripts[i].gameObject.GetComponent<NetworkIdentity>();
            if (_order == i) TargetRPCSetTurn(opponentIdentity.connectionToClient, true);
            else TargetRPCSetTurn(opponentIdentity.connectionToClient, false);
        }
    }

    #endregion SettingUpPlayer

    #region SwitchPlayer

    private void SwitchPlayer(object sender, System.EventArgs e)
    {
        _cardGameManager.GetUI().DisableInteractions(false);
        if (_localPlayerScript != null)
        {
            if (_localPlayerScript.GetTurns() == 0) CmdSwitchTurn(true, true);
            else CmdSwitchTurn(true, false);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSwitchTurn(bool b, bool bb)
    {
        _syncTurns0 = bb;
        SwitchTurn(b);
    }

    [Server]
    public void SwitchTurn(bool b)
    {
        ClientRPCCountAllCells();

        if (_syncLastPlayer == true && _syncTurns0 == true)
        {
            StartCoroutine(WaitSync());
            return;
        }
        else if (_order < _playerSripts.Count - 1) _order++;
        else
        {
            _order = 0;
            _syncGameStarted = true;
        }

        LastPlayer();
        NetworkIdentity opponentIdentity = _playerSripts[_order].gameObject.GetComponent<NetworkIdentity>();
        TargetRPCSetTurn(opponentIdentity.connectionToClient, b);
    }

    //After player finishes turn it counts everyones cells and updates the information
    [ClientRpc]
    public void ClientRPCCountAllCells()
    {
        if (_localPlayerScript != null) _cardGameManager.CountPlayerCells();
    }

    //Waits for 0.2 sceconds for scoreBoard to be synced 
    IEnumerator WaitSync()
    {
        yield return new WaitForSeconds(0.2f);
        if (isServerOnly) _onlineGameOver.GameOver(_scoreBoard.playerStats, _scoreBoard.playerNames);
        GameOverScore();
    }

    //Checks if the current player is the last player in order. Useful for determening when the game of life should start and game over to be called
    [Server]
    public void LastPlayer()
    {
        if (_order == _playerSripts.Count - 1) _syncLastPlayer = true;
        else _syncLastPlayer = false;
    }

    [TargetRpc]
    public void TargetRPCSetTurn(NetworkConnection target, bool b)
    {
        if (_undoButton.interactable && b) _cardGameManager.GetUI().ButtonInteractions(b);
        else _cardGameManager.GetUI().DisableInteractions(b);
        if (b) CmdcurrentlyPlayingText(_localPlayerScript.GetName(), _localPlayerScript.GetTurns(), _localPlayerScript.GetPlayerColor());
    }

    //Information to everyone(server and clients) about who the current player is and what turn it is
    [Command(requiresAuthority = false)]
    public void CmdcurrentlyPlayingText(string name, int turn, Color color)
    {
        ClientRPCcurrentlyPlayingText(name, turn, color);
        _currentlyPlaying.text = "Current player: " + name + "\nTrun :" + turn;
        _currentlyPlaying.color = color;
    }

    [ClientRpc]
    public void ClientRPCcurrentlyPlayingText(string name, int turn, Color color)
    {
        _currentlyPlaying.text = "Current player: " + name + "\nTrun :" + turn;
        _currentlyPlaying.color = color;
    }

    #endregion SwitchPlayer

    private void StartGameofLife(object sender, System.EventArgs e)
    {
        _gameOfLifeManager.SetCellsArray();
        if (_syncLastPlayer)
        {
            _cardGameManager.SetSeconds(2);
            _gameOfLifeManager.SetIsPaused(false);
        }
        else
        {
            _cardGameManager.SetSeconds(0);
            _gameOfLifeManager.SetIsPaused(true);
        }
    }

    [Server]
    public void RemoveCells(Color color) //Removes cells of a client that disconnected
    {
        Cell[] cells = _gameOfLifeManager.GetCellsArray();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].GetCellSpriteRenderer().color == color) cells[i].SetIsAlive(false);
        }
    }

    #region GameOver&NewGame
    [ClientRpc] public void GameOverScore() => _onlineGameOver.GameOver(_scoreBoard.playerStats, _scoreBoard.playerNames);

    [Command(requiresAuthority = false)]
    public void OnClickNewGame()
    {
        _syncTurns0 = false;
        _order = 0;
        _syncGameStarted = false;
        if (isServerOnly)
        {
            _scoreBoard.NewGame();
            _onlineGameOver.gameObject.SetActive(false);
        }
        NewGame();
        LastPlayer();
        for (int i = 0; i < _playerSripts.Count; i++)
        {
            NetworkIdentity opponentIdentity = _playerSripts[i].gameObject.GetComponent<NetworkIdentity>();
            if (_order == i) TargetRPCSetTurn(opponentIdentity.connectionToClient, true);
            else TargetRPCSetTurn(opponentIdentity.connectionToClient, false);
        }
    }

    [ClientRpc]
    public void NewGame()
    {
        _scoreBoard.NewGame();
        if (_localPlayerScript != null) _cardGameManager.NewGame();
        _onlineGameOver.gameObject.SetActive(false);
    }


    #endregion GameOver&NewGame

    #region GettersAndSetters
    public void SetOrder(int order) => _order = order;
    public int GetOrder() => _order;
    public void SetGameStarted(bool b) => _syncGameStarted = b;
    public bool GetGameStarted() => _syncGameStarted;
    public List<Player> GetPlayerSripts() => _playerSripts;
    public ScoreBoard GetScoreBoard() => _scoreBoard;
    public NetworkManager GetNetworkManager() => _networkManager;
    public NetworkManagerHUD GetNetworkManagerHUD() => _networkManagerHUD;

    #endregion GettersAndSetters
}
