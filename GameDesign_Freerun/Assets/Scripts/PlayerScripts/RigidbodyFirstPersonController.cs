using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private DetectObs _detectGround;
        
        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private CapsuleCollider _playerCol;
        
        [SerializeField]
        private Camera _mainCam;
        
        [Header("Movement Settings")]
        [SerializeField]
        private float _forwardSpeed = 4f, _backwardSpeed = 3f, _strafeSpeed = 3f;

        [SerializeField]
        private float _speedInAir = 0.3f, _jumpForce = 10f;

        [SerializeField]
        private bool _canRotate, _isGrounded;
        
        private float _currentTargetSpeed = 8f, _yRotation;

        public MouseLook MouseLook = new MouseLook();
        public Vector3 Relativevelocity;
        public bool IsWallRunning;

        public Vector3 Velocity { get => _rb.velocity; }
        public bool Grounded { get => _isGrounded; }

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero)
                return;

            if (input.x > 0 || input.x < 0)
                _currentTargetSpeed = _strafeSpeed;

            if (input.y < 0)
                _currentTargetSpeed = _backwardSpeed;

            if (input.y > 0)
                _currentTargetSpeed = _forwardSpeed;
        }

        private void Awake()
        {
            _canRotate = true;
            _rb = GetComponent<Rigidbody>();
            _playerCol = GetComponent<CapsuleCollider>();
            MouseLook.Init (transform, _mainCam.transform);
        }

        private void Update()
        {
            Relativevelocity = transform.InverseTransformDirection(_rb.velocity);
            
            if (_isGrounded)
                if (Input.GetKeyDown(KeyCode.Space))
                    NormalJump();
        }

        private void LateUpdate()
        {
            if (_canRotate)
                RotateView();

            else
                MouseLook.LookOveride(transform, _mainCam.transform);
        }

        public void CamGoBack(float speed)
        {
            MouseLook.CamGoBack(transform, _mainCam.transform, speed);
        }

        public void CamGoBackAll ()
        {
            MouseLook.CamGoBackAll(transform, _mainCam.transform);
        }

        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            float h = input.x;
            float v = input.y;
            Vector3 inputVector = new Vector3(h, 0, v);
            inputVector = Vector3.ClampMagnitude(inputVector, 1);

            //grounded
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && _isGrounded && !IsWallRunning)
            {
                if (Input.GetAxisRaw("Vertical") > 0.3f)
                    _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * _forwardSpeed * Mathf.Abs(inputVector.z));
                
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                    _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -_backwardSpeed * Mathf.Abs(inputVector.z));
                
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                    _rb.AddRelativeForce(Time.deltaTime * 1000f * _strafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
                
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                    _rb.AddRelativeForce(Time.deltaTime * 1000f * -_strafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
            }

            //inair
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && !_isGrounded  && !IsWallRunning)
            {
                if (Input.GetAxisRaw("Vertical") > 0.3f)
                    _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * _speedInAir * Mathf.Abs(inputVector.z));
                
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                    _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -_speedInAir * Mathf.Abs(inputVector.z));
                
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                    _rb.AddRelativeForce(Time.deltaTime * 1000f * _speedInAir * Mathf.Abs(inputVector.x), 0, 0);
                
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                    _rb.AddRelativeForce(Time.deltaTime * 1000f * -_speedInAir * Mathf.Abs(inputVector.x), 0, 0);
            }
        }

        public void NormalJump()
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
        }

        public void SwitchDirectionJump()
        {
            _rb.velocity = transform.forward * _rb.velocity.magnitude;
            _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
        }
  
        private Vector2 GetInput()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			UpdateDesiredTargetSpeed(input);

            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            MouseLook.LookRotation (transform, _mainCam.transform);
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
          if(_detectGround.Obstruction)
                _isGrounded = true;

          else
                _isGrounded = false;
        }
    }
}