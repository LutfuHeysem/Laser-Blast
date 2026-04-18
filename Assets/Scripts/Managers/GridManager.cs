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

        [Header("Görsel Ayarlar")]
        public GameObject blackGlassPrefab;
        public float glassPadding = 0.2f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void InitializeGrid(LevelData levelData)
        {
            currentLevel = levelData; 
            logicalGrid = new CellType[levelData.rows, levelData.cols];
            
            for (int r = 0; r < levelData.rows; r++)
            {
                for (int c = 0; c < levelData.cols; c++)
                {
                    logicalGrid[r, c] = levelData.GetCell(c, r);
                    
                    if (emptyBlockPrefab != null)
                    {
                        // Z değerini 0.1f yaparak arka planı objelerin biraz gerisine itiyoruz
                        Vector3 spawnPos = GetWorldPosition(new Vector2Int(c, r));
                        spawnPos.z = 0.1f; 

                        Instantiate(emptyBlockPrefab, spawnPos, Quaternion.identity, gridParent != null ? gridParent : transform);
                    }
                }
            }
            
            // Arka planları oluştur ve kamerayı ayarla
            CreateBlackGlassBackground();
            FitCameraToGrid(levelData.cols, levelData.rows, 0.5f, 2.0f);
            
            Debug.Log($"[GridManager] Grid {levelData.cols}x{levelData.rows} boyutunda kuruldu.");
        }

        private void CreateBlackGlassBackground()
        {
            if (blackGlassPrefab == null) return;

            float totalWidth = (currentLevel.cols * cellSize) + glassPadding;
            float totalHeight = (currentLevel.rows * cellSize) + glassPadding;

            // En arkada durması için Z'yi 0.5f yapıyoruz
            Vector3 glassPosition = new Vector3(0, 0, 0.5f);
            GameObject glass = Instantiate(blackGlassPrefab, glassPosition, Quaternion.identity, transform);

            glass.transform.localScale = new Vector3(totalWidth, totalHeight, 1f);
            glass.name = "Background_BlackGlass";
            
            // Kodla da Order in Layer'ı garantiye alalım (isteğe bağlı)
            var sr = glass.GetComponent<SpriteRenderer>();
            if(sr != null) sr.sortingOrder = -10;
        }

        public Vector3 GetWorldPosition(Vector2Int gridPos)
        {
            float offsetX = (currentLevel.cols - 1) * cellSize / 2f;
            float offsetY = (currentLevel.rows - 1) * cellSize / 2f;
            // Gameplay objeleri tam 0 noktasında oluşur
            return new Vector3((gridPos.x * cellSize) - offsetX, (-gridPos.y * cellSize) + offsetY, 0);
        }

        // --- DİĞER FONKSİYONLAR (Aynı Kalıyor) ---
        public bool IsValidPosition(Vector2Int gridPos)
        {
            if (logicalGrid == null) return false;
            return gridPos.x >= 0 && gridPos.x < logicalGrid.GetLength(1) &&
                   gridPos.y >= 0 && gridPos.y < logicalGrid.GetLength(0);
        }

        public void FitCameraToGrid(int cols, int rows, float paddingX = 0.5f, float paddingY = 2.0f)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return;
            mainCam.transform.position = new Vector3(0, 0, -10f);
            float gridWidth = cols * cellSize + paddingX;
            float gridHeight = rows * cellSize + paddingY;
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = gridWidth / gridHeight;

            if (screenRatio >= targetRatio) mainCam.orthographicSize = gridHeight / 2f;
            else mainCam.orthographicSize = (gridHeight / 2f) * (targetRatio / screenRatio);
        }
    }
}