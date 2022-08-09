using UnityEngine;

public class CellBehaviourWithAI : CellBehaviour
{
    public override void CheckState()
    {
        NeighboursCount();
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i].GetIsAlive() && _cells[i].GetAliveNeighbours() != 2 && _cells[i].GetAliveNeighbours() != 3) _aliveState[i] = false;
            else if (!_cells[i].GetIsAlive() && _cells[i].GetAliveNeighbours() == 3) _aliveState[i] = true;
            if (_cells[i].GetAIneighbours() == 3) _cells[i].SetIsAI(true);
            else if (_cells[i].GetAIneighbours() != 2 && _cells[i].GetAIneighbours() != 3) _cells[i].SetIsAI(false);
        }

        for (int i = 0; i < _cells.Length; i++) _cells[i].SetIsAlive(_aliveState[i]);
    }

    protected override void NeighboursCount()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            int coutAIneighbours = 0;
            int countAlive = 0;
            colliderArray = Physics2D.OverlapBoxAll(_cells[i].GetCellPosition(), new Vector2(1, 1), 0f);
            for (int j = 0; j < colliderArray.Length; j++)
            {
                if (colliderArray[j].tag == "Cell")
                {
                    Cell _neighbourCell = colliderArray[j].GetComponent<Cell>();
                    if (_cells[i] != _neighbourCell && _neighbourCell.GetIsAlive()) countAlive++;
                    if (_cells[i] != _neighbourCell && _neighbourCell.GetIsAI()) coutAIneighbours++;
                }
                _cells[i].SetAIneighbours(coutAIneighbours);
                _cells[i].SetAliveNeighbours(countAlive);
            }
        }
    }
}
