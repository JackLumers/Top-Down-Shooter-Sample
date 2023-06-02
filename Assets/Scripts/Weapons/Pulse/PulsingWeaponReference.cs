using Globals;
using UnityEngine;

namespace Weapons.Pulse
{
    [CreateAssetMenu(menuName = ProjectConstants.ScriptableObjectsAssetMenuName + 
                                "/" + ProjectConstants.WeaponsMenuName + 
                                "/Create new " + nameof(PulsingWeaponReference))]
    public class PulsingWeaponReference : BaseWeaponReference
    {
        public override BaseWeapon CreateWeaponInstance()
        {
            return new PulsingWeapon(this, WeaponSettings);
        }
    }
}