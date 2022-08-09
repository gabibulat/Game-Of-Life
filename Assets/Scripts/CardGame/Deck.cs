using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Card> _deckOfCards;
    public List<Card> GetDeckOfCards() => _deckOfCards;
}
