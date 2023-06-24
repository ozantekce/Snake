public enum Direction
{
    None = 0,
    Up = 1,
    Down = ~Up,
    Right = 2,
    Left = ~Right,
}


public enum CellType
{
    None,
    Head,
    Tail,
    LastTail,
    Food
}