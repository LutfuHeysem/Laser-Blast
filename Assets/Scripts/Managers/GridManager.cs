using UnityEngine;
using VectorFlow.Core;
using VectorFlow.Data;

namespace VectorFlow.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Ayarları")]
        public LevelData currentLevel;
        
        [Header("İkili Çapa (Dual Anchor) Sistemi")]
        [Tooltip("Arka plan resmindeki EN SOL ÜST karenin tam merkezi")]
        public Transform topLeftAnchor; 
        [Tooltip("Arka plan resmindeki EN SAĞ ALT karenin tam merkezi")]
        public Transform bottomRightAnchor;

        [Header("Görsel (Opsiyonel)")]
        [Tooltip("Hizalama testi yapmak için boş kare atayabilirsiniz. İşim bitince boş bırakın.")]
        public GameObject emptyBlockPrefab; 
        public Transform gridParent;

        private CellType[,] logicalGrid;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void InitializeGrid(LevelData levelData)
        {
            currentLevel = levelData; 
            logicalGrid = new CellType[levelData.rows, levelData.cols];
            
            if (topLeftAnchor == null || bottomRightAnchor == null)
            {
                Debug.LogError("[GridManager] Çapa noktaları eksik! Lütfen Inspector'dan TopLeft ve BottomRight atamalarını yapın.");
                return;
            }

            for (int r = 0; r < levelData.rows; r++)
            {
                for (int c = 0; c < levelData.cols; c++)
                {
                    logicalGrid[r, c] = levelData.GetCell(c, r);
                    
                    if (emptyBlockPrefab != null)
                    {
                        Vector3 spawnPos = GetWorldPosition(new Vector2Int(c, r));
                        // Test kutuları TNT/Ok objelerinin arkasında kalsın diye Z eksenini 0.1f yapıyoruz
                        spawnPos.z = 0.1f; 
                        Instantiate(emptyBlockPrefab, spawnPos, Quaternion.identity, gridParent != null ? gridParent : transform);
                    }
                }
            }
            
            FitCameraToGrid();
            Debug.Log($"[GridManager] Çift Anchor'lı Grid {levelData.cols}x{levelData.rows} başarıyla kuruldu.");
        }

        // --- İKİ NOKTA ARASINI KUSURSUZ BÖLEN MATEMATİK ---
        public Vector3 GetWorldPosition(Vector2Int gridPos)
        {
            if (topLeftAnchor == null || bottomRightAnchor == null) return Vector3.zero;

            // X ekseninde yüzde kaçıncı sıradayız? (0 ile 1 arası bir değer)
            float percentX = (float)gridPos.x / (currentLevel.cols - 1);
            
            // Y ekseninde yüzde kaçıncı sıradayız? (0 ile 1 arası bir değer)
            float percentY = (float)gridPos.y / (currentLevel.rows - 1);

            // Lerp (Linear Interpolation) ile o yüzdeye denk gelen tam koordinatı bul
            float xPos = Mathf.Lerp(topLeftAnchor.position.x, bottomRightAnchor.position.x, percentX);
            float yPos = Mathf.Lerp(topLeftAnchor.position.y, bottomRightAnchor.position.y, percentY);

            // Oyun objeleri önde dursun diye Z'yi 0 yapıyoruz
            return new Vector3(xPos, yPos, 0);
        }

        // Fare tıklamaları için tersine hesaplama (Hangi hücreye tıklandı?)
        public Vector2Int GetGridPosition(Vector3 worldPos)
        {
            if (topLeftAnchor == null || bottomRightAnchor == null) return Vector2Int.zero;

            // Tıklanan yerin iki nokta arasında yüzde kaça denk geldiğini bul
            float percentX = Mathf.InverseLerp(topLeftAnchor.position.x, bottomRightAnchor.position.x, worldPos.x);
            float percentY = Mathf.InverseLerp(topLeftAnchor.position.y, bottomRightAnchor.position.y, worldPos.y);

            // Bu yüzdeyi sütun ve satır sayısıyla çarparak tam indeksi (0, 1, 2...) bul
            int x = Mathf.RoundToInt(percentX * (currentLevel.cols - 1));
            int y = Mathf.RoundToInt(percentY * (currentLevel.rows - 1));

            return new Vector2Int(x, y);
        }

        // Kamerayı her zaman o iki noktanın tam ortasına odaklar
        public void FitCameraToGrid()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null || topLeftAnchor == null || bottomRightAnchor == null) return;
            
            // İki noktanın tam ortasını (merkez) bul
            float centerX = (topLeftAnchor.position.x + bottomRightAnchor.position.x) / 2f;
            float centerY = (topLeftAnchor.position.y + bottomRightAnchor.position.y) / 2f;

            mainCam.transform.position = new Vector3(centerX, centerY, -10f);
            
            // Genişlik ve yüksekliği iki nokta arasındaki mesafeden al
            float width = Mathf.Abs(bottomRightAnchor.position.x - topLeftAnchor.position.x) + 2f;
            float height = Mathf.Abs(topLeftAnchor.position.y - bottomRightAnchor.position.y) + 3f;

            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = width / height;

            // Sığdırma işlemi
            if (screenRatio >= targetRatio) mainCam.orthographicSize = height / 2f;
            else mainCam.orthographicSize = (height / 2f) * (targetRatio / screenRatio);
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
    }
}