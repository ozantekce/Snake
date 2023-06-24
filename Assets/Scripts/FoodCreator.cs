using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCreator
{

    private SnakeController _snakeController;

    private Vector2Int _foodPosition;

    private TailList _tailList;

    private int _gameSize;

    public void Init(SnakeController snakeController,int gameSize)
    {
        this._gameSize = gameSize;
        this._snakeController = snakeController;
        this._tailList = this._snakeController.TailList;
        TryCreateNewFood(gameSize);
    }


    // return true if create a new food
    public bool Tick(ref bool gameOver)
    {
        if(NeedCreateFood())
        {
            if (TryCreateNewFood(_gameSize))
            {
                return true;
            }
            else
            {
                gameOver = true;
                return false;
            }
        }
        return false;
    }


    private bool NeedCreateFood()
    {
        return _foodPosition == _tailList.GetHeadCurrentPos();
    }


    private List<Vector2Int> _cells;

    public Vector2Int FoodPosition { get => _foodPosition; set => _foodPosition = value; }

    private bool TryCreateNewFood(int gameSize)
    {
        
        if(_cells == null)
        {
            _cells = new List<Vector2Int>();
            for (int i = 0; i < gameSize; i++)
            {
                for (int j = 0; j < gameSize; j++)
                {
                    _cells.Add(new Vector2Int(i, j));
                }
            }
        }

        _cells.Shuffle();

        for (int i = 0; i < _cells.Count; i++)
        {
            if (!_tailList.ContainPos(_cells[i]))
            {
                _foodPosition = _cells[i];
                return true;
            }
        }

        return false;
    }


}
