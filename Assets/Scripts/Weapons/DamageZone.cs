using System;
using System.Collections.Generic;
using System.Threading;
using Character;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// An object that is used by all weapons to affect.
    /// Targets to which weapon affects defined by <see cref="Faction"/>
    /// </summary>
    public class DamageZone : MonoBehaviour
    {
        private Faction _faction;
        private int _damage;

        private Transform _transform;
        private CancellationTokenSource _movingCts;
        
        public event Action<DamageZone, IDamageTaker> Affected;

        private void Awake()
        {
            _transform = transform;
        }

        public void Init(int damage, Faction faction)
        {
            _damage = damage;
            _faction = faction;
        }
        
        public void MoveTo(Vector3 target, float speed, Action<DamageZone> reached)
        {
            _movingCts?.Cancel();
            _movingCts = new CancellationTokenSource();
            
            MoveProcess(target, speed, reached, _movingCts.Token).Forget();
        }
        
        public void StopMove()
        {
            _movingCts?.Cancel();
        }
        
        private async UniTask MoveProcess(Vector3 target, float speed, Action<DamageZone> reached, CancellationToken cancellationToken)
        {
            var startPoint = _transform.position;
            float t = 0;
            
            while (t <= 1)
            {
                t += speed * Time.deltaTime;
                
                _transform.position = Vector3.Lerp(startPoint, target, t);

                await UniTask.DelayFrame(1, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                
                if(cancellationToken.IsCancellationRequested)
                    return;
            }
            
            reached?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageTaker>(out var damageTaker) && damageTaker.Faction != _faction)
            {
                damageTaker.TakeDamage(_damage);
                Affected?.Invoke(this, damageTaker);
            }
        }
        
        private void OnDestroy()
        {
            _movingCts?.Cancel();
            Affected = null;
        }
    }
}