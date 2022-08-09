using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Predefined shape/card that can be dragged and placed on grid

public class UIShape : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _shapeImage;
    [SerializeField] private Card _card;
    [SerializeField] private bool _isAI = false;

    private void Awake() => SetImage();
    
    public void SetCard(Card card)
    {
        _card = card;
        SetImage();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        _shapeImage.maskable = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _shapeImage.maskable = true;
        transform.localPosition = new Vector3(0, 0, 0);
    }

    public Card GetCard() => _card;
    void SetImage() => gameObject.GetComponent<Image>().sprite = _card.GetShapeSprite();
    public List<Vector2> GetCellPositions() => _card.GetCellPositions();
    public Image GetShapeImage() => _shapeImage;
    public bool GetIsAI() => _isAI;
    public void SetShapeColor(Color color) => _shapeImage.color = color;
    public void SetTransparent(int i) => _shapeImage.color = new Color(_shapeImage.color.r, _shapeImage.color.g, _shapeImage.color.b, i);
}
