using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HELPER
{

    private static Dictionary<Direction, Vector2Int> DirectionToDeltaDic = new Dictionary<Direction, Vector2Int>() 
    {

            {Direction.None, Vector2Int.zero },
            {Direction.Left, new Vector2Int(0,-1) },
            {Direction.Right, new Vector2Int(0,+1) },
            {Direction.Up, new Vector2Int(-1,0) },
            {Direction.Down, new Vector2Int(+1,0) },
    };

    private static Dictionary<Vector2Int, Direction> DeltaToDirectionDic= new Dictionary<Vector2Int, Direction>() 
    {
        {Vector2Int.zero, Direction.None },
        {new Vector2Int(0,-1), Direction.Left },
        {new Vector2Int(0,+1), Direction.Right },
        {new Vector2Int(-1,0), Direction.Up },
        {new Vector2Int(+1,0), Direction.Down },
    };


    private static Dictionary<char, Direction> CharToDirectionDic = new Dictionary<char, Direction>
    {
        {'n', Direction.None },
        {'u',Direction.Up },
        {'d',Direction.Down },
        {'l',Direction.Left },
        {'r',Direction.Right }
    };

    private static Dictionary<Direction, char> DirectionToCharDic = new Dictionary<Direction, char>
    {
        {Direction.None, 'n' },
        {Direction.Up, 'u' },
        {Direction.Down, 'd' },
        {Direction.Left, 'l' },
        {Direction.Right, 'r' }
    };


    private static Dictionary<Vector2Int, List<Vector2Int>> NextPositions = new Dictionary<Vector2Int, List<Vector2Int>>();

    public static Vector2Int ToDelta(this Direction direction)
    {
        return DirectionToDeltaDic[direction];
    }


    public static Direction ToDirection(this Vector2Int delta)
    {
        return DeltaToDirectionDic[delta];
    }


    public static Direction ToDirection(this char c)
    {
        return CharToDirectionDic[c];
    }

    public static char ToChar(this Vector2Int delta)
    {
        return DeltaToDirectionDic[delta].ToChar();
    }


    public static char ToChar(this Direction direction)
    {
        return DirectionToCharDic[direction];
    }


    public static Vector2Int NextPos(this Vector2Int currentPos, Direction direction)
    {
        return currentPos + direction.ToDelta();
    }


    public static List<Vector2Int> GetNextPoses(this Vector2Int currentPos)
    {

        List<Vector2Int> result;
        if(NextPositions.ContainsKey(currentPos)) { 
            result = NextPositions[currentPos];
        }
        else
        {
            result = new List<Vector2Int>
            {
                currentPos.NextPos(Direction.Left),
                currentPos.NextPos(Direction.Right),
                currentPos.NextPos(Direction.Up),
                currentPos.NextPos(Direction.Down)
            };
            NextPositions[currentPos] = result;
        }

        return result;

    }


    public static bool CheckPositionValid(this Vector2Int nextPos, int gameSize)
    {
        int x = nextPos.x;
        if (x < 0 || x >= gameSize) return false;
        int y = nextPos.y;
        if (y < 0 || y >= gameSize) return false;

        return true;
    }


    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}




public struct PositionTypePair
{
    public Vector2Int pos;
    public CellType partType;
}