using UnityEngine;
using System.Collections.Generic;
using VectorFlow.Data; // LevelData'yı tanıyabilmek için ekledik

namespace VectorFlow.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Ayarlar")]
        public string levelsFolder = "Levels"; // Resources altındaki klasör
        private TextAsset[] availableLevels;
        
        [Header("Prefab Eşleşmeleri")]
        public GameObject prefab_Arrow;
        public GameObject prefab_TNT;
        public GameObject prefab_SteelWall;
        public GameObject prefab_Glass;
        public GameObject prefab_Prism;
        public GameObject prefab_Mirror_NE_SW;
        public GameObject prefab_Mirror_SW_NE;
        public GameObject prefab_Goal;

        public static LevelManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void LoadAvailableLevels()
        {
            availableLevels = Resources.LoadAll<TextAsset>(levelsFolder);
            
            // Level1, Level2, Level10 gibi isimleri doğru sıralamak için sayıya göre diziyoruz
            System.Array.Sort(availableLevels, (a, b) => 
            {
                int numA = ExtractNumber(a.name);
                int numB = ExtractNumber(b.name);
                if (numA == numB) return a.name.CompareTo(b.name);
                return numA.CompareTo(numB);
            });

            Debug.Log($"[LevelManager] Bulunan bölüm sayısı: {availableLevels.Length}");
        }

        private int ExtractNumber(string name)
        {
            string numberString = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
            if (int.TryParse(numberString, out int num)) return num;
            return 999; // Sayı yoksa sona at
        }

        public int GetTotalLevelsCount()
        {
            if (availableLevels == null) LoadAvailableLevels();
            return availableLevels.Length;
        }

        public void LoadLevelByIndex(int index)
        {
            if (availableLevels == null) LoadAvailableLevels();

            if (index < 1 || index > availableLevels.Length)
            {
                Debug.LogError($"[LevelManager] Geçersiz bölüm indeksi: {index}. Toplam bölüm: {availableLevels.Length}");
                return;
            }

            // Önceki seviyeden kalanları temizle
            ClearCurrentLevel();

            // 1 tabanlı indeksi 0 tabanlı diziye çevir
            TextAsset levelFile = availableLevels[index - 1];
            LoadLevel(levelFile);
        }

        public void ClearCurrentLevel()
        {
            // Eski blokları sil
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void LoadLevel(TextAsset levelJsonFile) 
        {
            if (levelJsonFile == null) return;

            LevelData data = JsonUtility.FromJson<LevelData>(levelJsonFile.text);
            if (data != null) 
            {
                Debug.Log("Yüklenen Bölüm: " + data.levelName);

                // 1. Önce GridManager'a "Al bu JSON verisiyle Grid'i kur" diyoruz
                if (GridManager.Instance != null)
                {
                    GridManager.Instance.InitializeGrid(data);
                }

                // 2. GameManager'ı enerjisiyle başlat
                if (VectorFlow.Managers.GameManager.Instance != null)
                {
                    VectorFlow.Managers.GameManager.Instance.InitializeGame(data.startingEnergy);
                }

                // 3. ScoreManager'ı sıfırla
                if (VectorFlow.Managers.ScoreManager.Instance != null)
                {
                    VectorFlow.Managers.ScoreManager.Instance.InitializeScore();
                }

                // 4. Sonra senin Prefablari diziyoruz
                foreach (var entry in data.blocks) 
                {
                    SpawnBlock(entry);
                }
            }
        }

        void SpawnBlock(BlockEntry entry) 
        {
            GameObject prefabToSpawn = null;
            
            // İsimleri manuel girmek yerine direkt koda gömdük.
            switch (entry.type) 
            {
                case "Arrow": prefabToSpawn = prefab_Arrow; break;
                case "TNT": prefabToSpawn = prefab_TNT; break;
                case "SteelWall": prefabToSpawn = prefab_SteelWall; break;
                case "Glass": prefabToSpawn = prefab_Glass; break;
                case "Prism": prefabToSpawn = prefab_Prism; break;
                case "Mirror_NE_SW": prefabToSpawn = prefab_Mirror_NE_SW; break;
                case "Mirror_SW_NE": prefabToSpawn = prefab_Mirror_SW_NE; break;
                case "Goal": prefabToSpawn = prefab_Goal; break;
                default: 
                    Debug.LogWarning("Bilinmeyen blok tipi, prefab atanamadı: " + entry.type); 
                    break;
            }

            if (prefabToSpawn != null) 
            {
                // CRİTİCAL FIX: Objelerin merkezde kalmaması için, koordinatı arkadaşının 
                // GridManager içindeki formülünden alıyoruz! Böylece 100% oturur.
                Vector3 position = GridManager.Instance.GetWorldPosition(new Vector2Int(entry.x, entry.y));
                
                GameObject newBlock = Instantiate(prefabToSpawn, position, Quaternion.Euler(0, 0, entry.rotation));
                newBlock.name = entry.type + "_" + entry.x + "_" + entry.y;
                newBlock.transform.SetParent(this.transform);
            }
        }
    }
}