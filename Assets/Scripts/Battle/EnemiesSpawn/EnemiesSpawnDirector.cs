using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Character;
using Cysharp.Threading.Tasks;
using Globals;
using Pooling;
using UnityEngine;

namespace Battle.EnemiesSpawn
{
    public class EnemiesSpawnDirector : MonoBehaviour
    {
        [SerializeField] private EnemiesSpawnConfig _enemiesSpawnConfig;
        [SerializeField] private Camera _playerViewCamera;

        private string PoolName => name + "CharactersPool";
        
        private CancellationTokenSource _spawningCts;
        private System.Random _random;
        private Transform _enemiesFollowTarget;
        private readonly HashSet<BaseCharacter> _spawnedEnemies = new();

        private bool _isInitialized;

        public event Action<BaseCharacter> EnemyDied;

        public void Init(Transform enemiesFollowTarget, Camera playerViewCamera)
        {
            // TODO: Rewrite pooling
            SimplePool.CreatePool(PoolName);
            
            _playerViewCamera = playerViewCamera;
            _enemiesFollowTarget = enemiesFollowTarget;
            _random = new System.Random();

            _isInitialized = true;
        }
        
        public void EnableSpawn(bool enable)
        {
            if(!_isInitialized)
                Debug.LogError($"{nameof(EnableSpawn)} can be called only after {nameof(Init)} was called.");
            
            _spawningCts?.Cancel();

            if (!enable) return;
            
            _spawningCts = new CancellationTokenSource();
            SpawningProcess(_spawningCts.Token).Forget();
        }

        public void ClearEnemies()
        {
            foreach (var activeEnemy in _spawnedEnemies.ToList()) 
                ReleaseCharacter(activeEnemy);

            _spawnedEnemies.Clear();
        }

        private async UniTaskVoid SpawningProcess(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var delay = _random.Next(
                    _enemiesSpawnConfig.SpawnSettings.MinSpawnDelayMillis,
                    _enemiesSpawnConfig.SpawnSettings.MaxSpawnDelayMillis + 1
                );

                if (delay <= 0)
                    delay = 100;

                await UniTask.Delay(delay, DelayType.DeltaTime, PlayerLoopTiming.FixedUpdate, cancellationToken);
            
                if(cancellationToken.IsCancellationRequested)
                    return;

                // Don't spawn if reached max enemies limit
                if(_spawnedEnemies.Count >= _enemiesSpawnConfig.SpawnSettings.MaxEnemiesCount)
                    continue;

                // Don't spawn if there is no valid spawn point
                if (!TryGetSpawnPoint(out var spawnPoint)) 
                    continue;
                
                var enemiesList = _enemiesSpawnConfig.SpawnSettings.PossibleEnemiesPrefabs;
                var enemyToSpawnPrefab = enemiesList[_random.Next(enemiesList.Count)];
                
                var spawnedEnemy = SimplePool.Get(enemyToSpawnPrefab, transform, PoolName);
                _spawnedEnemies.Add(spawnedEnemy);

                spawnedEnemy.Reinit();
                
                spawnedEnemy.transform.position = 
                    new Vector3(spawnPoint.x, spawnPoint.y + spawnedEnemy.Height, spawnPoint.z);
                
                spawnedEnemy.Died += OnEnemyDied;
                
                if (spawnedEnemy is IFollowing follower)
                {
                    follower.FollowTarget = _enemiesFollowTarget;
                }
            }
        }

        private void OnEnemyDied(BaseCharacter enemy)
        {
            EnemyDied?.Invoke(enemy);
            ReleaseCharacter(enemy);
        }

        private void ReleaseCharacter(BaseCharacter character)
        {
            character.Died -= OnEnemyDied;

            _spawnedEnemies.Remove(character);
            SimplePool.Return(character, PoolName);
        }

        private void OnDestroy()
        {
            EnemyDied = null;
        }

        private bool TryGetSpawnPoint(out Vector3 point)
        {
            // TODO: Calculate from a character wight somehow?
            const float gapFromBound = 0.1f;

            float spawnPointY, spawnPointX;
            
            var spawnSide = (SpawnSide) _random.Next(0, 4);
            switch (spawnSide)
            {
                case SpawnSide.Bottom:
                    spawnPointY = -gapFromBound;
                    spawnPointX = (float) _random.NextDouble();
                    break;
                case SpawnSide.Top:
                    spawnPointY = 1 + gapFromBound;
                    spawnPointX = (float) _random.NextDouble();
                    break;
                case SpawnSide.Left:
                    spawnPointY = (float) _random.NextDouble();
                    spawnPointX = -gapFromBound;;
                    break;
                case SpawnSide.Right:
                    spawnPointY = (float) _random.NextDouble();
                    spawnPointX = 1 + gapFromBound;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var ray = _playerViewCamera.ViewportPointToRay(new Vector3(spawnPointX, spawnPointY, 
                _playerViewCamera.nearClipPlane));
            
            if (Physics.Raycast(ray, out var hitInfo, 
                    _playerViewCamera.farClipPlane, 1 << LayerMaskConstants.GroundLayer))
            {
                point = hitInfo.point;
                return true;
            }
            
            point = Vector3.zero;
            return false;
        }

        private enum SpawnSide
        {
            Bottom = 0,
            Top = 1,
            Left = 2,
            Right = 3
        }
    }
}