using JetBrains.Annotations;
using UnityEngine;
using Weapons;

namespace Character.Enemy
{
    public class SimpleGroundEnemy : BaseCharacter, IFollowing
    {
        [SerializeField] private BaseWeaponReference _weaponReference;
        
        public BaseWeapon Weapon { get; private set; }

        [CanBeNull] public Transform FollowTarget { get; set; }

        private SimpleGroundEnemyAI _simpleGroundEnemyAI;

        private void Start()
        {
            Weapon = _weaponReference.CreateWeaponInstance();
            Weapon.SetOwner(this);
            
            _simpleGroundEnemyAI = new SimpleGroundEnemyAI(this);
        }

        private void FixedUpdate()
        {
            _simpleGroundEnemyAI.OnTurn();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent<IDamageTaker>(out var damageTaker)) 
                return;
            
            _simpleGroundEnemyAI.OnAroundDamageTaker(damageTaker);
        }

        protected override void InheritOnDestroy()
        {
            base.InheritOnDestroy();
            Weapon.Dispose();
        }
    }
}