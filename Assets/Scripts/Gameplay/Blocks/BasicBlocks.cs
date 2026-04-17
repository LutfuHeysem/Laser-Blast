using UnityEngine;
using VectorFlow.Managers;

namespace VectorFlow.Gameplay.Blocks
{
    public class EmptyBehavior : IBlockBehavior
    {
        public void OnBeamHit(Beam beam)
        {
            // Do nothing, beam continues
        }
    }

    public class SteelWallBehavior : IBlockBehavior
    {
        public void OnBeamHit(Beam beam)
        {
            beam.active = false;
        }
    }

    public class GlassWallBehavior : IBlockBehavior
    {
        public void OnBeamHit(Beam beam)
        {
            GridManager.Instance.SetCellType(beam.pos, Core.CellType.Empty);
        }
    }
}
