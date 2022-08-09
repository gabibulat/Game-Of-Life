using UnityEngine;
using Mirror;
using System;

public class Cell : MonoBehaviour
{
    [SerializeField] private Vector2 _cellPosition;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _aliveColor = Color.white;
    [SerializeField] private Color _deadColor = Color.black;
    private Color _currentColor;
    private Color _AIcolor = new Color(1f, 0.5f, 0f, 1f);
    private bool _isHovering = false, _isAI = false;
    private int _AIneighbours;

    //public because of SyncCell script; when setting it in [ClientRPC] not to fire off events again
    public int _aliveNeighbours;
    public bool _isAlive = false;

    public event EventHandler<bool> onChangedAlive;
    public event EventHandler<int> onChangedNeighbours;

    public void SetIsAlive(bool alive)
    {

        _isAlive = alive;
        if (_isHovering == false)
        {
            if (_isAI) _spriteRenderer.color = _AIcolor;
            else if (_isAlive)
            {
                _spriteRenderer.color = _aliveColor;
                _currentColor = _aliveColor;
            }
            else _spriteRenderer.color = _deadColor;
        }
        onChangedAlive?.Invoke(this, alive);

    }

    public void SetAliveNeighbours(int count)
    {

        _aliveNeighbours = count;
        onChangedNeighbours?.Invoke(this, count);
    }

    #region BasicGettersAndSetters

    public Vector2 GetCellPosition() => _cellPosition;
    public void SetCellPosition(Vector2 position) => _cellPosition = position;
    public int GetAliveNeighbours() => _aliveNeighbours;
    public void SetIsAI(bool b) => _isAI = b;
    public bool GetIsAI() => _isAI;
    public Color GetAIcolor() => _AIcolor;
    public SpriteRenderer GetCellSpriteRenderer() => _spriteRenderer;
    public bool SetIsHovering(bool hovering) => _isHovering = hovering;
    public int GetAIneighbours() => _AIneighbours;
    public void SetAIneighbours(int count) => _AIneighbours = count;
    public Color GetAliveColor() => _aliveColor;
    public Color GetDeadColor() => _deadColor;
    public void SetCurrentColor(Color color) => _currentColor = color;
    public void SetAliveColor(Color color) => _aliveColor = color;
    public void SetDeadColor(Color color) => _deadColor = color;
    public bool GetIsAlive() => _isAlive;
    public Color GetCurrentColor() => _currentColor;

    #endregion BasicGettersAndSetters
}
