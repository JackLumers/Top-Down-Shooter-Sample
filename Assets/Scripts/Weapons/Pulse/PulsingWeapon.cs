using System.Threading;
using Character;
using Cysharp.Threading.Tasks;
using Pooling;
using UnityEngine;

namespace Weapons.Pulse
{
    public class PulsingWeapon : BaseWeapon
    {
        private DamageZone _damageZone;

        private CancellationTokenSource _fireCts;
        private const int DamageZoneLifetimeMillis = 300;

        private string PoolName => WeaponReference.name + "DamageZones";
        
        public PulsingWeapon(BaseWeaponReference weaponReference, WeaponSettings weaponSettings) 
            : base(weaponReference, weaponSettings)
        {
            // TODO: Rewrite pooling
            SimplePool.CreatePool(PoolName);
        }

        ~PulsingWeapon()
        {
            _fireCts?.Dispose();
        }
        
        protected override void OnFire(Vector3 direction)
        {
            _fireCts?.Cancel();
            _fireCts = new CancellationTokenSource();
            
            FireProcess(_fireCts.Token).Forget();
        }
        
        private async UniTaskVoid FireProcess(CancellationToken cancellationToken)
        {
            _damageZone = SimplePool.Get(WeaponSettings.DamageZonePrefab, null, PoolName);
            
            _damageZone.transform.position = OwnerTransform.position;
            _damageZone.gameObject.SetActive(true);
            _damageZone.Init(WeaponSettings.Damage, Owner.Faction);
            _damageZone.Affected += OnDamageZoneAffected;

            await UniTask.Delay(DamageZoneLifetimeMillis, DelayType.DeltaTime, PlayerLoopTiming.FixedUpdate, cancellationToken)
                .SuppressCancellationThrow();

            ReleaseDamageZone(_damageZone);
        }
        
        private void OnDamageZoneAffected(DamageZone damageZone, IDamageTaker damageTaker)
        {
            _fireCts?.Cancel();
            ReleaseDamageZone(damageZone);
        }

        private void ReleaseDamageZone(DamageZone damageZone)
        {
            damageZone.Affected -= OnDamageZoneAffected;
            SimplePool.Return(damageZone, PoolName);
        }
    }
}