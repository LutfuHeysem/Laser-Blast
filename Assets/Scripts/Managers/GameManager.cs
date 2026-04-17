using UnityEngine;
using VectorFlow.Core;
using System;

namespace VectorFlow.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }
        public int CurrentEnergy { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<int> OnEnergyChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartLevel(1); // Default test
        }

        public void StartLevel(int startingEnergy)
        {
            CurrentEnergy = startingEnergy;
            ChangeState(GameState.Idle);
            OnEnergyChanged?.Invoke(CurrentEnergy);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] State changed to: {newState}");
        }

        public bool TryConsumeEnergy()
        {
            if (CurrentEnergy > 0)
            {
                CurrentEnergy--;
                OnEnergyChanged?.Invoke(CurrentEnergy);
                return true;
            }
            return false;
        }
    }
}
