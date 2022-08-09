using UnityEngine;

//Used for running/stopping game of life simulation.

public class GameOfLifeManager : MonoBehaviour
{
    [SerializeField] private Cell[] _cells;
    [SerializeField] private CellBehaviour _cellBehaviour;
    private bool _isPaused = true;

    private void Awake()
    {
        if (_cells.Length != 0) SetCellsArray();
    }

    public void Initialize(Vector2 _gridSize)
    {
        if (_cells != null) System.Array.Clear(_cells, 0, _cells.Length);
        _cells = new Cell[(int)_gridSize.x * (int)_gridSize.y];
    }

    //If not paused, continously checks cells for game of life rules 
    private void Update()
    {
        if (!_isPaused) _cellBehaviour.CheckState();
    }

    #region GettersAndSetters

    public void SetCellsArray() => _cellBehaviour.SetCellsArray(_cells);
    public Cell[] GetCellsArray() => _cells;
    public bool GetIsPaused() => _isPaused;
    public void SetIsPaused(bool paused) => _isPaused = paused;
    
    #endregion GettersAndSetters
}
