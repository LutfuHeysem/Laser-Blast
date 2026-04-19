using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace VectorFlow.Managers
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }

        [Header("Master Container")]
        public GameObject levelSelectUpperMenu; // Tüm panellerin ve EXIT butonunun içinde olduğu ana obje

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
            // Sahne her açıldığında bu sahnede bulunan MenuManager aktif olur.
            Instance = this;
        }

        private void Start()
        {
            // Başlangıçta tüm menüleri kapat, sadece Splash'ı aç
            CloseAllPanels();
            
            // Eğer oyundan (LevelScene) dönüyorsak Splash'ı atla
            if (UIManager.goToLevelSelect)
            {
                UIManager.goToLevelSelect = false;
                ShowLevelSelect();
            }
            else
            {
                if (splashScreenPanel != null) splashScreenPanel.SetActive(true);
                StartCoroutine(SplashRoutine());
            }
        }

        private IEnumerator SplashRoutine()
        {
            // İlk açılışta logolu loading ekranı efekti için 2 saniye bekle
            yield return new WaitForSeconds(2f);
            ShowMainMenu();
        }

        private void CloseAllPanels()
        {
            // Ana kapsayıcı açık kalmalı ki içindeki butonlar (EXIT gibi) çalışmaya devam etsin
            if (levelSelectUpperMenu != null) levelSelectUpperMenu.SetActive(true);

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
            if (levelButtonPrefab == null || levelSelectContainer == null) 
            {
                Debug.LogError("[MenuManager] HATA: levelButtonPrefab veya levelSelectContainer atanmamış!");
                return;
            }

            // Önceki butonları temizle
            foreach (Transform child in levelSelectContainer)
            {
                Destroy(child.gameObject);
            }

            // Resources/Levels klasöründeki bölümleri bul
            TextAsset[] availableLevels = Resources.LoadAll<TextAsset>("Levels");
            Debug.Log($"[MenuManager] Resources/Levels klasöründe {availableLevels.Length} adet bölüm bulundu.");
            
            if (availableLevels.Length == 0)
            {
                Debug.LogWarning("[MenuManager] UYARI: Hiç bölüm bulunamadı! Klasör ismini ve dosyaları kontrol edin.");
                return;
            }

            // Bölümleri adına göre düzgün sırala
            System.Array.Sort(availableLevels, (a, b) => 
            {
                int numA = ExtractNumber(a.name);
                int numB = ExtractNumber(b.name);
                if (numA == numB) return a.name.CompareTo(b.name);
                return numA.CompareTo(numB);
            });

            int unlockedLevel = SaveManager.GetUnlockedLevel();

            for (int i = 0; i < availableLevels.Length; i++)
            {
                int levelNum = i + 1; // 1 tabanlı level numarası
                GameObject newButtonObj = Instantiate(levelButtonPrefab, levelSelectContainer);
                VectorFlow.UI.LevelButtonUI buttonUI = newButtonObj.GetComponent<VectorFlow.UI.LevelButtonUI>();
                
                if (buttonUI != null)
                {
                    bool isUnlocked = (levelNum <= unlockedLevel);
                    int stars = SaveManager.GetLevelStars(levelNum);
                    int score = SaveManager.GetLevelScore(levelNum);
                    
                    buttonUI.Setup(levelNum, isUnlocked, stars, score);

                    // Butona tıklama özelliğini ekle
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

        public void ExitGame()
        {
            Debug.Log("Oyundan çıkılıyor...");
            Application.Quit();
        }
    }
}
