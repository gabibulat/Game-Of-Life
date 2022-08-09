using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UICardContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] _cardSpaces;
    [SerializeField] private UIShape[] _shape;
    [SerializeField] private TMP_Text[] _type;
    [SerializeField] private TMP_Text[] _cost;
    private Color _activePlayerColor;

    public void DropedCard(GameObject gameObject)
    {
        gameObject.transform.parent.gameObject.SetActive(!gameObject.activeInHierarchy);
        for (int i = 0; i < _shape.Length; i++) _shape[i].SetShapeColor(_activePlayerColor);
    }

    public void SetCards(List<Card> currentCards, Color playerColor)
    {
        _activePlayerColor = playerColor;
        for (int i = 0; i < _cardSpaces.Length; i++)
        {
            _cardSpaces[i].SetActive(false);
        }
        for (int i = 0; i < currentCards.Count; i++)
        {
            _cardSpaces[i].SetActive(true);
            _shape[i].SetCard(currentCards[i]);
            _shape[i].SetShapeColor(_activePlayerColor);
            _cost[i].text = "Cost: " + (int)currentCards[i].GetTypeOfCard();
            _type[i].text = currentCards[i].GetTypeOfCard().ToString();
            _shape[i].SetCard(currentCards[i]);
        }
    }

    public void ActionCheck(int actions)
    {
        for (int i = 0; i < _shape.Length; i++)
        {
            if ((int)_shape[i].GetCard().GetTypeOfCard() > actions)
            {
                _shape[i].GetShapeImage().raycastTarget = false;
                _cost[i].color = Color.red;
            }
            else
            {
                _shape[i].GetShapeImage().raycastTarget = true;
                _cost[i].color = Color.white;
            }
        }
    }

    public void AddNewCard(Card card)
    {
        for (int i = 0; i < _cardSpaces.Length; i++)
        {
            if (_cardSpaces[i].activeInHierarchy == false)
            {
                _shape[i].SetCard(card);
                _shape[i].SetShapeColor(_activePlayerColor);
                _cost[i].text = "Cost: " + (int)card.GetTypeOfCard();
                _type[i].text = card.GetTypeOfCard().ToString();
                _cardSpaces[i].SetActive(true);

                break;
            }
        }
    }

    public void CardRaycastTarget(bool b)
    {
        for (int i = 0; i < _shape.Length; i++) _shape[i].GetShapeImage().raycastTarget = b;
    }

    public void DisableCards()
    {
        for (int i = 0; i < _cardSpaces.Length; i++) _cardSpaces[i].SetActive(false);
    }

    public bool RoomForCardCheck()
    {
        for (int i = 0; i < _cardSpaces.Length; i++)
        {
            if (!_cardSpaces[i].activeInHierarchy) return true;
        }
        return false;
    }
}
