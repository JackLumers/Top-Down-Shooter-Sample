using Character;
using Pooling;
using UnityEngine;

namespace Weapons.Ranged.SniperRifle
{
    public class SniperRifleWeapon : BaseRangedWeapon
    {
        private string PoolName => WeaponReference.name + "DamageZones";

        public SniperRifleWeapon(BaseWeaponReference weaponReference, WeaponSettings weaponSettings,
            RangedWeaponSettings rangedWeaponSettings)
            : base(weaponReference, weaponSettings, rangedWeaponSettings)
        {
            // TODO: Rewrite pooling
            SimplePool.CreatePool(PoolName);
        }

        protected override void OnFire(Vector3 direction)
        {
            var damageZone = SimplePool.Get(WeaponSettings.DamageZonePrefab, null, PoolName);
            damageZone.Init(WeaponSettings.Damage, Owner.Faction);

            var ownerPosition = OwnerTransform.position;
            var shotRelativePosition =
                ownerPosition + direction * RangedWeaponSettings.MaxShotDistance;
            
            damageZone.transform.position = ownerPosition;
            damageZone.Affected += OnDamageZoneAffected;

            damageZone.MoveTo(shotRelativePosition, RangedWeaponSettings.ProjectileSpeed, ReleaseDamageZone);
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