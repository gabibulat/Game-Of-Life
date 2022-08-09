using UnityEngine;

public class CellCounter
{
    private int _counter = 0;
    private Color _cellColor;
    
    public void CountCells(Cell[] _cells)
    {
        _counter = 0;
        for (int i = 0; i < _cells.Length; i++) if (_cells[i].GetIsAlive() && _cells[i].GetCellSpriteRenderer().color == _cellColor) _counter++;
    }
    public int GetCounter() => _counter;
    public void SetCounter(int counter) => _counter = counter;
    public void SetCellColor(Color color) => _cellColor = color;
}
