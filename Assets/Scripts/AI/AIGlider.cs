using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AIGlider : MonoBehaviour
{
    [SerializeField] private List<Cell> _AIcells;
    [SerializeField] private Grid _grid;
    [SerializeField] private TMP_Text _positionText;
    private Cell[] _cells;
    private int _wait = 0, _currentangle = 0, _numberOfRotation = 0, rand=0;
    private Vector2 _centerPoint, _gridsize;

    private void OnEnable()
    {
        _currentangle = 0;
        _positionText.gameObject.SetActive(true);
        _gridsize = _grid.GetGridSize();
    }

    public void SetCells(Cell[] cells) => _cells = cells;

    private void Update()
    {
        _AIcells.Clear();
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i].GetIsAI()) _AIcells.Add(_cells[i]); //Continously clearing and filling list as the AI glider is moving and switching cells
        }  
        if (_wait == 0) CheckNeedForRotation();
        else _wait--; 
    }

    private void CheckNeedForRotation()
    {
        //finding center cell to RotateAI around it
        float maxX = 0, maxY = 0, minX = _gridsize.x, minY = _gridsize.y;
        for (int i = 0; i < _AIcells.Count; i++)
        {
            if (_AIcells[i].GetCellPosition().x > maxX) maxX = _AIcells[i].GetCellPosition().x;
            if (_AIcells[i].GetCellPosition().y > maxY) maxY = _AIcells[i].GetCellPosition().y;
            if (_AIcells[i].GetCellPosition().x < minX) minX = _AIcells[i].GetCellPosition().x;
            if (_AIcells[i].GetCellPosition().y < minY) minY = _AIcells[i].GetCellPosition().y;
        }
        _centerPoint = new Vector2(maxX - 1, maxY - 1);

        _positionText.text = "AI position" + "\nX: " + _centerPoint.x + "\nY:"+ _centerPoint.y;

        System.Random rnd = new System.Random();

        if (_numberOfRotation == 0) rand = rnd.Next(1,3);

        //Rotations random 1-2(representing random 90 or -90 angle), how it will rotate depends on the current angle 
        //example: if the random rotation angle is -90 and current angle of AI glider is 0 and it's about to hit right wall, 
        //if it rotates by -90 it will still hit the wall and that's why it rotates (_nuberOfRotation) twice for -90 to avoid hittiing wall
        //if it won't hit the wall after rotation then it just rotates once for that randomised angle

        switch (_currentangle)
        {
            case 0: //current angle
                if (minY <= 1) //if it's about to hit walls checks 
                {
                    if (rand == 1) SetForRotating(rand, 90, 1);
                    else if (rand == 2)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 0, 2);
                        else SetForRotating(rand, 180, 1);
                    }
                }
                else if (maxX >= _gridsize.x - 3)
                {
                    if (rand == 1)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 0, 2);
                        else SetForRotating(rand, 180, 1);
                    }
                    else if (rand == 2) SetForRotating(rand, -90, 1);
                }
                break;
            case 90:
                if (maxY >= _gridsize.y - 2)
                {
                    if (rand == 1)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 90, 2);
                        else SetForRotating(rand, -90, 1);
                    }
                    else if (rand == 2) SetForRotating(rand, 0, 1);
                }
                else if (maxX >= _gridsize.x - 2)
                {
                    if (rand == 1) SetForRotating(rand, 180, 1);
                    else if (rand == 2)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 90, 2);
                        else SetForRotating(rand, -90, 1);
                    }
                }
                break;
            case 180:
                if (minX <= 1)
                {
                    if (rand == 1)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 180, 2);
                        else SetForRotating(rand, 0, 1);
                    }
                    else if (rand == 2) SetForRotating(rand, 90, 1);
                }
                else if (maxY >= _gridsize.y - 3)
                {
                    if (rand == 1) SetForRotating(rand, -90, 1);
                    else if (rand == 2)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, 180, 2);
                        else SetForRotating(rand, 0, 1);
                    }
                }
                break;
            case -90:
                if (minY <= 1)
                {
                    if (rand == 1)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, -90, 2);
                        else SetForRotating(rand, 90, 1);
                    }
                    else if (rand == 2) SetForRotating(rand, 180, 1);

                }
                else if (minX <= 1)
                {
                    if (rand == 1) SetForRotating(rand, 0, 1);
                    else if (rand == 2)
                    {
                        if (_numberOfRotation == 0) SetForRotating(rand, -90, 2);
                        else SetForRotating(rand, 90, 1);
                    }
                }
                break;
        }
    }

    private void SetForRotating(int angle, int currentangle, int numOfRotation)
    {
        _numberOfRotation = numOfRotation;
        RotateAI(angle);
        _currentangle = currentangle;
    }

    //Rotation happens around the center point of the AI glider = center cell
    //Gets the current position of AI cells and calculates the rotated position cell. Sets IsAI=true for the new cells sitting on rotated position
    //and isAI=false for the current position cells
    private void RotateAI(int angle)
    {
        List<Cell> newAIcells = new();
        float newX = 0, newY = 0;
        for (int i = 0; i < _AIcells.Count; i++)
        {
            //90 angle
            if (angle == 1)
            {
                newX = _centerPoint.x + (_centerPoint.y - _AIcells[i].GetCellPosition().y);
                newY = _centerPoint.y - (_centerPoint.x - _AIcells[i].GetCellPosition().x);
            }
            //-90 angle
            else if (angle == 2)
            {
                newX = _centerPoint.x - (_centerPoint.y - _AIcells[i].GetCellPosition().y);
                newY = _centerPoint.y + (_centerPoint.x - _AIcells[i].GetCellPosition().x);

            }

            for (int j = 0; j < _cells.Length; j++)
            {
                if (_cells[j].GetCellPosition().x == newX && _cells[j].GetCellPosition().y == newY)
                {
                    newAIcells.Add(_cells[j]);
                    _cells[j].SetIsAI(true);
                    _cells[j].SetIsAlive(_cells[j].GetIsAlive());
                    break;
                }
            }

        }
        int m = 0;
        for (int i = 0; i < _AIcells.Count; i++)
        {
            for (int j = 0; j < newAIcells.Count; j++)
            {
                if (newAIcells[j] == _AIcells[i]) m++;
            }
            if (m == 0)
            {
                _AIcells[i].SetIsAI(false);
                _AIcells[i].SetIsAlive(_AIcells[i].GetIsAlive());
            }
            m = 0;
        }

        if (_numberOfRotation == 1)
        {
            _wait = 1;
            _numberOfRotation = 0;
        }
        else if (_numberOfRotation == 2) _numberOfRotation = 1; //If it needs to rotate twice in order to not hit the wall
    }
}
