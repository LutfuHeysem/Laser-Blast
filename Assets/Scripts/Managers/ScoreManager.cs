using UnityEngine;

namespace VectorFlow.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int CurrentScore { get; private set; }
        public int ComboMultiplier { get; private set; }

        [Header("Görsel Efektler")]
        [Tooltip("Ekranda çıkacak +Puan yazısının Prefab'ı")]
        public GameObject floatingTextPrefab; 

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void InitializeScore()
        {
            CurrentScore = 0;
            ComboMultiplier = 1;
        }

        public void AddScore(int amount, Vector3 position)
        {
            int finalAmount = amount * ComboMultiplier;
            CurrentScore += finalAmount;
            
            // Eğer prefab atandıysa, puan yazısını sahnede yarat
            if (floatingTextPrefab != null)
            {
                // Rastgele hafif sağa/sola kaydırma
                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0);
                
                GameObject textObj = Instantiate(floatingTextPrefab, position + randomOffset, Quaternion.identity);
                textObj.GetComponent<FloatingText>().Setup(finalAmount);
            }

            Debug.Log($"[ScoreManager] Added {finalAmount} score. Total: {CurrentScore}");
        }
        
        public void IncrementCombo(Vector3 hitPosition)
        {
            ComboMultiplier++;
            Debug.Log($"[ScoreManager] Combo Increased! Current Multiplier: x{ComboMultiplier}");
        }

        public void ResetCombo()
        {
            ComboMultiplier = 1;
        }

        // BENİM EKSİK BIRAKTIĞIM YERLERİN DÜZELTİLMİŞ HALİ:
        public void CalculateFinalScore(int remainingEnergy)
        {
            int energyBonus = remainingEnergy * 500;
            if (energyBonus > 0)
            {
                CurrentScore += energyBonus;
                Debug.Log($"[ScoreManager] Energy Bonus Added: {energyBonus}");
            }
        }

        public int CalculateStars()
        {
            if (CurrentScore > 3000) return 3;
            if (CurrentScore > 1500) return 2;
            return 1;
        }
    }
}