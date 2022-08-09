using UnityEngine;

public class UIManager1 : MonoBehaviour
{
    [SerializeField] private GameOfLifeManager _gameOfLifeManager;
    private bool[] _initialState;
    private Cell[] _cells;
    [SerializeField] private Grid _grid;
    [SerializeField] private TMPro.TMP_InputField x;
    [SerializeField] private TMPro.TMP_InputField y;

    private void Start() => Initialize();

    private void Initialize()
    {
        _cells = _gameOfLifeManager.GetCellsArray();
        if (_initialState != null) System.Array.Clear(_initialState, 0, _initialState.Length);
        _initialState = new bool[_cells.Length];
        for (int i = 0; i < _cells.Length; i++) _initialState[i] = _cells[i].GetIsAlive();
    }

    public void Restart()
    {
        for (int i = 0; i < _initialState.Length; i++)
        {
            _cells[i].SetIsAI(false);
            _cells[i].SetIsAlive(_initialState[i]);
        }
        _gameOfLifeManager.SetCellsArray();
    }

    public void Clear()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].SetIsAI(false);
            _cells[i].GetCellSpriteRenderer().color = Color.black;
            _cells[i].SetIsAlive(false);
        }
        _gameOfLifeManager.SetCellsArray();
    }

    public void Pause() => _gameOfLifeManager.SetIsPaused(!_gameOfLifeManager.GetIsPaused());

    public void ChangeGridSize()
    {
        if (_grid.GetGridSize() != new Vector2(System.Int32.Parse(y.text), System.Int32.Parse(x.text)))
        {
            for (int i = 0; i < _cells.Length; i++) Destroy(_cells[i].gameObject);
            _grid.SetGridSize(new Vector2(System.Int32.Parse(y.text), System.Int32.Parse(x.text)));
            _grid.Initialize();
            Initialize();
        }
    }
}
