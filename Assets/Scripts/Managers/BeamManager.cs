using UnityEngine;
using VectorFlow.Core;
using System.Collections.Generic;
using System.Collections;

namespace VectorFlow.Managers
{
    public class Beam
    {
        public Vector2Int pos;
        public Vector2Int dir;
        public bool active;
    }

    public class BeamManager : MonoBehaviour
    {
        public static BeamManager Instance { get; private set; }

        public float stepDelay = 0.15f; 
        private List<Beam> activeBeams = new List<Beam>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SpawnBeamFromArrow(Vector2Int pos, CellType arrowType)
        {
            Vector2Int dir = Vector2Int.zero;
            if (arrowType == CellType.ArrowUp) dir = new Vector2Int(0, -1);
            else if (arrowType == CellType.ArrowRight) dir = new Vector2Int(1, 0);
            else if (arrowType == CellType.ArrowDown) dir = new Vector2Int(0, 1);
            else if (arrowType == CellType.ArrowLeft) dir = new Vector2Int(-1, 0);

            activeBeams.Add(new Beam { pos = pos, dir = dir, active = true });

            if (activeBeams.Count == 1) // Start loop
            {
                StartCoroutine(BeamLoop());
            }
        }

        private IEnumerator BeamLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(stepDelay);
                StepBeams();

                bool anyActive = false;
                foreach(var b in activeBeams) if(b.active) anyActive = true;

                if (!anyActive && activeBeams.Count > 0)
                {
                    if (GameManager.Instance.CurrentState == GameState.Playing)
                    {
                        GameManager.Instance.ChangeState(GameState.Lost);
                    }
                    activeBeams.Clear();
                    break;
                }
            }
        }

        private void StepBeams()
        {
            List<Beam> newBeams = new List<Beam>();

            foreach(var beam in activeBeams)
            {
                if (!beam.active) continue;

                beam.pos += beam.dir;

                if (!GridManager.Instance.IsValidPosition(beam.pos))
                {
                    beam.active = false;
                    continue;
                }

                CellType cell = GridManager.Instance.GetCellType(beam.pos);

                if (cell == CellType.Empty)
                {
                    // Continue
                }
                else if (cell == CellType.SteelWall)
                {
                    beam.active = false;
                }
                else if (cell == CellType.GlassWall)
                {
                    GridManager.Instance.SetCellType(beam.pos, CellType.Empty);
                }
                else if (cell >= CellType.ArrowUp && cell <= CellType.ArrowLeft)
                {
                    SpawnBeamFromArrow(beam.pos, cell);
                    beam.active = false;
                }
                else if (cell == CellType.Mirror_NW_SE) // \
                {
                    int tmp = beam.dir.x;
                    beam.dir.x = beam.dir.y;
                    beam.dir.y = tmp;
                }
                else if (cell == CellType.Mirror_NE_SW) // /
                {
                    int tmp = beam.dir.x;
                    beam.dir.x = -beam.dir.y;
                    beam.dir.y = -tmp;
                }
                else if (cell == CellType.Prism_H)
                {
                    beam.active = false;
                    newBeams.Add(new Beam { pos = beam.pos, dir = new Vector2Int(-1,0), active=true });
                    newBeams.Add(new Beam { pos = beam.pos, dir = new Vector2Int(1,0), active=true });
                }
                else if (cell == CellType.Prism_V)
                {
                    beam.active = false;
                    newBeams.Add(new Beam { pos = beam.pos, dir = new Vector2Int(0,-1), active=true });
                    newBeams.Add(new Beam { pos = beam.pos, dir = new Vector2Int(0,1), active=true });
                }
                else if (cell == CellType.TNT)
                {
                    beam.active = false;
                    ExplodeTNT(beam.pos);
                }
                else if (cell == CellType.GoalHole)
                {
                    beam.active = false;
                    GameManager.Instance.ChangeState(GameState.Won);
                }
            }

            activeBeams.AddRange(newBeams);
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
}
