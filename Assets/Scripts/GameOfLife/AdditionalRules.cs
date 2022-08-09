using UnityEngine;

//Place to add additional cell behaviour rules.
//Currently added rule thar if cell has more alive enemy cells than its own
//turns color into enemy color 
public class AdditionalRules : CellBehaviour
{
    protected override void NeighboursCount()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            int countAlive = 0;
            int countEnemy = 0;
            int countFriendly = 0;
            Color enemyColor= Color.white;
            Cell _neighbourCell;
            colliderArray = Physics2D.OverlapBoxAll(_cells[i].GetCellPosition(), new Vector2(1, 1), 0f);
            for (int j = 0; j < colliderArray.Length; j++)
            {
                _neighbourCell = colliderArray[j].GetComponent<Cell>(); //svaka susjedna æelija
                if (_cells[i] != _neighbourCell && _neighbourCell.GetIsAlive())
                {
                    countAlive++;
                    if (_cells[i].GetAliveColor() != _neighbourCell.GetAliveColor())
                    {
                        countEnemy++;
                        enemyColor = _neighbourCell.GetAliveColor();
                    }
                    else countFriendly++;
                }
            }
            _cells[i].SetAliveNeighbours(countAlive);
            if (countEnemy> countFriendly) _cells[i].SetAliveColor(enemyColor);
        }
    }
}
