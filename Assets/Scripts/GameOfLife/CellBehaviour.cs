using UnityEngine;

//Calculates alive neighbours then checks for game of life rules
//if next gen cells should be alive or dead and sets them.

public class CellBehaviour : MonoBehaviour
{
    [SerializeField] protected Cell[] _cells;
    [SerializeField] protected bool[] _aliveState;
    protected Collider2D[] colliderArray;

    public void SetCellsArray(Cell[] cells)
    {
        if (_cells == null || _cells.Length != cells.Length)
        {
            if (_cells != null) System.Array.Clear(_aliveState, 0, _aliveState.Length);
            _cells = cells;
            _aliveState = new bool[_cells.Length];
        }
        for (int i = 0; i < _cells.Length; i++) _aliveState[i] = _cells[i].GetIsAlive();
        NeighboursCount();
    }

    protected virtual void NeighboursCount()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            int countAlive = 0;
            colliderArray = Physics2D.OverlapBoxAll(_cells[i].GetCellPosition(), new Vector2(1, 1), 0f);
            for (int j = 0; j < colliderArray.Length; j++)
            {
                if (colliderArray[j].tag == "Cell")
                {
                    Cell _neighbourCell = colliderArray[j].GetComponent<Cell>();
                    if (_cells[i] != _neighbourCell && _neighbourCell.GetIsAlive()) countAlive++;
                }
            }
            _cells[i].SetAliveNeighbours(countAlive);
        }
    }

    public virtual void CheckState()
    {
        for (int i = 0; i < _cells.Length; i++) _aliveState[i] = _cells[i].GetIsAlive();

        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i].GetIsAlive() && _cells[i].GetAliveNeighbours() != 2 && _cells[i].GetAliveNeighbours() != 3) _aliveState[i] = false;
            else if (!_cells[i].GetIsAlive() && _cells[i].GetAliveNeighbours() == 3) _aliveState[i] = true;
        }
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i].GetIsAlive() != _aliveState[i]) _cells[i].SetIsAlive(_aliveState[i]);
        }
        NeighboursCount();
    }
}
