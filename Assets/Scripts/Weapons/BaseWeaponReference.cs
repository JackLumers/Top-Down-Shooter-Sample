using System;
using UnityEngine;

namespace Weapons
{
    public abstract class BaseWeaponReference : ScriptableObject
    {
        [SerializeField] protected WeaponSettings WeaponSettings;
        
        public abstract BaseWeapon CreateWeaponInstance();
    }
    
    [Serializable]
    public struct WeaponSettings
    {
        public int CooldownMillis;
        public int Damage;
        public DamageZone DamageZonePrefab;
    }
}