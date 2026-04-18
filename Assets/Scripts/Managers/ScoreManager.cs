using UnityEngine;

namespace VectorFlow.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int CurrentScore { get; private set; }
        public int ComboMultiplier { get; private set; }

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

        public void InitializeScore()
        {
            CurrentScore = 0;
            ComboMultiplier = 1;
            Debug.Log("[ScoreManager] Score Initialized.");
        }

        public void AddScore(int amount)
        {
            int finalAmount = amount * ComboMultiplier;
            CurrentScore += finalAmount;
            Debug.Log($"[ScoreManager] Added {finalAmount} score (Base: {amount}, Multiplier: x{ComboMultiplier}). Total Score: {CurrentScore}");
            // TODO: UIManager.UpdateScoreText();
        }

        public void IncrementCombo()
        {
            ComboMultiplier++;
            Debug.Log($"[ScoreManager] Combo Increased! Current Multiplier: x{ComboMultiplier}");
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowFloatingText($"COMBO x{ComboMultiplier}!", Vector3.zero); // Daha sonra vurulan silahın lokasyonu verilebilir
            }
        }

        public void ResetCombo()
        {
            ComboMultiplier = 1;
        }

        public void CalculateFinalScore(int remainingEnergy)
        {
            // Kalan enerji başına bonus puan
            int energyBonus = remainingEnergy * 500;
            if (energyBonus > 0)
            {
                CurrentScore += energyBonus;
                Debug.Log($"[ScoreManager] Energy Bonus Added: {energyBonus}");
            }
        }

        public int CalculateStars()
        {
            // Yıldız hesaplama mantığı (Örnek: Puan eşiklerine göre)
            if (CurrentScore > 3000) return 3;
            if (CurrentScore > 1500) return 2;
            return 1;
        }
    }
}
