using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int gameSize;
    public GameArea gameArea;
    public SnakeRenderer snakeRenderer;

    public SnakeController snakeController;
    public FoodCreator foodCreator;
    
    public GameObject food;

    public bool gameOver;


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

        Direction dir = ReadInput();

        if (_elapsedTimeLastMove > _waitToNextMove && !snakeRenderer.Moving)
        {

            bool rst = foodCreator.Tick(ref gameOver);
            if (gameOver) return;

            if (rst == true) RenderGameArea(foodCreator.FoodPosition, CellType.Food);

            MoveSnake(dir);
            _elapsedTimeLastMove = 0;
            _waitToNextMove = MinGameSpeed / gameSpeed;
            snakeRenderer.ArrangeTailSize(snakeController.TailList.Size, gameSpeed);
            snakeRenderer.MoveHead(gameArea.GetRealPosition(snakeController.HeadPosition), _waitToNextMove);
        }
            

    }



    private Direction _readInput = Direction.None;
    public Direction ReadInput()
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
        return _readInput;
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


}
