using UnityEngine;

namespace VectorFlow.Managers
{
    public static class SaveManager
    {
        private const string KEY_UNLOCKED_LEVEL = "UnlockedLevel";
        private const string KEY_LEVEL_SCORE_PREFIX = "LevelScore_";
        private const string KEY_LEVEL_STARS_PREFIX = "LevelStars_";

        // Kaçıncı seviyeye kadar açıldığını döndürür (Varsayılan 1)
        public static int GetUnlockedLevel()
        {
            return PlayerPrefs.GetInt(KEY_UNLOCKED_LEVEL, 1);
        }

        // Oyuncu yeni bir bölümü başarıyla geçerse, bir sonraki bölümü aç
        public static void UnlockNextLevel(int currentLevelIndex)
        {
            int unlockedLevel = GetUnlockedLevel();
            // Eğer bitirilen seviye en son açılan seviyeyse, bir sonrakini aç
            if (currentLevelIndex >= unlockedLevel)
            {
                PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, currentLevelIndex + 1);
                PlayerPrefs.Save();
            }
        }

        // Bir bölüm için puan kaydet (Sadece eskisinden yüksekse kaydeder)
        public static void SaveLevelScoreAndStars(int levelIndex, int newScore, int newStars)
        {
            int currentBestScore = GetLevelScore(levelIndex);
            if (newScore > currentBestScore)
            {
                PlayerPrefs.SetInt(KEY_LEVEL_SCORE_PREFIX + levelIndex, newScore);
                PlayerPrefs.SetInt(KEY_LEVEL_STARS_PREFIX + levelIndex, newStars);
                PlayerPrefs.Save();
            }
        }

        public static int GetLevelScore(int levelIndex)
        {
            return PlayerPrefs.GetInt(KEY_LEVEL_SCORE_PREFIX + levelIndex, 0);
        }

        public static int GetLevelStars(int levelIndex)
        {
            return PlayerPrefs.GetInt(KEY_LEVEL_STARS_PREFIX + levelIndex, 0);
        }

        // Toplam istatistikler (Scoreboard için)
        public static int GetTotalScore(int totalLevels)
        {
            int total = 0;
            for (int i = 1; i <= totalLevels; i++)
            {
                total += GetLevelScore(i);
            }
            return total;
        }

        public static int GetTotalStars(int totalLevels)
        {
            int total = 0;
            for (int i = 1; i <= totalLevels; i++)
            {
                total += GetLevelStars(i);
            }
            return total;
        }
        
        public static int GetCompletedLevelsCount(int totalLevels)
        {
            int count = 0;
            for (int i = 1; i <= totalLevels; i++)
            {
                if (GetLevelScore(i) > 0) count++;
            }
            return count;
        }

        // İlerlemeyi sıfırlamak için (Ayarlar menüsüne eklenebilir)
        public static void ResetProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
