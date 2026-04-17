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
        public List<BlockMapping> blockMappings;

        [System.Serializable]
        public struct BlockMapping 
        {
            public string typeName; 
            public GameObject prefab; 
        }

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
            foreach (var map in blockMappings) 
            {
                if (map.typeName == entry.type) 
                {
                    prefabToSpawn = map.prefab;
                    break;
                }
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