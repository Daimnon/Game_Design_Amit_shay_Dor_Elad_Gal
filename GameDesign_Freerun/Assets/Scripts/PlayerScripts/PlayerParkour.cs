using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerParkour : MonoBehaviour
{

    [SerializeField]
    private DetectObs _detectVaultObject, _detectClimbObject; //checks for..
    
    [SerializeField]
    private DetectObs _detectVaultObstruction, _detectClimbObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to..

    [SerializeField]
    private DetectObs _detectWallLeft, _detectWallRight; //detects for a wall on player sides

    [SerializeField]
    private FirstPersonController _playerController;
    
    [SerializeField]
    private Transform _vaultEndPoint, _climbEndPoint;
    
    [SerializeField]
    private Rigidbody _playerRb;
    
    [SerializeField]
    public Animator _mainCamAnimator;

    [SerializeField]
    private float _wallRunUpForce, _wallRunUpForceDecreaseRate, _wallJumpUpVelocity, _wallJumpForwardVelocity;

    [SerializeField]
    private float _dragGrounded, _dragInAir, _dragWallRun;

    [SerializeField]
    private float _vaultTime, _climbTime; //How long it takes to do Action
    
    [SerializeField]
    private bool _wallRunning, _wallrunningLeft, _wallrunningRight, _isVaulting;

    private Vector3 _recordedStartPosition; //position of player right before vault
    private Vector3 _recordedMoveToPosition; //the position of the vault end point in world space to move the player to
    private float _upForce, _tParkour, _currentActionTime;

    private bool _canVault, _canClimb;
    /* private bool _canWallRun; //ensure that player can only wallrun once before needing to hit the ground again, can be modified for double wallruns*/

    void Update()
    {
        if (_playerController.Grounded)
        {
            _playerRb.drag = _dragGrounded;
            //_canWallRun = true;
        }
        else
            _playerRb.drag = _dragInAir;

        if (_wallRunning)
            _playerRb.drag = _dragWallRun;

        //vault
        if (_detectVaultObject.Obstruction && !_detectVaultObstruction.Obstruction && !_canVault && !_isVaulting && !_wallRunning)
            if ((Input.GetKey(KeyCode.Space) || !_playerController.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
                _canVault = true;

        if (_canVault)
        {
            _canVault = false; // so this is only called once *currently off completly
            
            _playerRb.isKinematic = true; //ensure physics do not interrupt the vault

            _recordedMoveToPosition = _vaultEndPoint.position;
            _recordedStartPosition = transform.position;
            _isVaulting = true;
            _currentActionTime = _vaultTime;

            _mainCamAnimator.CrossFade("Vault", 0.1f);
        }

        //climb
        if (_detectClimbObject.Obstruction && !_detectClimbObstruction.Obstruction && !_canClimb && !_isVaulting && !_wallRunning)
            if ((Input.GetKey(KeyCode.Space) || !_playerController.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
                _canClimb = true;

        if (_canClimb)
        {
            _canClimb = false; // so this is only called once

            _playerRb.isKinematic = true; //ensure physics do not interrupt the vault
            
            _recordedMoveToPosition = _climbEndPoint.position;
            _recordedStartPosition = transform.position;
            _isVaulting = true;
            _currentActionTime = _climbTime;

            _mainCamAnimator.CrossFade("Climb", 0.1f);
        }

        //Vault movement
        if (_isVaulting && _tParkour < 1f)
        {
            _tParkour += Time.deltaTime / _currentActionTime;
            transform.position = Vector3.Lerp(_recordedStartPosition, _recordedMoveToPosition, _tParkour);

            if (_tParkour >= 1f)
            {
                _isVaulting = false;
                _tParkour = 0f;
                _playerRb.isKinematic = false;
            }
        }

        //Wallrun
        if (_detectWallLeft.Obstruction && !_playerController.Grounded && !_isVaulting /*&& _canWallRun*/) // if detect wall on the left and is not on the ground and not doing parkour(climb/vault)
        {
            _wallrunningLeft = true;
            //_canWallRun = false;
            _upForce = _wallRunUpForce; //refer to line 166
        }

        if (_detectWallRight.Obstruction && !_playerController.Grounded && !_isVaulting /*&& _canWallRun*/) // if detect wall on thr right and is not on the ground
        {
            _wallrunningRight = true;
            //_canWallRun = false;
            _upForce = _wallRunUpForce;
        }

        // if there is no wall on the lef tor pressing forward or forward speed < 1 (refer to fpscontroller script)
        if (_wallrunningLeft && !_detectWallLeft.Obstruction || Input.GetAxisRaw("Vertical") <= 0f || _playerController.Relativevelocity.magnitude < 1f)
        {
            _wallrunningLeft = false;
            _wallrunningRight = false;
        }

        // same as above
        if (_wallrunningRight && !_detectWallRight.Obstruction || Input.GetAxisRaw("Vertical") <= 0f || _playerController.Relativevelocity.magnitude < 1f)
        {
            _wallrunningLeft = false;
            _wallrunningRight = false;
        }

        if (_wallrunningLeft || _wallrunningRight)
        {
            _wallRunning = true;
            _playerController.IsWallRunning = true; // this stops the playermovement (refer to fpscontroller script)
        }
        else
        {
            _wallRunning = false;
            _playerController.IsWallRunning = false;
        }

        if (_wallrunningLeft)
            _mainCamAnimator.SetBool("WallLeft", true); //Wallrun camera tilt

        else
            _mainCamAnimator.SetBool("WallLeft", false);
        
        if (_wallrunningRight)
            _mainCamAnimator.SetBool("WallRight", true);

        else
            _mainCamAnimator.SetBool("WallRight", false);

        if (_wallRunning)
        {
            //set the y velocity while wallrunning
            _playerRb.velocity = new Vector3(_playerRb.velocity.x, _upForce, _playerRb.velocity.z);

            //so the player will have a curve like wallrun, upforce from line 136
            _upForce -= Mathf.Lerp(_playerRb.velocity.y, _wallRunUpForceDecreaseRate * Time.deltaTime, Time.deltaTime * 5);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //walljump
                _playerRb.velocity = transform.forward * _wallJumpForwardVelocity + transform.up * _wallJumpUpVelocity;
                
                _wallrunningLeft = false;
                _wallrunningRight = false;
            }
            
            if (_playerController.Grounded)
            {
                _wallrunningLeft = false;
                _wallrunningRight = false;
            }
        }
    }
}
