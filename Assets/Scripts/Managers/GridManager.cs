using UnityEngine;
using VectorFlow.Core;
using VectorFlow.Data;
using System.Collections.Generic;

namespace VectorFlow.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        public LevelData currentLevel;
        public float cellSize = 1f;

        private CellType[,] logicalGrid;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (currentLevel != null)
            {
                InitializeGrid(currentLevel);
            }
        }

        public void InitializeGrid(LevelData levelData)
        {
            logicalGrid = new CellType[levelData.rows, levelData.cols];
            for (int r = 0; r < levelData.rows; r++)
            {
                for (int c = 0; c < levelData.cols; c++)
                {
                    logicalGrid[r, c] = levelData.GetCell(c, r);
                }
            }
            Debug.Log($"[GridManager] Initialized {levelData.cols}x{levelData.rows} grid.");
        }

        public CellType GetCellType(Vector2Int gridPos)
        {
            if (!IsValidPosition(gridPos)) return CellType.SteelWall; // Out of bounds acts like wall
            return logicalGrid[gridPos.y, gridPos.x];
        }

        public void SetCellType(Vector2Int gridPos, CellType newType)
        {
            if (!IsValidPosition(gridPos)) return;
            logicalGrid[gridPos.y, gridPos.x] = newType;
        }

        public bool IsValidPosition(Vector2Int gridPos)
        {
            if (logicalGrid == null) return false;
            return gridPos.x >= 0 && gridPos.x < logicalGrid.GetLength(1) &&
                   gridPos.y >= 0 && gridPos.y < logicalGrid.GetLength(0);
        }

        public Vector3 GetWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * cellSize, -gridPos.y * cellSize, 0); 
        }

        public Vector2Int GetGridPosition(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / cellSize);
            int y = Mathf.RoundToInt(-worldPos.y / cellSize);
            return new Vector2Int(x, y);
        }
    }
}
