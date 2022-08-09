using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropCard : DropShape
{
    private List<List<Cell>> _undoPlacedCells = new();
    private List<List<bool>> _undoAliveState = new();
    private List<List<Color>> _undoCurrentColor = new();
    private List<GameObject> _undoCards = new();
    public event EventHandler<Tuple<GameObject, int>> onDropOrUndoCard;

    public override void OnDrop(PointerEventData eventData)
    {
        if (_uiShape != null && _highlightedCells[0].GetCellSpriteRenderer().color == Color.green)
        {
            List<bool> holder = new();
            List<Color> holder3 = new();
            List<Cell> holder2 = new();
            for (int i = 0; i < _highlightedCells.Count; i++)
            {
                holder.Add(_highlightedCells[i].GetIsAlive());
                holder2.Add(_highlightedCells[i]);
                holder3.Add(_highlightedCells[i].GetCurrentColor());
            }
            _undoCurrentColor.Add(holder3);
            _undoAliveState.Add(holder);
            _undoPlacedCells.Add(holder2);
            _undoCards.Add(eventData.pointerDrag);
            _gameOfLifeManager.SetCellsArray();
            base.OnDrop(eventData);
            onDropOrUndoCard?.Invoke(this, new Tuple<GameObject, int>(eventData.pointerDrag, -1));
        }
    }

    public void UndoAction()
    {
        for (int i = 0; i < _undoPlacedCells[_undoPlacedCells.Count - 1].Count; i++)
        {
            _undoPlacedCells[_undoPlacedCells.Count - 1][i].SetCurrentColor(_undoCurrentColor[_undoCurrentColor.Count - 1][i]);
            _undoPlacedCells[_undoPlacedCells.Count - 1][i].SetAliveColor(_undoCurrentColor[_undoCurrentColor.Count - 1][i]);
            _undoPlacedCells[_undoPlacedCells.Count - 1][i].SetIsAlive(_undoAliveState[_undoPlacedCells.Count - 1][i]);
        }
        onDropOrUndoCard?.Invoke(this, new Tuple<GameObject, int>(_undoCards[_undoCards.Count - 1], 1));
        _undoCurrentColor.RemoveAt(_undoCurrentColor.Count - 1);
        _undoPlacedCells.RemoveAt(_undoPlacedCells.Count - 1);
        _undoAliveState.RemoveAt(_undoAliveState.Count - 1);
        _undoCards.RemoveAt(_undoCards.Count - 1);
        _gameOfLifeManager.SetCellsArray();
        if (_undoPlacedCells.Count == 0) onDropOrUndoCard?.Invoke(this, new Tuple<GameObject, int>(null, 0));
    }

    public void ClearUndo()
    {
        _undoPlacedCells.Clear();
        _undoAliveState.Clear();
        _undoCards.Clear();
    }
}
