using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropShapeWithAI : DropShape
{
    [SerializeField] private Image _AIcontainer;
    [SerializeField] private AIGlider _aiGlider;
    [SerializeField] private Grid _grid;

    public override void OnDrop(PointerEventData eventData)
    {
        if (_uiShape.GetIsAI())
        {
            if (_highlightedCells[0].GetCellSpriteRenderer().color == Color.green)
            {
                for (int i = 0; i < _highlightedCells.Count; i++)
                {
                    _highlightedCells[i].SetIsHovering(false);
                    _highlightedCells[i].SetIsAI(true);
                }

                _gameOfLifeManager.SetCellsArray();
                _AIcontainer.gameObject.SetActive(false);
                _aiGlider.SetCells(_cells);
                _aiGlider.gameObject.SetActive(true);

            }
            _uiShape.GetShapeImage().maskable = true;
            _uiShape.transform.localPosition = new Vector3(0, 0, 0);
        }
        else base.OnDrop(eventData);

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _cells = _gameOfLifeManager.GetCellsArray();
        if (eventData.pointerDrag != null)
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
                            //drop borders for AI
                            if (_uiShape.GetIsAI() && (
                                _cellPositions[i].x + x <= 1 ||
                                _cellPositions[i].x + x >= _grid.GetGridSize().x - 2 ||
                                _cellPositions[i].y + y >= _grid.GetGridSize().y - 2 ||
                                _cellPositions[i].y + y <= 1)) m--;
                            else
                            {
                                _cells[j].SetIsHovering(true);
                                _cells[j].GetCellSpriteRenderer().color = Color.red;
                                _highlightedCells.Add(_cells[j]);
                                m++;
                            }
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
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (_uiShape != null)
        {
            if (_uiShape.GetIsAI()) _uiShape.SetShapeColor(_cells[0].GetAIcolor());
        }

    }
}
