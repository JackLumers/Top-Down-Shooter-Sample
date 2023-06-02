using JetBrains.Annotations;
using UnityEngine;

namespace Character.Enemy
{
    public class SimpleGroundEnemyAI
    {
        [CanBeNull] private Transform FollowTransform => _simpleGroundEnemy.FollowTarget;

        private readonly SimpleGroundEnemy _simpleGroundEnemy;
        private readonly Transform _enemyTransform;
        
        public SimpleGroundEnemyAI(SimpleGroundEnemy simpleGroundEnemy)
        {
            _simpleGroundEnemy = simpleGroundEnemy;
            _enemyTransform = simpleGroundEnemy.transform;
        }

        public void OnAroundDamageTaker(IDamageTaker damageTaker)
        {
            if(damageTaker.Faction == _simpleGroundEnemy.Faction || !_simpleGroundEnemy.Weapon.CanFire)
                return;
            
            _simpleGroundEnemy.Weapon.TryFire(Vector3.zero);
        }

        public void OnTurn()
        {
            if (!ReferenceEquals(FollowTransform, null))
            {
                _simpleGroundEnemy.MoveSelf(-(_enemyTransform.position - FollowTransform.position).normalized);
            }
        }
    }
}