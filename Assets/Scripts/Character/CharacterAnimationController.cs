using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Character
{
    public class CharacterAnimationController : IDisposable
    {
        private Animator _animator;
        private MeshRenderer _meshRenderer;
        
        private Color _invincibilityColor = Color.red;
        private Color _normalColor;
        private Material _playerMaterialInstance;
        
        public CharacterAnimationController(Animator animator, Renderer playerRenderer)
        {
            _animator = animator;
            
            // Setting color by property block is more efficient but this requires custom shader.
            // This is used only for simple invincibility indication so will go.
            _playerMaterialInstance = playerRenderer.material;
            _normalColor = _playerMaterialInstance.color;
        }

        public void AnimateMoving(bool isMoving)
        {
            //_playerAnimator.SetBool(AnimationConstants.IsMoving, isMoving);
        }
        
        public void AnimateShot()
        {
            //_playerAnimator.SetTrigger(AnimationConstants.DashTrigger);
        }

        public void AnimateInvincibility(bool isInvincible)
        {
            _playerMaterialInstance.color = isInvincible ? _invincibilityColor : _normalColor;
        }

        public void Dispose()
        {
            Object.Destroy(_playerMaterialInstance);
        }
    }
}