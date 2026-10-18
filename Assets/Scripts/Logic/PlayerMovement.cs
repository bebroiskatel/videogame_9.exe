using _Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logic
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform cameraTransform;
        
        [Header("Steps per tick")]
        public sbyte stepSize = 1;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.2f;

        [Header("Sway settings")]
        [SerializeField] private float bobSpeed = 11f;
        [SerializeField] private float bobIntensityX = 0.2f;
        [SerializeField] private float bobIntensityY = 0.5f;

        [Header("Jump settings")]
        public sbyte jumpForce = 8;
        public float floatForce = 2;

        private Rigidbody _rb;
        private Collider _collider;
        private Vector3 _tickMoveDirection;
        private float _initialCameraY;
        private bool _isGrounded;
        private float _waveTimer;
        private bool _isMovingThisTick;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _initialCameraY = cameraTransform.localPosition.y;
        }

        private void OnEnable()  => TickTimer.OnTick += MoveOnTick;
        private void OnDisable() => TickTimer.OnTick -= MoveOnTick;

        private void Update()
        {
            var input = Vector3.zero;
            if (Keyboard.current.wKey.isPressed) input.z = 1;
            if (Keyboard.current.sKey.isPressed) input.z = -1;
            if (Keyboard.current.aKey.isPressed) input.x = -1;
            if (Keyboard.current.dKey.isPressed) input.x = 1;
        
            _tickMoveDirection = transform.TransformDirection(input.normalized);
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame && _isGrounded)
            {
                //setting speedY to 0
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            
            if (Keyboard.current.spaceKey.isPressed && !_isGrounded && _rb.linearVelocity.y < 0.1f)
            {
                var rng1 = Random.Range(floatForce - 7, floatForce);
                var rng2 = Random.Range(floatForce - 1, floatForce + 2);
                _rb.AddForce(Vector3.up * (rng1 / rng2), ForceMode.Acceleration);
            }

            HandleCameraBobbing();
        }

        private void MoveOnTick()
        {
            _isGrounded = CheckIfGrounded();
            
            if (_tickMoveDirection.sqrMagnitude > 0)
            {
                _isMovingThisTick = true;
                var targetPosition = _rb.position + _tickMoveDirection * stepSize;
                targetPosition.y = _rb.position.y; 
                _rb.MovePosition(targetPosition);
            }
            else
            {
                _isMovingThisTick = false;
            }
        }

        private void HandleCameraBobbing()
        {
            if (cameraTransform == null) return;
            if (_isMovingThisTick && _isGrounded)
            {
                _waveTimer += Time.deltaTime * bobSpeed;
            }
            else
            {
                _waveTimer = Mathf.MoveTowards(_waveTimer, Mathf.Round(_waveTimer / Mathf.PI) * Mathf.PI,
                    Time.deltaTime * bobSpeed);
            }
            var newX = Mathf.Cos(_waveTimer) * bobIntensityX;
            var newY = _initialCameraY + Mathf.Abs(Mathf.Sin(_waveTimer) * bobIntensityY);
            
            var targetLocalPosition = new Vector3(newX, newY, cameraTransform.localPosition.z);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetLocalPosition, Time.deltaTime * 15f);
        }

        private bool CheckIfGrounded()
        {
            if (_collider == null) return false;
            var rayLength = _collider.bounds.extents.y + groundCheckDistance;
            var hit = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
            
            return hit;
        }
    }
}
