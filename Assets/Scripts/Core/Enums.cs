namespace VectorFlow.Core
{
    public enum CellType
    {
        Empty = 0,
        ArrowUp = 1,
        ArrowRight = 2,
        ArrowDown = 3,
        ArrowLeft = 4,
        GlassWall = 5,
        SteelWall = 6,
        Mirror_NW_SE = 7, // \
        Mirror_NE_SW = 8, // /
        Prism_H = 9,      // splits left/right
        Prism_V = 10,     // splits up/down
        TNT = 11,
        GoalHole = 12
    }

    public enum GameState
    {
        Idle,
        Playing,
        Won,
        Lost
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
