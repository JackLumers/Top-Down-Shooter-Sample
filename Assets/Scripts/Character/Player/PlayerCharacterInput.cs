using Globals;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerCharacterInput : MonoBehaviour
    {
        [SerializeField] private float _maxDistanceFromCameraToGround = 100f;
        [SerializeField] private float _lookingHeight = 1f;
        [Space] [SerializeField] private bool _debugMode;
        
        private Camera _playerViewCamera;
        private PlayerCharacter _playerCharacter;
        private PlayerInputActions _playerInputActions;
        
        private GameObject _debugPointerObject;
        
        private void Awake()
        {
            // Better to get it from some Camera manager but will do the work
            _playerViewCamera = Camera.main;

            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _playerInputActions.Enable();
            _playerInputActions.Player.Fire.performed += OnFirePerformed;
            _playerInputActions.Player.WeaponSwitch.performed += OnWeaponSwitchPerformed;
        }

        private void FixedUpdate()
        {
            var value = _playerInputActions.Player.Move.ReadValue<Vector2>();
            var asVector3 = new Vector3(value.x, 0, value.y);

            if (!asVector3.Equals(Vector3.zero))
            {
                _playerCharacter.MoveSelf(asVector3);
            }

            var pointerPosition = _playerInputActions.Player.Look.ReadValue<Vector2>();
            var screenPointToRay = _playerViewCamera.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(screenPointToRay, out var groundHitInfo, 
                    _maxDistanceFromCameraToGround,
                    1 << LayerMaskConstants.GroundLayer))
            {
                var groundHitPoint = groundHitInfo.point;
                var lookingPoint = new Vector3(groundHitPoint.x, groundHitPoint.y + _lookingHeight, groundHitPoint.z);

                _playerCharacter.SetLookingPoint(lookingPoint);
                
                SetDebugPointer(lookingPoint, _debugMode);
            }
        }

        private void OnFirePerformed(InputAction.CallbackContext callbackContext)
        {
            _playerCharacter.FireSelectedWeapon();
        }
        
        private void OnWeaponSwitchPerformed(InputAction.CallbackContext callbackContext)
        {
            _playerCharacter.SwitchWeapon();
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Fire.performed -= OnFirePerformed;
        }

        private void OnDestroy()
        {
            _playerInputActions?.Dispose();
        }
        
        private void SetDebugPointer(Vector3 position, bool show)
        {
            if (show)
            {
                if (ReferenceEquals(_debugPointerObject, null))
                {
                    _debugPointerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    _debugPointerObject.GetComponent<MeshRenderer>().material.color = Color.red;
                    _debugPointerObject.GetComponent<Collider>().enabled = false;
                    _debugPointerObject.transform.parent = transform;
                    _debugPointerObject.name = $"{name}_DebugPointerObject";
                }
                else
                {
                    _debugPointerObject.gameObject.SetActive(true);
                    _debugPointerObject.transform.position = position;
                }
            }
            else
            {
                if (ReferenceEquals(_debugPointerObject, null)) return;
                
                Destroy(_debugPointerObject);
                _debugPointerObject = null;
            }
        }
    }
}