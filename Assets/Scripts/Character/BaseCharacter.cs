using System;
using System.Collections.Generic;
using System.Threading;
using Character.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseCharacter : MonoBehaviour, IDamageTaker
    {
        [SerializeField] protected CharacterConfig _characterConfig;
        [SerializeField] private Transform _modelTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshRenderer _modelMeshRenderer;
        [SerializeField] private Collider _collider;

        private Rigidbody _rigidbody;
        private CharacterMovingController _characterMovingController;
        private CharacterAnimationController _characterAnimationController;
        
        private CancellationTokenSource _invincibilityStatusChangeCts;
        private readonly HashSet<string> _invincibilityFlags = new();
        private Faction _faction1;
        
        protected CharacterStats CharacterStats;

        public bool IsInvincible => _invincibilityFlags.Count > 0;
        
        public Faction Faction => CharacterStats.Faction;

        public float Height => _collider.bounds.size.y;

        public event Action<BaseCharacter> Died;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _characterMovingController = new CharacterMovingController(_rigidbody, _modelTransform);
            _characterAnimationController = new CharacterAnimationController(_animator, _modelMeshRenderer);
            
            Reinit();

            InheritAwake();
        }

        protected virtual void InheritAwake() { }

        /// <summary>
        /// Sets values to their default state, as if character was just spawned.
        /// Automatically called in <see cref="Awake"/>.
        /// </summary>
        public void Reinit()
        {
            _invincibilityStatusChangeCts?.Dispose();
            _invincibilityStatusChangeCts = null;
            _invincibilityFlags.Clear();
            _characterAnimationController.AnimateInvincibility(false);

            CharacterStats = _characterConfig.CharacterStats;
        }

        public void MoveSelf(Vector3 direction)
        {
            var b = direction.x;
            var a = direction.z;
            
            var rotationY = Mathf.Asin(a / Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2))) * 57.2957795131f;
            rotationY -= 90;
            
            if (b > 0)
            {
                rotationY *= -1;
            }

            var rotation = new Vector3(0, rotationY, 0);
            
            _characterMovingController.Rotate(rotation);
            
            _characterMovingController.Move(direction, CharacterStats.MoveSpeed, 
                ForceMode.VelocityChange, true, CharacterStats.MaxMoveSpeed);
            
            _characterAnimationController.AnimateMoving(true);
        }
        
        public virtual void TakeDamage(int damage)
        {
            if(IsInvincible) return;
            
            CharacterStats.Health -= damage;

            if (CharacterStats.Health <= 0)
                OnDied();
            else
                SetTemporarilyInvincible(nameof(TakeDamage), CharacterStats.InvincibilityOnDamageMillis);
        }
        
        private void OnDied()
        {
            // There could be animation
            gameObject.SetActive(false);
            
            Died?.Invoke(this);
        }
        
        private void SetTemporarilyInvincible(string context, int millis)
        {
            _invincibilityStatusChangeCts?.Cancel();
            
            if (millis > 0)
            {
                _invincibilityStatusChangeCts = new CancellationTokenSource();
                SetTempInvincibleProcess(_invincibilityStatusChangeCts.Token).Forget();
            }
            else
            {
                SetInvincibleByContext(context, false);
            }
            
            // Notice that if process is cancelled, player will be vulnerable again immediately.
            async UniTask SetTempInvincibleProcess(CancellationToken cancellationToken)
            {
                if(cancellationToken.IsCancellationRequested) 
                    return;

                SetInvincibleByContext(context, true);

                await UniTask.Delay(millis, DelayType.DeltaTime, PlayerLoopTiming.FixedUpdate, cancellationToken)
                    .SuppressCancellationThrow();

                SetInvincibleByContext(context, false);
            }
        }
        
        private void SetInvincibleByContext(string context, bool isInvincible)
        {
            if (isInvincible)
            {
                if (!_invincibilityFlags.Contains(context)) _invincibilityFlags.Add(context);
            }
            else
            {
                if (_invincibilityFlags.Contains(context)) _invincibilityFlags.Remove(context);
            }

            _characterAnimationController.AnimateInvincibility(isInvincible);
        }

        private void OnDestroy()
        {
            _invincibilityStatusChangeCts?.Dispose();
            _invincibilityStatusChangeCts = null;

            Died = null;

            InheritOnDestroy();
        }

        protected virtual void InheritOnDestroy() { }
    }
}