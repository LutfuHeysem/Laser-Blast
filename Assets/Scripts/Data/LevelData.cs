using UnityEngine;
using VectorFlow.Core;

namespace VectorFlow.Data
{
    [System.Serializable]
    public class CellData
    {
        public CellType type;
    }

    [CreateAssetMenu(fileName = "New Level", menuName = "VectorFlow/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string levelName;
        public int startingEnergy = 1;
        public int rows = 8;
        public int cols = 6;
        
        // Flattened 2D array representation for Inspector mapping
        public CellType[] gridCells = new CellType[48]; 

        public CellType GetCell(int x, int y)
        {
            if (x < 0 || x >= cols || y < 0 || y >= rows) return CellType.SteelWall;
            return gridCells[y * cols + x];
        }

        public void SetCell(int x, int y, CellType type)
        {
            if (x < 0 || x >= cols || y < 0 || y >= rows) return;
            gridCells[y * cols + x] = type;
        }
    }
}
