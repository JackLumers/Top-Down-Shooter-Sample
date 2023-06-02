using System;
using System.Threading;
using Character;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public abstract class BaseWeapon : IDisposable
    {
        protected WeaponSettings WeaponSettings;
        protected BaseCharacter Owner;
        protected Transform OwnerTransform;
        protected BaseWeaponReference WeaponReference;
        
        private CancellationTokenSource _cooldownCts;
        public bool CanFire { get; private set; }

        protected BaseWeapon(BaseWeaponReference weaponReference, WeaponSettings weaponSettings)
        {
            WeaponReference = weaponReference;
            WeaponSettings = weaponSettings;
        }

        public virtual void SetOwner(BaseCharacter owner)
        {
            Owner = owner;
            OwnerTransform = owner.transform;
            CanFire = true;
        }

        public bool TryFire(Vector3 direction)
        {
            if(!CanFire)
                return false;
            
            OnFire(direction);
            
            _cooldownCts?.Cancel();
            _cooldownCts = new CancellationTokenSource();
            
            Cooldown(WeaponSettings.CooldownMillis, _cooldownCts.Token).Forget();
            return true;
        }
        
        protected abstract void OnFire(Vector3 direction);

        private async UniTaskVoid Cooldown(int cooldownMillis, CancellationToken cancellationToken)
        {
            CanFire = false;
            
            await UniTask.Delay(cooldownMillis, DelayType.DeltaTime, PlayerLoopTiming.TimeUpdate, cancellationToken)
                .SuppressCancellationThrow();

            CanFire = true;
        }
        
        public virtual void Dispose()
        {
            _cooldownCts?.Dispose();
        }
    }
}