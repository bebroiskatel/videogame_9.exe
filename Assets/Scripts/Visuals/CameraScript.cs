using UnityEngine;
using UnityEngine.InputSystem;

namespace Visuals
{
    public class CameraScript : MonoBehaviour
    {
        public float mouseSensitivity = 100f;
        public float smoothSpeed = 5f;

        private float _xRotation;
        private float _yRotation;
    
        private float _currentXRotation;
        private float _currentYRotation;

        private Transform _playerBody;

        private void Start()
        {
            _playerBody = transform.parent;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
            var mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;
            
            //turning angles
            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);

            //angle smothering
            _currentXRotation = Mathf.Lerp(_currentXRotation, _xRotation, smoothSpeed * Time.deltaTime);
            _currentYRotation = Mathf.Lerp(_currentYRotation, _yRotation, smoothSpeed * Time.deltaTime);
            
            transform.localRotation = Quaternion.Euler(_currentXRotation, 0f, 0f);
            
            _playerBody.rotation = Quaternion.Euler(0f, _currentYRotation, 0f);
        }
    }
}
