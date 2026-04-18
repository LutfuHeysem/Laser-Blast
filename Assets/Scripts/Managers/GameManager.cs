using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VectorFlow.Managers
{
    public enum GameState
    {
        Playing,      // Oyuncu hamle yapabilir
        Animating,    // Lazer animasyon halinde
        LevelComplete,// Bölüm başarıyla bitirildi
        GameOver      // Enerji bitti ve hedefe ulaşılamadı
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }
        public int CurrentEnergy { get; private set; }

        // Aktif lazerleri takip etmek için
        private HashSet<LaserEmitter> activeLasers = new HashSet<LaserEmitter>();

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

        public void InitializeGame(int startingEnergy)
        {
            CurrentEnergy = startingEnergy;
            ChangeState(GameState.Playing);
            activeLasers.Clear();
            Debug.Log($"[GameManager] Game Initialized. Starting Energy: {CurrentEnergy}");
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"[GameManager] State changed to: {CurrentState}");

            if (newState == GameState.GameOver)
            {
                Debug.Log("[GameManager] GAME OVER! Enerji bitti.");
                if (UIManager.Instance != null) UIManager.Instance.ShowGameOver();
            }
            else if (newState == GameState.LevelComplete)
            {
                Debug.Log($"[GameManager] LEVEL COMPLETE! Kalan Enerji: {CurrentEnergy}");
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.CalculateFinalScore(CurrentEnergy);
                    int stars = ScoreManager.Instance.CalculateStars();
                    
                    // Puanı ve yıldızları kaydet
                    int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
                    SaveManager.SaveLevelScoreAndStars(currentLevel, ScoreManager.Instance.CurrentScore, stars);
                    
                    // Bir sonraki bölümün kilidini aç
                    SaveManager.UnlockNextLevel(currentLevel);

                    if (UIManager.Instance != null) UIManager.Instance.ShowLevelComplete(ScoreManager.Instance.CurrentScore, stars);
                }
            }
        }

        // Oyuncu bir silaha tıkladığında çağrılır
        public bool TryConsumeEnergy()
        {
            if (CurrentState != GameState.Playing) return false;

            if (CurrentEnergy > 0)
            {
                CurrentEnergy--;
                Debug.Log($"[GameManager] Enerji kullanıldı. Kalan Enerji: {CurrentEnergy}");
                // TODO: UIManager.UpdateEnergy();
                return true;
            }

            Debug.LogWarning("[GameManager] Yeterli enerji yok!");
            return false;
        }

        public void RegisterActiveLaser(LaserEmitter laser)
        {
            activeLasers.Add(laser);
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Animating);
            }
        }

        public void UnregisterActiveLaser(LaserEmitter laser)
        {
            activeLasers.Remove(laser);
            
            // Tüm lazerler bittiğinde
            if (activeLasers.Count == 0 && CurrentState == GameState.Animating)
            {
                CheckWinLoseCondition();
            }
        }

        private void CheckWinLoseCondition()
        {
            // Eğer oyun zaten kazanıldıysa (hedefe ulaşıldıysa) tekrar kontrol etme
            if (CurrentState == GameState.LevelComplete) return;

            // Lazerler durdu ama hedefe ulaşılamadı. Enerji bitti mi?
            if (CurrentEnergy <= 0)
            {
                ChangeState(GameState.GameOver);
            }
            else
            {
                // Daha enerji var, oyuncu yeni hamle yapabilir
                ChangeState(GameState.Playing);
            }
        }

        public void WinLevel()
        {
            if (CurrentState == GameState.Playing || CurrentState == GameState.Animating)
            {
                ChangeState(GameState.LevelComplete);
            }
        }
    }
}
