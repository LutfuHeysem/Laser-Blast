using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VectorFlow.UI
{
    public class LevelButtonUI : MonoBehaviour
    {
        [Header("UI Referansları")]
        public TextMeshProUGUI levelNumberText;
        public TextMeshProUGUI scoreText;
        public Image[] starImages; // 3 adet yıldız resmi
        public Button buttonComponent;
        public Image lockImage; // Kilitliyse gösterilecek ikon

        [Header("Renkler")]
        public Color lockedColor = Color.gray;
        public Color unlockedColor = Color.white;
        public Color starActiveColor = Color.yellow;
        public Color starInactiveColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Soluk renk

        private int myLevelIndex;

        public void Setup(int levelIndex, bool isUnlocked, int stars, int score)
        {
            myLevelIndex = levelIndex;
            
            if (levelNumberText != null)
            {
                levelNumberText.text = "Bölüm " + levelIndex.ToString();
            }

            if (scoreText != null)
            {
                scoreText.text = isUnlocked && score > 0 ? $"Skor: {score}" : "";
            }

            // Kilit durumu ayarlamaları
            if (buttonComponent != null)
            {
                buttonComponent.interactable = isUnlocked;
                buttonComponent.image.color = isUnlocked ? unlockedColor : lockedColor;
            }

            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(!isUnlocked);
            }

            // Yıldızları ayarlama
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    // Eğer bölüm kilitliyse veya hiç oynanmamışsa yıldızları soluk göster veya gizle
                    if (!isUnlocked)
                    {
                        starImages[i].color = new Color(0, 0, 0, 0); // Tamamen görünmez yap
                        continue;
                    }

                    // i=0 (1.yıldız), i=1 (2.yıldız), i=2 (3.yıldız)
                    if (stars > i)
                    {
                        starImages[i].color = starActiveColor;
                    }
                    else
                    {
                        starImages[i].color = starInactiveColor;
                    }
                }
            }
        }

        // Buton tıklandığında Level'i yükle
        public void OnClickButton()
        {
            if (VectorFlow.Managers.MenuManager.Instance != null)
            {
                VectorFlow.Managers.MenuManager.Instance.StartLevel(myLevelIndex);
            }
        }
    }
}
