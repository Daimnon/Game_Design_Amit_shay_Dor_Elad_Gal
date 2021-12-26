using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour
{
    [SerializeField]
    private float _slowTimeModifier = 0.5f, _slowTimeSmootingTime = 1f, _slowTimeLength = 3f;

    [SerializeField]
    private float _previousTimeScale, _slowTimer;

    [SerializeField]
    private bool _isSlowingTime;
    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && !_isSlowingTime)
        {
            _isSlowingTime = true;
            _previousTimeScale = Time.timeScale;
            Time.timeScale = Mathf.Lerp(Time.timeScale, _slowTimeModifier, _slowTimeSmootingTime);
        }
        else if (_isSlowingTime)
        {
            _slowTimer += Time.deltaTime;

            if (_slowTimer >= _slowTimeLength)
            {
                Time.timeScale = Mathf.Lerp(_slowTimeModifier, _previousTimeScale, _slowTimeSmootingTime);
                _isSlowingTime = false;
                _slowTimer = 0;
            }

            if (Input.GetKey(KeyCode.R))
            {
                Time.timeScale = Mathf.Lerp(_slowTimeModifier, _previousTimeScale, _slowTimeSmootingTime);
                _isSlowingTime = false;
                _slowTimer = 0;
            }
        }
    }
}
