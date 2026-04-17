using UnityEngine;
using System.Collections.Generic;
using VectorFlow.Data; // LevelData'yı tanıyabilmek için ekledik

namespace VectorFlow.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Ayarlar")]
        public TextAsset levelJsonFile; 
        
        [Header("Prefab Eşleşmeleri")]
        public GameObject prefab_Arrow;
        public GameObject prefab_TNT;
        public GameObject prefab_SteelWall;
        public GameObject prefab_Glass;
        public GameObject prefab_Prism;
        public GameObject prefab_Mirror_NE_SW;
        public GameObject prefab_Mirror_SW_NE;
        public GameObject prefab_Goal;

        void Start() 
        {
            LoadLevel();
        }

        public void LoadLevel() 
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

                // 2. Sonra senin Prefablari diziyoruz
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