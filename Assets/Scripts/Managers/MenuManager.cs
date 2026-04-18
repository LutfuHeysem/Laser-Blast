using UnityEngine;
using System.Collections;

namespace VectorFlow.Managers
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }

        [Header("Menu Panels")]
        public GameObject splashScreenPanel;
        public GameObject mainMenuPanel;
        public GameObject levelSelectPanel;
        public GameObject scoreboardPanel;
        
        [Header("In-Game Panel Group")]
        public GameObject inGameUIPanel; // UIManager'ın yönettiği oyun içi UI grubu

        public int CurrentLevelIndex { get; private set; } = 1;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            // Başlangıçta tüm menüleri kapat, sadece Splash'ı aç
            CloseAllPanels();
            if (splashScreenPanel != null) splashScreenPanel.SetActive(true);

            StartCoroutine(SplashRoutine());
        }

        private IEnumerator SplashRoutine()
        {
            // Logolu loading ekranı efekti için 2 saniye bekle
            yield return new WaitForSeconds(2f);
            ShowMainMenu();
        }

        private void CloseAllPanels()
        {
            if (splashScreenPanel != null) splashScreenPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (scoreboardPanel != null) scoreboardPanel.SetActive(false);
            if (inGameUIPanel != null) inGameUIPanel.SetActive(false);
        }

        public void ShowMainMenu()
        {
            CloseAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            
            // Eğer Main Menu'ye döndüysek, arkadaki leveli temizleyebiliriz
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ClearCurrentLevel();
            }
        }

        public void ShowLevelSelect()
        {
            CloseAllPanels();
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
            
            // TODO: Level butonlarını dinamik olarak oluştur veya güncelle
        }

        public void ShowScoreboard()
        {
            CloseAllPanels();
            if (scoreboardPanel != null) scoreboardPanel.SetActive(true);
        }

        // Oyunu Başlatma (Level Select'ten bir butona basıldığında çağrılır)
        public void StartLevel(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            CloseAllPanels();

            // Oyun içi UI'ı aktif et
            if (inGameUIPanel != null) inGameUIPanel.SetActive(true);

            // Bölümü yükle
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadLevelByIndex(levelIndex);
            }
        }

        // Next Level (Level Complete ekranından çağrılır)
        public void LoadNextLevel()
        {
            int totalLevels = LevelManager.Instance != null ? LevelManager.Instance.GetTotalLevelsCount() : 0;
            
            if (CurrentLevelIndex < totalLevels)
            {
                StartLevel(CurrentLevelIndex + 1);
            }
            else
            {
                Debug.Log("Oyun bitti! Son bölümü de geçtiniz.");
                ShowMainMenu();
            }
        }

        // Retry (Game Over ekranından çağrılır)
        public void RetryCurrentLevel()
        {
            StartLevel(CurrentLevelIndex);
        }
    }
}
