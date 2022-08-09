using UnityEngine;

//Places random alive/dead cells on grid
public class PlaceCells
{
    private Cell[] _cells;
    private bool _random = false;

    public void PlaceRandomCells(Vector2 _gridSize, Cell[] cells, Transform gameoflife)
    {
        int i = 0;
        _cells = cells;

        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Cell cell = MonoBehaviour.Instantiate(Resources.Load("Prefabs/CellBase", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                cell.transform.SetParent(gameoflife);

                cell.SetCellPosition(new Vector2(x, y));
                _cells[i] = cell;

                System.Random rnd = new System.Random();               
                int rand = rnd.Next(0, 101);
                if (rand < 75) _random = false;
                else _random = true;
                _cells[i].SetIsAlive(_random);
                i++;
            }
        }
    }
}
