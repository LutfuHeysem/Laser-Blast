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

        public GameObject emptyBlockPrefab;
        public Transform gridParent;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Start() içindeki yüklemeyi sildik çünkü artık işlemi LevelManager tetikliyor.

        public void InitializeGrid(LevelData levelData)
        {
            currentLevel = levelData; // HAYAT KURTARAN DÜZELTME: Bu satır olmadan GetWorldPosition çöker!
            
            logicalGrid = new CellType[levelData.rows, levelData.cols];
            for (int r = 0; r < levelData.rows; r++)
            {
                for (int c = 0; c < levelData.cols; c++)
                {
                    logicalGrid[r, c] = levelData.GetCell(c, r);
                    
                    // Görsel olarak base gridi oluştur
                    if (emptyBlockPrefab != null)
                    {
                        Vector3 spawnPos = GetWorldPosition(new Vector2Int(c, r));
                        Instantiate(emptyBlockPrefab, spawnPos, Quaternion.identity, gridParent != null ? gridParent : transform);
                    }
                }
            }
            Debug.Log($"[GridManager] Initialized {levelData.cols}x{levelData.rows} grid.");
        }

        public CellType GetCellType(Vector2Int gridPos)
        {
            if (!IsValidPosition(gridPos)) return default(CellType); 
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
            float offsetX = (currentLevel.cols - 1) * cellSize / 2f;
            float offsetY = (currentLevel.rows - 1) * cellSize / 2f;
            return new Vector3((gridPos.x * cellSize) - offsetX, (-gridPos.y * cellSize) + offsetY, 0); 
        }

        public Vector2Int GetGridPosition(Vector3 worldPos)
        {
            float offsetX = (currentLevel.cols - 1) * cellSize / 2f;
            float offsetY = (currentLevel.rows - 1) * cellSize / 2f;
            
            int x = Mathf.RoundToInt((worldPos.x + offsetX) / cellSize);
            int y = Mathf.RoundToInt(-(worldPos.y - offsetY) / cellSize);
            return new Vector2Int(x, y);
        }
    }
}