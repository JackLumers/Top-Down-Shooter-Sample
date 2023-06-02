using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;

        public event Action RestartButtonClicked;
        
        private void Awake()
        {
            _restartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
        }

        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }

        private void OnDestroy()
        {
            RestartButtonClicked = null;
        }
    }
}