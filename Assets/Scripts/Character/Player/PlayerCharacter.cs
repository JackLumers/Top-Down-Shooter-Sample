using System.Collections.Generic;
using ReferenceVariables;
using UnityEngine;
using Weapons;

namespace Character.Player
{
    public class PlayerCharacter : BaseCharacter
    {
        [SerializeField] private IntVariable _healthVariable;
        [SerializeField] private Transform _lookingDirectionMark;

        [SerializeField]
        private List<BaseWeaponReference> _weaponReferences = new List<BaseWeaponReference>();
        
        private int _selectedWeaponIndex = 0;
        private Transform _transform;
        private readonly List<BaseWeapon> _weapons = new();

        public Vector3 LookingPoint { get; private set; }

        private void Start()
        {
            _transform = transform;

            foreach (var weaponReference in _weaponReferences)
            {
                var weapon = weaponReference.CreateWeaponInstance();
                weapon.SetOwner(this);
                _weapons.Add(weapon);
            }

            _healthVariable.Value = CharacterStats.Health;
        }

        public void SetLookingPoint(Vector3 lookingPoint)
        {
            LookingPoint = lookingPoint;
            _lookingDirectionMark.LookAt(lookingPoint);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            _healthVariable.Value = CharacterStats.Health;
        }

        public void FireSelectedWeapon()
        {
            var fireDirection = -(_transform.position - LookingPoint).normalized;
            
            _weapons[_selectedWeaponIndex].TryFire(fireDirection);
        }

        public void SwitchWeapon()
        {
            if (_selectedWeaponIndex + 1 >= _weapons.Count)
                _selectedWeaponIndex--;
            else
                _selectedWeaponIndex++;
        }

        protected override void InheritOnDestroy()
        {
            base.InheritOnDestroy();
            foreach (var weapon in _weapons)
            {
                weapon.Dispose();
            }
        }
    }
}