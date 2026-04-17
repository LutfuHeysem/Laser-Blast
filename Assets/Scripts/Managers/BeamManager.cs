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

        private List<Beam> pendingBeams = new List<Beam>();

        public void AddPendingBeam(Beam beam)
        {
            pendingBeams.Add(beam);
        }

        private void StepBeams()
        {
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
                
                // Use Strategy Pattern via Registry
                var behavior = Gameplay.BlockRegistry.GetBehavior(cell);
                behavior.OnBeamHit(beam);
            }

            if (pendingBeams.Count > 0)
            {
                activeBeams.AddRange(pendingBeams);
                pendingBeams.Clear();
            }
        }
    }
}
