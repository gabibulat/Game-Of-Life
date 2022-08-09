using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCard
{
    StillLife = 1,
    Oscillator = 2,
    Spaceship = 3
}

[System.Serializable]
public class Card
{
    [SerializeField] private List<Vector2> _cellPositions;
    [SerializeField] private Sprite _shapeSprite;
    [SerializeField] private TypeOfCard _type;

    public List<Vector2> GetCellPositions() => _cellPositions;
    public Sprite GetShapeSprite() => _shapeSprite;
    public TypeOfCard GetTypeOfCard() => _type;
}
