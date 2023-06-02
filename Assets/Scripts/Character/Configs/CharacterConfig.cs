using System;
using Globals;
using UnityEngine;

namespace Character.Configs
{
    [CreateAssetMenu(menuName = ProjectConstants.ScriptableObjectsAssetMenuName + "/Create new " + nameof(CharacterConfig))]
    public class CharacterConfig : ScriptableObject
    {
        [SerializeField] private CharacterStats _characterStats;

        public CharacterStats CharacterStats => _characterStats;
    }
    
    [Serializable]
    public struct CharacterStats
    {
        public Faction Faction;
        public int Health;
        
        public float MoveSpeed;
        public float MaxMoveSpeed;
        
        /// <summary>
        /// Duration of the invincibility after damage was taken.
        /// </summary>
        public int InvincibilityOnDamageMillis;
    }
}