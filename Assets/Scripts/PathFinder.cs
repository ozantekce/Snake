using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PathFinder
{


    private int _gameSize;


    private int[][] _addCheck;

    public PathFinder(int gameSize) { 
        this._gameSize = gameSize;
        _addCheck = new int[gameSize][];
        for (int i = 0; i < gameSize; i++)
        {
            _addCheck[i] = new int[gameSize];
        }
    }


    private void ResetAddCheck(int maxAdd)
    {
        for (int i = 0; i < _gameSize; i++)
        {
            for (int j = 0; j < _gameSize; j++)
            {
                _addCheck[i][j] = maxAdd;
            }
        }
    }


    private Frontier _frontier = new FrontierHeap();

    private HashSet<Vector2Int> _visited = new HashSet<Vector2Int>();
    

    public string FindPath(TailList tailList,Direction direction, Vector2Int target, bool longPath)
    {

        ResetAddCheck(1);
        _frontier.Clear();
        _visited.Clear();

        MyVector2 first 
            = new MyVector2(tailList.GetHeadCurrentPos(), "", direction, SquaredDistance(tailList.GetHeadCurrentPos(), target));
        _frontier.Add(first);

        int index = 0;

        while (!_frontier.IsEmpty())
        {

            MyVector2 current = _frontier.Remove();
            Vector2Int currentPos = current.Pos;
            string currentPath = current.Path;
            Direction currentDirection = current.Direction;
            //Debug.Log(currentPos + " " + currentPath + " " +target);

            if (currentPos == target) return currentPath;

            if (_visited.Contains(currentPos)) continue;
            _visited.Add(currentPos);

            List<Vector2Int> nextPoses = currentPos.GetNextPoses();
            nextPoses.Shuffle();
            for (int i = 0; i < nextPoses.Count; i++)
            {
                if (nextPoses[i].CheckPositionValid(_gameSize))
                {
                    Vector2Int nextPos = nextPoses[i];
                    if(nextPos == target && index == 0 && longPath)
                    {
                        continue;
                    }

                    if (_visited.Contains(nextPos) || _addCheck[nextPos.x][nextPos.y]<=0) continue;
                    if (tailList.ContainPos(nextPos) && tailList.GetLastTailCurrentPos()!=nextPos) continue;
                    if (currentDirection == ~(nextPos - currentPos).ToDirection()) continue;
                    _addCheck[nextPos.x][nextPos.y]--;

                    string nextPath = currentPath + (nextPos - currentPos).ToChar();

                    MyVector2 next 
                        = new MyVector2(nextPos, nextPath, (nextPos - currentPos).ToDirection(), SquaredDistance(nextPos, target));

                    if (longPath)
                    {
                        next.Score *= -1;
                    }

                    _frontier.Add(next);

                }

            }


            index++;

        }




        return null;
    }


    public static float SquaredDistance(Vector2Int pointA, Vector2Int pointB)
    {
        int dx = pointB.x - pointA.x;
        int dy = pointB.y - pointA.y;

        return ((dx * dx + dy * dy));
    }



    private interface Frontier
    {

        public void Add(MyVector2 t);
        public MyVector2 Remove();

        public void Clear();

        public bool IsEmpty();

    }

    private class FrontierList : Frontier
    {
        private List<MyVector2> _list;
        private int _index = 0;

        public FrontierList()
        {
            _list = new List<MyVector2>();
        }
        
        public void Add(MyVector2 t)
        {
            _list.Add(t);
        }

        public void Clear()
        {
            _list.Clear();
            _index = 0;
        }

        public bool IsEmpty()
        {
            return _index == _list.Count;
        }

        public MyVector2 Remove()
        {
            MyVector2 t = _list[_index];
            _index++;
            return t;
        }
    }


    private class FrontierHeap : Frontier
    {
        private Heap<MyVector2> _heap;
        public FrontierHeap()
        {
            _heap = new Heap<MyVector2>();
        }


        public void Add(MyVector2 t)
        {
            _heap.Insert(t);

        }

        public void Clear()
        {
            _heap.Clear();
        }

        public bool IsEmpty()
        {
            return _heap.IsEmpty();
        }


        public MyVector2 Remove()
        {
            return _heap.Remove();
        }

    }
    public class MyVector2 : IComparable<MyVector2>
    {

        private Vector2Int _pos;
        private string _path;
        private float _score;
        private Direction _direction;
        public MyVector2(Vector2Int pos,string path,Direction direction, float score)
        {
            _pos = pos;
            _path = path;
            _score = score;
            _direction = direction;
        }


        public Vector2Int Pos { get => _pos; set => _pos = value; }

        public float Score { get => _score; set => _score = value; }
        public string Path { get => _path; set => _path = value; }
        public Direction Direction { get => _direction; set => _direction = value; }

        public int CompareTo(MyVector2 other)
        {
            if (other.Score == Score) return 0;
            else if (other.Score > Score) return -1;
            else return 1;
        }
    }



}
