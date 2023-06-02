using System;
using System.Collections.Generic;
using Character;
using Globals;
using UnityEngine;

namespace Battle.EnemiesSpawn
{
    [CreateAssetMenu(menuName = ProjectConstants.ScriptableObjectsAssetMenuName + "/Create new " + nameof(EnemiesSpawnConfig))]
    public class EnemiesSpawnConfig : ScriptableObject
    {
        [SerializeField] private SpawnSettings _spawnSettings;

        public SpawnSettings SpawnSettings => _spawnSettings;
    }
    
    [Serializable]
    public struct SpawnSettings
    {
        public int MinSpawnDelayMillis;
        public int MaxSpawnDelayMillis;
        public int MaxEnemiesCount;
        public List<BaseCharacter> PossibleEnemiesPrefabs;
    }
}