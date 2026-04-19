using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro için

namespace VectorFlow.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD")]
        public TextMeshProUGUI energyText;
        public TextMeshProUGUI scoreText;

        [Header("Panels")]
        public GameObject levelCompletePanel;
        public TextMeshProUGUI levelCompleteScoreText;
        public GameObject[] stars; // 3 yıldız görseli
        
        public GameObject gameOverPanel;
        
        public static bool goToLevelSelect = false;

        [Header("Popups")]
        public GameObject floatingTextPrefab;
        public Canvas mainCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            // Başlangıçta paneller kapalı olsun
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void Update()
        {
            // HUD'u sürekli güncelle (ya da event bazlı da yapılabilir)
            if (GameManager.Instance != null && energyText != null)
            {
                energyText.text = $"{GameManager.Instance.CurrentEnergy}";
            }

            if (ScoreManager.Instance != null && scoreText != null)
            {
                scoreText.text = $"{ScoreManager.Instance.CurrentScore}";
            }
        }

        public void ShowLevelComplete(int finalScore, int starCount)
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                if (levelCompleteScoreText != null)
                {
                    levelCompleteScoreText.text = $"Score: {finalScore}";
                }

                // Yıldızları aç/kapat
                for (int i = 0; i < stars.Length; i++)
                {
                    if (stars[i] != null)
                    {
                        stars[i].SetActive(i < starCount);
                    }
                }
            }
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        public void ShowFloatingText(string text, Vector3 worldPosition)
        {
            if (floatingTextPrefab != null && mainCanvas != null)
            {
                // TODO: İleride DOTween gibi bir kütüphaneyle eklenecek animasyonlu popup
                Debug.Log($"[Floating Text] {text} at {worldPosition}");
            }
        }

        public void OnClickNextLevel()
        {
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            
            int current = PlayerPrefs.GetInt("SelectedLevel", 1);
            int total = LevelManager.Instance != null ? LevelManager.Instance.GetTotalLevelsCount() : 999;
            
            if (current < total)
            {
                PlayerPrefs.SetInt("SelectedLevel", current + 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene("LevelScene");
            }
            else
            {
                // Son level bittiyse menüye dön
                SceneManager.LoadScene("MenuScene");
            }
        }

        public void OnClickRetry()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            SceneManager.LoadScene("LevelScene");
        }

        public void OnClickMainMenu()
        {
            Time.timeScale = 1f; 
            goToLevelSelect = true; // Menü açıldığında direkt Level Select'e gitsin
            SceneManager.LoadScene("MenuScene");
            Debug.Log("[UIManager] Oyundan çıkıldı, tüm veriler temizlendi.");
        }
    }
}
