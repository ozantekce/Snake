using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int gameSize;
    public GameArea gameArea;
    public SnakeRenderer snakeRenderer;
    public GameObject food;


    public PathFinder pathFinder;

    public SnakeController snakeController;
    public FoodCreator foodCreator;
    


    

    public bool gameOver;

    public bool usePathFinder;

    private const float MinGameSpeed = 1;
    [Range(1.0f, 100f)]
    public float gameSpeed = 1;



    private float _elapsedTimeLastMove = 0;
    private float _waitToNextMove = 0;

    void Awake()
    {

        snakeController = new SnakeController();

        gameArea.CreateGameArea(gameSize);
        snakeController.CreateSnake(gameSize);
        snakeRenderer.ArrangeSize(1f/gameSize);
        snakeRenderer.ArrangeStartPosition(gameArea.GetRealPosition(snakeController.HeadStartPos));

        food.transform.localScale = new Vector3(1, 1, 20) * (1f/gameSize)/2.6f;

        foodCreator = new FoodCreator();
        foodCreator.Init(snakeController,gameSize);

    }

    private void Start()
    {
        RenderGameArea(snakeController.HeadPosition, CellType.Head);
        RenderGameArea(foodCreator.FoodPosition, CellType.Food);
    }


    void Update()
    {
        if (gameOver) return;

        _elapsedTimeLastMove += Time.deltaTime;

        bool rst = foodCreator.Tick(ref gameOver);
        if (gameOver) return;

        if (rst == true) RenderGameArea(foodCreator.FoodPosition, CellType.Food);

        bool canMove = _elapsedTimeLastMove > _waitToNextMove && !snakeRenderer.Moving;


        if (canMove)
        {
            Direction dir = ReadInput();
            MoveSnake(dir);
            _elapsedTimeLastMove = 0;
            _waitToNextMove = MinGameSpeed / gameSpeed;
            snakeRenderer.ArrangeTailSize(snakeController.TailList.Size, gameSpeed);
            snakeRenderer.MoveHead(gameArea.GetRealPosition(snakeController.HeadPosition), _waitToNextMove);
        }
            

    }


    [SerializeField]
    private Direction _readInput = Direction.None;
    public Direction ReadInput()
    {
        _readInput = Direction.None;
        if (usePathFinder)
        {
            if(_currentPath.Count == 0)
            {
                pathFinder ??= new PathFinder(gameSize);

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
            PathToQueue(pathToFood);
            return true;
        }

        return false;
    }


    private bool FindPathToTail()
    {
        string pathToTail 
            = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, snakeController.TailList.GetLastTailCurrentPos(), true);
        if (pathToTail != null)
        {
            PathToQueue(pathToTail);
            return true;
        }
        else
        {
            Debug.LogError("Cannot Find");
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
                pathToTail = pathFinder.FindPath(snakeController.TailList, snakeController.LastDirection, snakeController.TailList.GetLastTailCurrentPos(), false);
                if (pathToTail != null)
                {
                    PathToQueue(pathToTail);
                    return true;
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
    public void PathToQueue(string path)
    {
        _currentPath.Clear();

        for (int i = 0; i < path.Length; i++)
        {
            _currentPath.Enqueue(path[i].ToDirection());
        }

    }


}
