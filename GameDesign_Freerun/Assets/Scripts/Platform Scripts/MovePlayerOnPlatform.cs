using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerOnPlatform : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    
    [SerializeField]
    private Transform _platformTr;

    private Vector3 _movementAmount;
    private Vector3 _oldPos;
    private bool _isPlayerOnPlatform = false;

    private void Start()
    {
        _oldPos = _platformTr.position;
    }

    private void Update()
    {
        _movementAmount = _platformTr.position - _oldPos;
        _oldPos = _platformTr.position;

        if (_isPlayerOnPlatform)
            _player.transform.position += _movementAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
            _isPlayerOnPlatform = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _player)
            _isPlayerOnPlatform = false;
    }
}