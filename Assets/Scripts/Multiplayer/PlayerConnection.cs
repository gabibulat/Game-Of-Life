using UnityEngine;
using Mirror;

//Script is attached to player GameObject. Has control over when clients enter/exit online game

public class PlayerConnection : NetworkBehaviour
{
    private PlayerManagerOnlineMultiplayer _playerManager;
    private Player _player;
    [SyncVar] private Color _playerColor;
    
    private void Awake() => _playerManager = FindObjectOfType<PlayerManagerOnlineMultiplayer>();

    public override void OnStartServer()
    {
        _player = gameObject.GetComponent<Player>();
        _playerManager.GetPlayerSripts().Add(_player);
        if (_playerManager.GetPlayerSripts().Count == 1) _playerManager.SetGameStarted(false);
        _playerManager.LastPlayer();
    }

    public override void OnStopServer()
    {
        _playerManager.GetScoreBoard().Remove(_playerColor);
        _playerManager.RemoveCells(_playerColor);
        if (_playerManager.GetOrder() == _playerManager.GetPlayerSripts().IndexOf(_player)) _playerManager.SwitchTurn(true);
        _playerManager.GetPlayerSripts().Remove(_player);
        _playerManager.LastPlayer();
        if (_playerManager.GetPlayerSripts().Count == 0)
        {
            _playerManager.SetGameStarted(false);
            _playerManager.SetOrder(0);
        }
    }

    public override void OnStartLocalPlayer()
    {
        _player = gameObject.GetComponent<Player>();
        _playerManager.InitializePlayer(_player);
        CmdColorChange(_player.GetPlayerColor());
        if (_playerManager.GetGameStarted())
        {
            _playerManager.GetNetworkManagerHUD().message = "This game has already started";
            _playerManager.GetNetworkManager().StopClient();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdColorChange(Color color) => _playerColor = color;

}
