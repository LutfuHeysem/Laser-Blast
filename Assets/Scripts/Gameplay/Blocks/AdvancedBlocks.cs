
using UnityEngine;
using VectorFlow.Managers;
using VectorFlow.Core;

namespace VectorFlow.Gameplay.Blocks
{
    public class ArrowBehavior : IBlockBehavior
    {
        private CellType arrowType;
        
        public ArrowBehavior(CellType type)
        {
            this.arrowType = type;
        }

        public void OnBeamHit(Beam beam)
        {
            BeamManager.Instance.SpawnBeamFromArrow(beam.pos, arrowType);
            beam.active = false;
        }
    }

    public class MirrorBehavior : IBlockBehavior
    {
        private bool isNW_SE; // \
        
        public MirrorBehavior(bool isNW_SE)
        {
            this.isNW_SE = isNW_SE;
        }

        public void OnBeamHit(Beam beam)
        {
            int tmp = beam.dir.x;
            if (isNW_SE) // \
            {
                beam.dir.x = beam.dir.y;
                beam.dir.y = tmp;
            }
            else // /
            {
                beam.dir.x = -beam.dir.y;
                beam.dir.y = -tmp;
            }
        }
    }

    public class PrismBehavior : IBlockBehavior
    {
        private bool isHorizontal;
        
        public PrismBehavior(bool isHorizontal)
        {
            this.isHorizontal = isHorizontal;
        }

        public void OnBeamHit(Beam beam)
        {
            beam.active = false;
            if (isHorizontal)
            {
                BeamManager.Instance.AddPendingBeam(new Beam { pos = beam.pos, dir = new Vector2Int(-1, 0), active = true });
                BeamManager.Instance.AddPendingBeam(new Beam { pos = beam.pos, dir = new Vector2Int(1, 0), active = true });
            }
            else
            {
                BeamManager.Instance.AddPendingBeam(new Beam { pos = beam.pos, dir = new Vector2Int(0, -1), active = true });
                BeamManager.Instance.AddPendingBeam(new Beam { pos = beam.pos, dir = new Vector2Int(0, 1), active = true });
            }
        }
    }

    public class TNTBehavior : IBlockBehavior
    {
        public void OnBeamHit(Beam beam)
        {
            beam.active = false;
            ExplodeTNT(beam.pos);
        }

        private void ExplodeTNT(Vector2Int center)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2Int target = new Vector2Int(center.x + x, center.y + y);
                    if (GridManager.Instance.IsValidPosition(target))
                    {
                        CellType t = GridManager.Instance.GetCellType(target);
                        if (t != CellType.Empty && t != CellType.GoalHole)
                        {
                            GridManager.Instance.SetCellType(target, CellType.Empty);
                        }
                    }
                }
            }
        }
    }

    public class GoalHoleBehavior : IBlockBehavior
    {
        public void OnBeamHit(Beam beam)
        {
            beam.active = false;
            GameManager.Instance.ChangeState(GameState.Won);
        }
    }
}
