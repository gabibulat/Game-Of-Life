using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameObjectPlayer;
    [SerializeField] private GameOfLifeManager _gameOfLifeManager;
    [SerializeField] private Player _player;
    [SerializeField] private UICardContainer _uiCardContainer;
    [SerializeField] private DropCard _dropCard;
    [SerializeField] private UIGameOver _uIGameOverWindow;
    [SerializeField] private UIManager2 _ui;
    private float _seconds = 2;
    private int _actions = 4;

    public event EventHandler onFinishedAction;
    public event EventHandler onStartAction;

    public void Start()
    {
        if (_player != null)
        {
            _player.onPlayerEnable += onPlayerEnabled;
            _gameObjectPlayer.SetActive(true);
        }
        _dropCard.onDropOrUndoCard += onDropedOrUndoCard;
        _gameOfLifeManager.SetIsPaused(true);
    }

    public void onPlayerEnabled(object sender, Player player) => SetPlayer(player);

    public void SetPlayer(Player player)
    {
        _player = player;
        _ui.UpdateTurnsText(_player.GetTurns());
        _actions = 4;
        _ui.UpdateActionsText(_actions);
        _uiCardContainer.SetCards(_player.GetCurrentCards(), _player.GetPlayerColor());

        Cell[] cells = _gameOfLifeManager.GetCellsArray();
        for (int i = 0; i < cells.Length; i++)
        {
            CountPlayerCells();
            if (cells[i].GetAliveColor() == new Color(0, 0, 0, 1)) cells[i].SetAliveColor(_player.GetPlayerColor());
        }
    }

    public void CountPlayerCells()
    {
        _player.GetCellCounter().CountCells(_gameOfLifeManager.GetCellsArray());
        _ui.UpdateCellCountText(_player.GetCellCounter().GetCounter());
        _player.SetScore(_player.GetCellCounter().GetCounter());
    }

    protected void onDropedOrUndoCard(object sender, Tuple<GameObject, int> t)
    {
        if (t.Item2 < 0) _ui.DisableUndoInteractions(true);
        else if (t.Item2 == 0) _ui.DisableUndoInteractions(false);
        if (t.Item1 != null)
        {
            Card card = t.Item1.GetComponent<UIShape>().GetCard();
            if (t.Item2 == 1) _player.GetCurrentCards().Add(card);
            else _player.DroppedCard(card);
            _actions += (int)card.GetTypeOfCard() * t.Item2;
            _uiCardContainer.DropedCard(t.Item1);
            _ui.UpdateActionsText(_actions);
        }
    }
    #region ButtonFunctions

    //On draw card/cards undo is cleared and disabled interaction
    public void DrawCard(Button btn)
    {
        if (_uiCardContainer.RoomForCardCheck())
        {
            int numCards = 1;
            if (_player.GetTurns() == 6) numCards = 3;
            for (int i = 0; i < numCards; i++) PickACard();
            btn.interactable = false;
            _dropCard.ClearUndo();
            _ui.DisableUndoInteractions(false);
        }
    }

    private void PickACard()
    {
        int i = UnityEngine.Random.Range(0, _player.GetDeck().Count);
        _uiCardContainer.AddNewCard(_player.GetDeck()[i]);
        _player.DrawCard(i);
        _ui.UpdateActionsText(_actions);
    }

    virtual public void StartGameOfLife()
    {
        _player.SetTurns(_player.GetTurns() - 1);
        _ui.UpdateTurnsText(_player.GetTurns());
        _ui.DisableInteractions(false);
        _gameOfLifeManager.SetIsPaused(false);
        onStartAction?.Invoke(this, EventArgs.Empty);
        _dropCard.ClearUndo();
        _actions = 4;
        StartCoroutine(Ticker());
    }

    IEnumerator Ticker()
    {
        yield return new WaitForSeconds(_seconds);
        _ui.UpdateActionsText(_actions);
        _gameOfLifeManager.SetIsPaused(true);
        CountPlayerCells();

        if (_uIGameOverWindow != null)
        {
            if (_player.GetTurns() != 0) _ui.DisableInteractions(true);
            else _uIGameOverWindow.GameOver(_player.GetCellCounter().GetCounter());
        }
        onFinishedAction?.Invoke(this, EventArgs.Empty);
    }

    virtual public void NewGame()
    {
        _player.Initialize();
        _actions = 4;
        _ui.NewGame(_player.GetTurns(), _actions);
        Cell[] _cells = _gameOfLifeManager.GetCellsArray();
        for (int i = 0; i < _cells.Length; i++) _cells[i].SetIsAlive(false);
    }

    #endregion ButtonFunctions

    #region GettersAndSetters

    public GameOfLifeManager GetGameOfLifeManager() => _gameOfLifeManager;
    public void SetSeconds(float sec) => _seconds = sec;
    public UIManager2 GetUI() => _ui;
    public void SetActions(int actions) => _actions = actions;

    #endregion GettersAndSetters

}