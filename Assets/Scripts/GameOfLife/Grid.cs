using UnityEngine;
using Mirror;

//Dynamic Grid. Has options to resize and center the grid.

public class Grid : MonoBehaviour
{
    [SerializeField] private GameOfLifeManager _gameOfLifeManager;
    private Vector2 _gridSize = new Vector2(20, 20);
    private PlaceCells _placeCells = new();

    private void Awake() => Initialize();

    public void Initialize()
    {
        _gameOfLifeManager.Initialize(_gridSize);
        CenterGrid();
        _placeCells.PlaceRandomCells(_gridSize, _gameOfLifeManager.GetCellsArray(), gameObject.transform);
        _gameOfLifeManager.SetCellsArray();
        _gameOfLifeManager.SetIsPaused(false);
    }

    public Vector2 GetGridSize() => _gridSize;
    public void SetGridSize(Vector2 gridSize)
    {
        _gridSize = gridSize;
        CenterGrid();
    }

    protected void CenterGrid()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.orthographicSize = 10;
        int camerax = (int)Camera.main.transform.position.x + 20;
        int cameray = (int)Camera.main.transform.position.y + (int)Camera.main.orthographicSize;

        while (_gridSize.y > cameray)
        {
            Camera.main.orthographicSize += 15;
            cameray += (int)Camera.main.orthographicSize;
        }
        while (_gridSize.x > camerax)
        {
            camerax += 50;
            Camera.main.orthographicSize += 10;
        }
        Camera.main.transform.position = new Vector3((_gridSize.x / 2) + 5, _gridSize.y / 2, -10);
    }
}
