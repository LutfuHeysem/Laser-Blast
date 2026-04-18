using System;
using System.Collections.Generic;
using UnityEngine;
using VectorFlow.Core; // Arkadaşının CellType enum'ını okumak için

namespace VectorFlow.Data
{
    [Serializable]
    public class BlockEntry
    {
        public int x;
        public int y;
        public string type;
        public float rotation;
    }

    [Serializable]
    public class LevelData
    {
        public string levelName;
        public int cols;
        public int rows;
        public int startingEnergy = 5;
        public List<BlockEntry> blocks;

        // GridManager'ın çağırdığı, "Bu x ve y'de ne var?" sorusuna cevap veren fonksiyon
        public CellType GetCell(int x, int y)
        {
            if (blocks == null) return default(CellType);

            foreach (var block in blocks)
            {
                if (block.x == x && block.y == y)
                {
                    // JSON'daki yazıyı (örn: "TNT") arkadaşının CellType enum'ına çevirir
                    if (Enum.TryParse(block.type, true, out CellType cellType))
                    {
                        return cellType;
                    }
                }
            }
            
            return default(CellType); 
        }
    }
}