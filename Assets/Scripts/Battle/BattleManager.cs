using Battle.EnemiesSpawn;
using Character;
using Character.Player;
using Cinemachine;
using ReferenceVariables;
using UI;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private IntVariable _playerHealthVariable;
        [SerializeField] private IntVariable _scoreVariable;
        
        [SerializeField] private CinemachineVirtualCamera _playerViewCamera;
        [SerializeField] private EnemiesSpawnDirector _enemiesSpawnDirector;
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private GameUiController _gameUiController;
        
        private PlayerCharacter _playerCharacterInstance;
        private Transform _playerCharacterTransform;
        
        public void Awake()
        {
            _playerCharacterInstance = Instantiate(_playerCharacterPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
            _playerCharacterTransform = _playerCharacterInstance.transform;
            
            _playerViewCamera.Follow = _playerCharacterTransform;
            _playerViewCamera.LookAt = _playerCharacterTransform;
            _playerViewCamera.gameObject.SetActive(true);

            _enemiesSpawnDirector.Init(_playerCharacterInstance.transform, Camera.main);
            _enemiesSpawnDirector.EnableSpawn(true);

            _scoreVariable.Value = 0;
            _playerHealthVariable.Changed.AddListener(OnPlayerHealthChanged);
            
            _gameUiController.ShowGameOverScreen(false);
            _gameUiController.ShowGameStatsScreen(true);
            
            _gameUiController.RestartButtonClicked += OnRestartClicked;
            _enemiesSpawnDirector.EnemyDied += OnEnemyDied;
        }

        private void OnEnemyDied(BaseCharacter enemy)
        {
            _scoreVariable.Value++;
        }

        private void OnPlayerHealthChanged(int newValue)
        {
            if (newValue > 0) return;
            
            GameOver();
        }
        
        private void OnRestartClicked()
        {
            _scoreVariable.Value = 0;

            _gameUiController.ShowGameOverScreen(false);
            _gameUiController.ShowGameStatsScreen(true);

            _playerCharacterTransform.position = _playerSpawnPoint.position;
            _playerCharacterInstance.gameObject.SetActive(true);
            _playerCharacterInstance.Reinit();

            _enemiesSpawnDirector.ClearEnemies();
            _enemiesSpawnDirector.EnableSpawn(true);
        }

        private void GameOver()
        {
            _gameUiController.ShowGameOverScreen(true);
            _gameUiController.ShowGameStatsScreen(false);
            
            _playerCharacterInstance.gameObject.SetActive(false);
            
            _enemiesSpawnDirector.EnableSpawn(false);
        }

        private void OnDestroy()
        {
            _enemiesSpawnDirector.EnemyDied -= OnEnemyDied;
            _gameUiController.RestartButtonClicked -= OnRestartClicked;
        }
    }
}