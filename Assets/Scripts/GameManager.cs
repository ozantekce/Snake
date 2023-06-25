using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int _gameSize;
    public GameArea gameArea;
    public SnakeRenderer snakeRenderer;
    public GameObject food;


    public PathFinder pathFinder;

    public SnakeController snakeController;
    public FoodCreator foodCreator;
    


    

    public bool gameOver;


    private int _score;

    private bool _usePathFinder = false;



    private const float MinGameSpeed = 1;
    [SerializeField]
    [Range(1.0f, 100f)]
    private float _gameSpeed = 1;



    private float _elapsedTimeLastMove = 0;
    private float _waitToNextMove = 0;

    void Awake()
    {



    }


    private void Start()
    {
        //StartGame(20);
    }


    private bool _gameStarted = false;
    public void StartGame(int gameSize)
    {
        if(_gameStarted) return;
        _gameStarted = true;
        this._gameSize = gameSize;
        snakeController = new SnakeController();

        gameArea.CreateGameArea(gameSize);
        snakeController.CreateSnake(gameSize);
        snakeRenderer.ArrangeSize(1f / gameSize);
        snakeRenderer.ArrangeStartPosition(gameArea.GetRealPosition(snakeController.HeadStartPos));

        food.transform.localScale = new Vector3(1, 1, 20) * (1f / gameSize) / 2.6f;

        foodCreator = new FoodCreator();
        foodCreator.Init(snakeController, gameSize);

        RenderGameArea(snakeController.HeadPosition, CellType.Head);
        RenderGameArea(foodCreator.FoodPosition, CellType.Food);
    }



    void Update()
    {
        if (gameOver || !_gameStarted) return;

        _elapsedTimeLastMove += Time.deltaTime;

        bool rst = foodCreator.Tick(ref gameOver);
        if (gameOver) return;

        if (rst == true)
        {
            RenderGameArea(foodCreator.FoodPosition, CellType.Food);
            Score++;
        }

        bool canMove = _elapsedTimeLastMove > _waitToNextMove && !snakeRenderer.Moving;

        Direction dir = Direction.None;
        if (_usePathFinder)
        {
            _readInput = Direction.None;
            if (canMove)
            {
                dir = ReadInput();
            }
        }
        else
        {
            dir = ReadInput();
        }

        if (canMove)
        {
            if (_usePathFinder && dir == Direction.None) return;
            MoveSnake(dir);
            _elapsedTimeLastMove = 0;
            _waitToNextMove = MinGameSpeed / _gameSpeed;
            snakeRenderer.ArrangeTailSize(snakeController.TailList.Size, _gameSpeed);
            snakeRenderer.MoveHead(gameArea.GetRealPosition(snakeController.HeadPosition), _waitToNextMove);
        }
            

    }


    [SerializeField]
    private Direction _readInput = Direction.None;
    public Direction ReadInput()
    {
        //_readInput = Direction.None;
        if (_usePathFinder)
        {
            if(_currentPath.Count == 0)
            {
                pathFinder ??= new PathFinder(_gameSize);

                bool foodPath = FindPathToFood();
                if (!foodPath)
                {
                    bool tailPath = FindPathToTail();
                }
            }

            if(_currentPath.Count > 0)
            {
                _readInput = _currentPath.Dequeue();
            }


        }
        else
        {
            int up = (int)Input.GetAxisRaw("Vertical");
            int right = (int)Input.GetAxisRaw("Horizontal");

            if (up > 0)
            {
                _readInput = Direction.Up;
            }
            else if (up < 0)
            {
                _readInput = Direction.Down;
            }
            else if (right > 0)
            {
                _readInput = Direction.Right;
            }
            else if (right < 0)
            {
                _readInput = Direction.Left;
            }
        }

        return _readInput;
    }


    string savedPath;
    private bool FindPathToFood()
    {
        string pathToFood 
            = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, foodCreator.FoodPosition, false);

        if(pathToFood == null) return false;

        // Check can move to tail then eat food
        TailList tempList = new TailList(snakeController.TailList);
        for (int i = 0; i < pathToFood.Length; i++)
        {
            if (i < pathToFood.Length - 1)
                tempList.MoveForward(pathToFood[i].ToDirection());
            else
                tempList.AddNode(pathToFood[i].ToDirection());
        }
        string pathToFoodThenTail = pathFinder.FindPath(tempList, pathToFood[^1].ToDirection(), tempList.GetLastTailCurrentPos(), false);
        if (pathToFoodThenTail != null)
        {
            savedPath = pathToFoodThenTail;
            PathToQueue(pathToFood);
            return true;
        }

        return false;
    }


    private bool FindPathToTail()
    {

        if(savedPath != null)
        {
            PathToQueue(savedPath);
            savedPath = null;
            return true;
        }

        string pathToTail 
            = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, snakeController.TailList.GetLastTailCurrentPos(), true);
        if (pathToTail != null)
        {
            PathToQueue(pathToTail);
            return true;
        }
        else
        {
            //Debug.LogError("Cannot Find");
            for (int i = 0; i < 20; i++)
            {
                pathToTail = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, snakeController.TailList.GetLastTailCurrentPos(), true);
                if (pathToTail != null)
                {
                    PathToQueue(pathToTail);
                    break;
                }
            }
            if (pathToTail == null)
            {
                for (int i = 0; i < 20; i++)
                {
                    pathToTail = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, snakeController.TailList.GetLastTailCurrentPos(), false);
                    if (pathToTail != null)
                    {
                        PathToQueue(pathToTail);
                        break;
                    }
                }
            }

        }
        return false;

    }



    public void MoveSnake(Direction dir)
    {
        
        snakeController.Move(dir, foodCreator.FoodPosition,out PositionTypePair[] dirtyPoses, out int dirtyPosesSize);

        if (!snakeController.IsAlive) gameOver = true;

        RenderGameArea(dirtyPoses,dirtyPosesSize);

    }


    public void RenderGameArea(PositionTypePair[] dirtyPoses, int dirtyPosesSize)
    {

        for (int i = 0; i < dirtyPosesSize; i++)
        {
            gameArea.Render(dirtyPoses[i].pos, dirtyPoses[i].partType);
        }

    }



    public void RenderGameArea(Vector2Int pos, CellType cellType)
    {
        gameArea.Render(pos, cellType);
        if(cellType == CellType.Food)
        {
            float z = food.transform.position.z;
            Vector3 realPos = gameArea.GetRealPosition(pos);
            realPos.z = z;
            food.transform.position = realPos;
        }
    }


    private Queue<Direction> _currentPath = new Queue<Direction>();

    public int GameSize { get => _gameSize;}
    public float GameSpeed { get => _gameSpeed; set => _gameSpeed = value; }
    public bool UsePathFinder { get => _usePathFinder; set { _currentPath.Clear(); _readInput = Direction.None; _usePathFinder = value; } }

    public int Score { get => _score; set => _score = value; }

    public void PathToQueue(string path)
    {
        _currentPath.Clear();

        for (int i = 0; i < path.Length; i++)
        {
            _currentPath.Enqueue(path[i].ToDirection());
        }

    }


}
