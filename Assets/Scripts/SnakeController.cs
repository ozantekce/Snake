using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController
{

    private TailList _tailList;

    #region ControlVariables
    [SerializeField]
    private Vector2Int _headStartPos;
    private int _gameSize;
    private bool _isAlive = true;
    #endregion


    private Direction _lastDirection = Direction.None;
    


    public void CreateSnake(int gameSize)
    {
        GameSize = gameSize;
        HeadStartPos = new Vector2Int(gameSize/2,gameSize/2);
        _tailList = new TailList(HeadStartPos);
    }


    private PositionTypePair[] _dirtyPositions = new PositionTypePair[4];
    public void Move(Direction direction, Vector2Int foodPosition
        , out PositionTypePair[] dirtyPoses,out int dirtyPosesSize)
    {
        dirtyPosesSize = 0;
        dirtyPoses = _dirtyPositions;
        if (!IsAlive) return;

        if (!MoveInputValid(direction))
        {
            if (_lastDirection != Direction.None) direction = _lastDirection;   // last valid direction
            else return;    // game not start yet
        }

        Vector2Int nextPos = HeadPosition.NextPos(direction);


        if(!nextPos.CheckPositionValid(GameSize)) // cannot move out of game area
        {
            Debug.LogError("DIE");
            IsAlive = false;
            return;
        }

        if (!IsPositionEmpty(nextPos)) // cannot move full cell
        {
            Debug.LogError("DIE "+ nextPos+ " dir : "+direction);
            IsAlive = false;
            return;
        }


        bool eatFood = foodPosition == nextPos;

        if(eatFood)
        {
            Vector2Int oldHeadPosition = HeadPosition;
            _tailList.AddNode(nextPos);
            Vector2Int newHeadPosition = HeadPosition;

            AddDirtyPosition(oldHeadPosition, CellType.Tail, ref dirtyPosesSize);
            AddDirtyPosition(newHeadPosition, CellType.Head, ref dirtyPosesSize);

        }
        else
        {
            Vector2Int oldHeadPosition = HeadPosition;
            Vector2Int oldLastTailPosition = LastTailPosition;

            _tailList.MoveForward(nextPos);

            Vector2Int newHeadPosition = HeadPosition;
            Vector2Int newLastTailPosition = LastTailPosition;



            AddDirtyPosition(oldLastTailPosition, CellType.None, ref dirtyPosesSize);
            AddDirtyPosition(newLastTailPosition, CellType.LastTail, ref dirtyPosesSize);

            if(_tailList.Size>2)
                AddDirtyPosition(oldHeadPosition, CellType.Tail, ref dirtyPosesSize);
            
            AddDirtyPosition(newHeadPosition, CellType.Head, ref dirtyPosesSize);


        }


        _lastDirection = direction;
    }



    private void AddDirtyPosition(Vector2Int pos, CellType type, ref int size)
    {
        _dirtyPositions[size].pos = pos;
        _dirtyPositions[size].partType = type;
        size++;
    }


    private bool MoveInputValid(Direction direction)
    {
        bool c1 = direction == Direction.None;
        if(c1) return false;
        bool c2 = direction == ~_lastDirection;
        if(c2) return false;

        return true;
    }


    private bool IsPositionEmpty(Vector2Int pos)
    {
        if(pos == LastTailPosition) return true; // snake cannot eat lastTail
        return !_tailList.ContainPos(pos);
    }



    #region GetterSetter

    public Vector2Int HeadStartPos { get => _headStartPos; set => _headStartPos = value; }
    public int GameSize { get => _gameSize; set => _gameSize = value; }

    public Vector2Int HeadPosition { get => _tailList.GetHeadCurrentPos(); }
    public Vector2Int LastTailPosition { get => _tailList.GetLastTailCurrentPos(); }
    public bool IsAlive { get => _isAlive; set => _isAlive = value; }
    public TailList TailList { get => _tailList; set => _tailList = value; }
    public Direction LastDirection { get => _lastDirection; set => _lastDirection = value; }

    #endregion




}


