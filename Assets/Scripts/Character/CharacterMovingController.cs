using UnityEngine;

namespace Character
{
    public class CharacterMovingController
    {
        private readonly Rigidbody _rigidbody;
        private readonly Transform _modelTransform;
        
        public CharacterMovingController(Rigidbody rigidbody, Transform modelTransform)
        {
            _rigidbody = rigidbody;
            _modelTransform = modelTransform;
        }

        /// <summary>
        /// Moves player by applying force.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speedMultiplier"></param>
        /// <param name="forceMode"><see cref="ForceMode"/></param>
        /// <param name="smoothByTimeDelta">Do use <see cref="Time.deltaTime"/> for smoothing?</param>
        /// <param name="maxSpeed">Clamps if set. -1 means no clamp.</param>
        public void Move(Vector3 direction, float speedMultiplier, ForceMode forceMode, 
            bool smoothByTimeDelta, float maxSpeed = -1)
        {
            direction.Normalize();

            var force = direction * speedMultiplier;
            
            if (smoothByTimeDelta)
                force *= Time.deltaTime;
            
            _rigidbody.AddForce(force, forceMode);

            if (maxSpeed >= 0)
            {
                ClampVelocity(maxSpeed);
            }
        }
        
        public void Rotate(Vector3 rotation)
        {
            _modelTransform.rotation = Quaternion.Euler(rotation);
        }

        private void ClampVelocity(float maxVelocity)
        {
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxVelocity);
        }
    }
}