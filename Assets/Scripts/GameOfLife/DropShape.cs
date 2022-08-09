using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropShape : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected GameOfLifeManager _gameOfLifeManager;
    protected Cell[] _cells;
    protected UIShape _uiShape;
    protected List<Cell> _highlightedCells = new();

    //Green highlighted cells become alive cells on drop
    virtual public void OnDrop(PointerEventData eventData)
    {
        if (_uiShape != null)
        {
            if (_highlightedCells[0].GetCellSpriteRenderer().color == Color.green)
            {
                for (int i = 0; i < _highlightedCells.Count; i++)
                {
                    _highlightedCells[i].SetIsHovering(false);
                    _uiShape.SetTransparent(1);
                    _highlightedCells[i].SetAliveColor(_uiShape.GetShapeImage().color);
                    _highlightedCells[i].SetCurrentColor(_uiShape.GetShapeImage().color);
                    _highlightedCells[i].SetIsAlive(true);

                }
                _gameOfLifeManager.SetCellsArray();
            }
         

            _uiShape.GetShapeImage().maskable = true;
            _uiShape.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

   //Removes ui Image from following cursor up on entering grid and highlights green or red if the shape can be placed there
   virtual public void OnPointerEnter(PointerEventData eventData)
    {
        _cells = _gameOfLifeManager.GetCellsArray();
        if (eventData.pointerDrag != null )
        {
            _highlightedCells.Clear();
            _uiShape = eventData.pointerDrag.GetComponent<UIShape>();

            if (_uiShape != null)
            {
                List<Vector2> _cellPositions = _uiShape.GetCellPositions();
                _uiShape.SetTransparent(0);
                Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int x = Mathf.RoundToInt(mousePoint.x);
                int y = Mathf.RoundToInt(mousePoint.y);
                int m = 0;

                for (int i = 0; i < _cellPositions.Count; i++)
                {
                    for (int j = 0; j < _cells.Length; j++)
                    {

                        if (Mathf.RoundToInt(_cells[j].GetCellPosition().x) == _cellPositions[i].x + x && Mathf.RoundToInt(_cells[j].GetCellPosition().y) == _cellPositions[i].y + y)
                        {
                            _cells[j].SetIsHovering(true);
                            _cells[j].GetCellSpriteRenderer().color = Color.red;
                            _highlightedCells.Add(_cells[j]);

                            m++;
                            break;
                        }
                    }
                }

                if (m >= _cellPositions.Count)
                {
                    for (int i = 0; i < _highlightedCells.Count; i++)
                    {
                        _highlightedCells[i].SetIsHovering(true);
                        _highlightedCells[i].GetCellSpriteRenderer().color = Color.green;
                    }

                }
            }
        }
    }

    //Restores ui Image to follow cursor again and removes cell highlighting 
    virtual public void OnPointerExit(PointerEventData eventData)
    {
        if (_uiShape != null )
        {
            _uiShape.SetTransparent(1);

            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i].SetIsHovering(false);
                if (_cells[i].GetIsAlive()) _cells[i].GetCellSpriteRenderer().color = _cells[i].GetCurrentColor();
                else if (!_cells[i].GetIsAlive()) _cells[i].GetCellSpriteRenderer().color = _cells[i].GetDeadColor();
            }

        }
    }
}
