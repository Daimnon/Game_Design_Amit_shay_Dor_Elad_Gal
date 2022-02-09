using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class MouseLook
    {
        [SerializeField]
        private float _xSensitivity = 2f, _ySensitivity = 2f, _minimumX = -90F, _maximumX = 90F, _smoothTime = 5f;

        [SerializeField]
        private bool _smooth, _clampVerticalRotation = true;

        [SerializeField]
        private Quaternion _characterTargetRot, _cameraTargetRot;

        public bool IsMouseLocked = true;

        public void Init(Transform character, Transform camera)
        {
            _characterTargetRot = character.localRotation;
            _cameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera)
        {
            if (IsMouseLocked)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;

            float yRot = Input.GetAxis("Mouse X") * _xSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * _ySensitivity;

            _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (_clampVerticalRotation)
                _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            if (_smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, _characterTargetRot,
                    _smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, _cameraTargetRot,
                    _smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = _characterTargetRot;
                camera.localRotation = _cameraTargetRot;
            }
        }

        public void LookOveride(Transform character, Transform camera)
        {
            _characterTargetRot = character.localRotation;
            _cameraTargetRot = camera.localRotation;

            _characterTargetRot.x = 0f;
            _characterTargetRot.z = 0f;
            _cameraTargetRot.z = 0f;
            _cameraTargetRot.y = 0f;
        }

        public void CamGoBackAll(Transform character, Transform camera)
        {
            _cameraTargetRot.x = 0f;
            _cameraTargetRot.z = 0f;
            _cameraTargetRot.y = 0f;

            camera.localRotation = _cameraTargetRot;
        }

        public void CamGoBack(Transform character, Transform camera, float speed)
        {
            if (_cameraTargetRot.x > 0)
                _cameraTargetRot.x -= 1f * Time.deltaTime * speed;
            
            if (_cameraTargetRot.x < 0)
                _cameraTargetRot.x += 1f * Time.deltaTime * speed;

            if (_cameraTargetRot.y > 0)
                _cameraTargetRot.y -= 1f * Time.deltaTime * speed;
            
            if (_cameraTargetRot.y < 0)
                _cameraTargetRot.y += 1f * Time.deltaTime * speed;

            if (_cameraTargetRot.z > 0)
                _cameraTargetRot.z -= 1f * Time.deltaTime * speed;
            
            if (_cameraTargetRot.z < 0)
                _cameraTargetRot.z += 1f * Time.deltaTime * speed;

            camera.localRotation = _cameraTargetRot;
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, _minimumX, _maximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
