using UnityEngine;
using UnityEngine.InputSystem;
using VectorFlow.Core;

namespace VectorFlow.Managers
{
    public class InputManager : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Idle) return;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePos.z = 0;
                
                Vector2Int gridPos = GridManager.Instance.GetGridPosition(mousePos);
                
                if (GridManager.Instance.IsValidPosition(gridPos))
                {
                    CellType cell = GridManager.Instance.GetCellType(gridPos);
                    
                    if (IsArrow(cell))
                    {
                        if (GameManager.Instance.TryConsumeEnergy())
                        {
                            GameManager.Instance.ChangeState(GameState.Playing);
                            BeamManager.Instance.SpawnBeamFromArrow(gridPos, cell);
                        }
                    }
                }
            }
        }

        private bool IsArrow(CellType c)
        {
            return c >= CellType.ArrowUp && c <= CellType.ArrowLeft;
        }
    }
}
