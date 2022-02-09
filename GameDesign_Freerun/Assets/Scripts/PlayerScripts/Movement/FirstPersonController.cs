using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    #region Required Components
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    #endregion
    public class FirstPersonController : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        private DetectObs _detectGround;

        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private CapsuleCollider _playerCol;

        [SerializeField]
        private Camera _mainCam;

        [SerializeField]
        private float _forwardSpeed = 3f, _sprintSpeed = 5f, _backwardSpeed = 3f, _strafeSpeed = 3f, _speedInAir = 0.3f, _jumpForce = 10f;

        [SerializeField]
        private bool _canRotate, _isGrounded;
        #endregion

        #region Private Fields
        private float _currentTargetSpeed = 8f, _yRotation;
        private bool _isMouseLocked = true;
        #endregion

        #region Public Fields
        public MouseLook MouseLook = new MouseLook();
        public Vector3 Relativevelocity;
        public bool IsWallRunning;
        #endregion

        #region Properties
        public Vector3 Velocity { get => _rb.velocity; }
        public bool Grounded { get => _isGrounded; }
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            _canRotate = true;
            MouseLook.Init(transform, _mainCam.transform);
        }

        private void Update()
        {
            Relativevelocity = transform.InverseTransformDirection(_rb.velocity);

            if (_isGrounded)
                if (Input.GetKeyDown(KeyCode.Space))
                    NormalJump();

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (_isMouseLocked)
                {
                    _isMouseLocked = false;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    _isMouseLocked = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();

            Vector2 input = GetInput();
            float h = input.x;
            float v = input.y;
            Vector3 inputVector = new Vector3(h, 0, v);

            inputVector = Vector3.ClampMagnitude(inputVector, 1);

            SpeedOnGround(input, inputVector);
            SpeedInAir(input, inputVector);
        }

        private void LateUpdate()
        {
            if (_canRotate)
                RotateView();

            else
                MouseLook.LookOveride(transform, _mainCam.transform);
        }
        #endregion

        #region Methods
        private Vector2 GetInput()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            UpdateDesiredTargetSpeed(input);

            return input;
        }

        private void GetSpeedInAir(Vector2 input, Vector3 inputVector)
        {
            if (Input.GetAxisRaw("Vertical") > 0.3f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * (_speedInAir / 1.5f * 1.2f) * Mathf.Abs(inputVector.z));

            else if (Input.GetAxisRaw("Vertical") > 0.3f)
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * _speedInAir / 1.5f * Mathf.Abs(inputVector.z));


            if (Input.GetAxisRaw("Vertical") < -0.3f)
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -_speedInAir * Mathf.Abs(inputVector.z));


            if (Input.GetAxisRaw("Horizontal") > 0.5f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(Time.deltaTime * 1000f * (_speedInAir / 1.5f * 1.2f) * Mathf.Abs(inputVector.x), 0, 0);

            else if (Input.GetAxisRaw("Horizontal") > 0.5f)
                _rb.AddRelativeForce(Time.deltaTime * 1000f * _speedInAir / 1.5f * Mathf.Abs(inputVector.x), 0, 0);

            if (Input.GetAxisRaw("Horizontal") < -0.5f)
                _rb.AddRelativeForce(Time.deltaTime * 1000f * -_speedInAir * Mathf.Abs(inputVector.x), 0, 0);
        }

        private void GetSpeedOnGround(Vector2 input, Vector3 inputVector)
        {
            //run
            if (Input.GetAxisRaw("Vertical") > 0.3f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * _sprintSpeed * Mathf.Abs(inputVector.z));
            //walk
            else if (Input.GetAxisRaw("Vertical") > 0.3f)
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * _forwardSpeed * Mathf.Abs(inputVector.z));


            if (Input.GetAxisRaw("Vertical") < -0.3f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -(_sprintSpeed / 1.2f) * Mathf.Abs(inputVector.z));

            else if (Input.GetAxisRaw("Vertical") < -0.3f)
                _rb.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -_backwardSpeed * Mathf.Abs(inputVector.z));


            if (Input.GetAxisRaw("Horizontal") > 0.5f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(Time.deltaTime * 1000f * (_sprintSpeed / 1.1f) * Mathf.Abs(inputVector.x), 0, 0);

            else if (Input.GetAxisRaw("Horizontal") > 0.5f)
                _rb.AddRelativeForce(Time.deltaTime * 1000f * _strafeSpeed * Mathf.Abs(inputVector.x), 0, 0);


            if (Input.GetAxisRaw("Horizontal") < -0.5f && Input.GetKey(KeyCode.LeftShift))
                _rb.AddRelativeForce(Time.deltaTime * 1000f * -(_sprintSpeed / 1.1f) * Mathf.Abs(inputVector.x), 0, 0);

            else if (Input.GetAxisRaw("Horizontal") < -0.5f)
                _rb.AddRelativeForce(Time.deltaTime * 1000f * -_strafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
        }
        
        private void GroundCheck()
        {
            if (_detectGround.Obstruction)
                _isGrounded = true;

            else
                _isGrounded = false;
        }

        private void SpeedOnGround(Vector2 input, Vector3 inputVector)
        {
            //grounded
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && _isGrounded && !IsWallRunning)
                GetSpeedOnGround(input, inputVector);
        }

        private void SpeedInAir(Vector2 input, Vector3 inputVector)
        { 
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && !_isGrounded && !IsWallRunning)
                GetSpeedInAir(input, inputVector);
        }

        public void NormalJump()
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
        }

        public void CamGoBack(float speed)
        {
            MouseLook.CamGoBack(transform, _mainCam.transform, speed);
        }

        public void CamGoBackAll()
        {
            MouseLook.CamGoBackAll(transform, _mainCam.transform);
        }

        public void SwitchDirectionJump()
        {
            _rb.velocity = transform.forward * _rb.velocity.magnitude;
            _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
        }

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
        
        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            if (_isMouseLocked)
                MouseLook.IsMouseLocked = true;
            else
                MouseLook.IsMouseLocked = false;

            MouseLook.LookRotation(transform, _mainCam.transform);
        }
        #endregion
    }
}