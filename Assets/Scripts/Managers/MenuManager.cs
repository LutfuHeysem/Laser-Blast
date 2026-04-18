using UnityEngine;
using UnityEngine.SceneManagement;
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
        }

        public void ShowMainMenu()
        {
            CloseAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void ShowLevelSelect()
        {
            CloseAllPanels();
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        }

        public void ShowScoreboard()
        {
            CloseAllPanels();
            if (scoreboardPanel != null) scoreboardPanel.SetActive(true);
        }

        // Oyunu Başlatma (Level Select'ten bir butona basıldığında çağrılır)
        public void StartLevel(int levelIndex)
        {
            // Seçilen bölümü hafızaya kaydet
            PlayerPrefs.SetInt("SelectedLevel", levelIndex);
            PlayerPrefs.Save();

            // Level sahnesini yükle
            SceneManager.LoadScene("LevelScene");
        }
    }
}
