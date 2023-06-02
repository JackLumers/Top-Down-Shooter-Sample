using UnityEngine;

namespace Globals
{
    public static class AnimationConstants
    {
        public static readonly int IsInvincible = Animator.StringToHash("IsInvincible");
        public static readonly int IsMoving = Animator.StringToHash("IsMoving");
        public static readonly int WeaponFireTrigger = Animator.StringToHash("WeaponFireTrigger");
        public static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
    }
}