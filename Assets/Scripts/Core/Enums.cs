namespace VectorFlow.Core
{
    public enum CellType
    {
        Empty,
        ArrowUp,
        ArrowRight,
        ArrowDown,
        ArrowLeft,
        GlassWall,
        SteelWall,
        Mirror_NW_SE, // \
        Mirror_NE_SW, // /
        Prism_H,      // splits left/right
        Prism_V,      // splits up/down
        TNT,
        GoalHole
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
