using System;
using Character;
using Pooling;
using UnityEngine;
using Weapons.Ranged.SniperRifle;

namespace Weapons.Ranged.AutoRifle
{
    public class PumpShotgunWeapon : BaseRangedWeapon
    {
        private readonly System.Random _random;

        protected AutoRifleSettings AutoRifleSettings;

        private string PoolName => WeaponReference.name + "DamageZones";

        public PumpShotgunWeapon(BaseWeaponReference weaponReference, WeaponSettings weaponSettings,
            RangedWeaponSettings rangedWeaponSettings, AutoRifleSettings autoRifleSettings)
            : base(weaponReference, weaponSettings, rangedWeaponSettings)
        {
            AutoRifleSettings = autoRifleSettings;

            _random = new System.Random();

            // TODO: Rewrite pooling
            SimplePool.CreatePool(PoolName);
        }

        protected override void OnFire(Vector3 direction)
        {
            for (int i = 0; i < AutoRifleSettings.ShotProjectilesCount; i++)
            {
                var damageZone = SimplePool.Get(WeaponSettings.DamageZonePrefab, null, PoolName);
                damageZone.Init(WeaponSettings.Damage, Owner.Faction);

                var ownerPosition = OwnerTransform.position;
                var shotRelativePosition = ownerPosition + direction * RangedWeaponSettings.MaxShotDistance;

                var maxAngleOffset = AutoRifleSettings.DispersionAngle / 2;
                var randomAngleOffset = _random.NextDouble() * AutoRifleSettings.DispersionAngle - maxAngleOffset;

                var calcShotRelativePositionX = (float) (Math.Cos(Mathf.Deg2Rad * randomAngleOffset) * shotRelativePosition.x -
                                                         Math.Sin(Mathf.Deg2Rad * randomAngleOffset) * shotRelativePosition.z);
                
                var calcShotRelativePositionZ = (float) (Math.Sin(Mathf.Deg2Rad * randomAngleOffset) * shotRelativePosition.x +
                                                         Math.Cos(Mathf.Deg2Rad * randomAngleOffset) * shotRelativePosition.z);

                var calcShotRelativePosition = new Vector3(calcShotRelativePositionX, shotRelativePosition.y, calcShotRelativePositionZ);

                damageZone.transform.position = ownerPosition;
                damageZone.Affected += OnDamageZoneAffected;

                damageZone.MoveTo(calcShotRelativePosition, RangedWeaponSettings.ProjectileSpeed, ReleaseDamageZone);
            }
        }

        private void OnDamageZoneAffected(DamageZone damageZone, IDamageTaker damageTaker)
        {
            ReleaseDamageZone(damageZone);
        }

        private void ReleaseDamageZone(DamageZone damageZone)
        {
            damageZone.Affected -= OnDamageZoneAffected;
            damageZone.StopMove();
            SimplePool.Return(damageZone, PoolName);
        }
    }
}