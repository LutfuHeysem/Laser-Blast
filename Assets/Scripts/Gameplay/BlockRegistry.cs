using System.Collections.Generic;
using VectorFlow.Core;
using VectorFlow.Gameplay.Blocks;

namespace VectorFlow.Gameplay
{
    public static class BlockRegistry
    {
        private static readonly Dictionary<CellType, IBlockBehavior> behaviors;

        static BlockRegistry()
        {
            behaviors = new Dictionary<CellType, IBlockBehavior>
            {
                { CellType.Empty, new EmptyBehavior() },
                { CellType.SteelWall, new SteelWallBehavior() },
                { CellType.GlassWall, new GlassWallBehavior() },
                { CellType.ArrowUp, new ArrowBehavior(CellType.ArrowUp) },
                { CellType.ArrowRight, new ArrowBehavior(CellType.ArrowRight) },
                { CellType.ArrowDown, new ArrowBehavior(CellType.ArrowDown) },
                { CellType.ArrowLeft, new ArrowBehavior(CellType.ArrowLeft) },
                { CellType.Mirror_NW_SE, new MirrorBehavior(true) },
                { CellType.Mirror_NE_SW, new MirrorBehavior(false) },
                { CellType.Prism_H, new PrismBehavior(true) },
                { CellType.Prism_V, new PrismBehavior(false) },
                { CellType.TNT, new TNTBehavior() },
                { CellType.GoalHole, new GoalHoleBehavior() }
            };
        }

        public static IBlockBehavior GetBehavior(CellType type)
        {
            if (behaviors.TryGetValue(type, out var behavior))
            {
                return behavior;
            }
            return new SteelWallBehavior(); // Default fallback
        }
    }
}
