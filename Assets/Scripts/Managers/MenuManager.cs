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

        [Header("Level Select Settings")]
        public GameObject levelButtonPrefab; // LevelButtonUI componenti olan prefab
        public Transform levelSelectContainer; // Grid Layout Group'un olduğu panel
        private bool levelsPopulated = false;

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

            // Her açıldığında butonları tekrar üret/güncelle
            PopulateLevelSelect();
        }

        private void PopulateLevelSelect()
        {
            if (levelButtonPrefab == null || levelSelectContainer == null) return;

            // Önceki butonları temizle (isteğe bağlı, ama veriler değişmiş olabileceği için temizlemek iyi)
            foreach (Transform child in levelSelectContainer)
            {
                Destroy(child.gameObject);
            }

            // Resources klasöründeki bölümleri bul ve sayısına göre buton üret
            TextAsset[] availableLevels = Resources.LoadAll<TextAsset>("Levels");
            
            // Bölümleri adına göre (Level1, Level2) düzgün sırala
            System.Array.Sort(availableLevels, (a, b) => 
            {
                int numA = ExtractNumber(a.name);
                int numB = ExtractNumber(b.name);
                if (numA == numB) return a.name.CompareTo(b.name);
                return numA.CompareTo(numB);
            });

            int totalLevels = availableLevels.Length;
            int unlockedLevel = SaveManager.GetUnlockedLevel();

            for (int i = 1; i <= totalLevels; i++)
            {
                GameObject newButtonObj = Instantiate(levelButtonPrefab, levelSelectContainer);
                VectorFlow.UI.LevelButtonUI buttonUI = newButtonObj.GetComponent<VectorFlow.UI.LevelButtonUI>();
                
                if (buttonUI != null)
                {
                    bool isUnlocked = (i <= unlockedLevel);
                    int stars = SaveManager.GetLevelStars(i);
                    int score = SaveManager.GetLevelScore(i);
                    
                    buttonUI.Setup(i, isUnlocked, stars, score);

                    // Butona tıklama özelliğini koddan ekle
                    buttonUI.buttonComponent.onClick.AddListener(() => buttonUI.OnClickButton());
                }
            }
        }

        private int ExtractNumber(string name)
        {
            string numberString = System.Text.RegularExpressions.Regex.Match(name, @"\d+").Value;
            if (int.TryParse(numberString, out int num)) return num;
            return 999;
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
