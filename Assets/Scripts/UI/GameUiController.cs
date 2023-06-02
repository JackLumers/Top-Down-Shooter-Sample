using System;
using UnityEngine;

namespace UI
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private GameObject _gameStatsScreen;
        
        public event Action RestartButtonClicked;
        
        private void Awake()
        {
            _gameOverScreen.RestartButtonClicked += OnRestartButtonClicked;
        }
        
        private void OnRestartButtonClicked()
        {
            RestartButtonClicked?.Invoke();
        }

        public void ShowGameOverScreen(bool show)
        {
            _gameOverScreen.Show(show);
        }

        public void ShowGameStatsScreen(bool show)
        {
            _gameStatsScreen.gameObject.SetActive(show);
        }

        private void OnDestroy()
        {
            RestartButtonClicked = null;
        }
    }
}