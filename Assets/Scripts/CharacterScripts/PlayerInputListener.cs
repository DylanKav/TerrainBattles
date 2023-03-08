using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour
{
    //privates

    private bool _camMovementEnabled = true;
    private Vector2 startMouse = new Vector2(0,0);
    private Vector2 _movementInput;
    private bool _canJump = true;
    private bool _isGrounded = true;
    private bool _isCombatEngaged = true;
    Vector3 _move = new Vector3(0, 0, 0);
    private bool _attackDisabled = false;
    private bool _isBlocking = false;


    [Header("Custom Control Vars")]
    [SerializeField] private float cameraDampening = 5;
    [SerializeField] private bool invertCamControls = true;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotSpeed = 20f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityValue = 9.81f;
    [SerializeField] private float timeToFullSpeed = 5f;
    [SerializeField] private float slowDownMultiplier = 2f;
    
    [Header("Mandatory Fields")]
    [SerializeField] private OrbitalCamera cameraControls;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Rigidbody rigidBody;

    [SerializeField] private Camera cam;
    [SerializeField] private AnimationController animController;
    
    
    
    [Header("Variables used for FX")]
    private bool _ragdollMode = false;

    public float _isJumping = 0;
    private Vector3 playerVelocity;

    public void SetRagdollMode(bool value, Vector3 direction, float pushForce)
    {
        //used for ragdoll mode.
        _ragdollMode = value;
        //characterController.constraints = RigidbodyConstraints.None;
        characterController.enabled = !value;
        rigidBody.isKinematic = !value;
        rigidBody.AddForce(direction * pushForce, ForceMode.Impulse);
    }
    
    public void SetRagdollMode(bool value, Vector3 direction, float pushForce, Vector3 positionOfForce)
    {
        //used for ragdoll mode.
        _ragdollMode = value;
        //characterController.constraints = RigidbodyConstraints.None;
        characterController.enabled = !value;
        rigidBody.isKinematic = !value;
        rigidBody.AddForceAtPosition(direction * pushForce, positionOfForce, ForceMode.Impulse);
    }
    
    public void PlayerMove(InputAction.CallbackContext context)
    {
        if (_ragdollMode) return;
        _movementInput = context.ReadValue<Vector2>();
        animController.SetMovement(_movementInput);

    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (_ragdollMode) return;
        _isJumping = context.ReadValue<float>();
    }

    public void MouseScroll(InputAction.CallbackContext context)
    {
        
        if (_ragdollMode) return;
        var scroll = context.ReadValue<Vector2>();
        cameraControls.Zoom = (float)(scroll.y * 0.5d);
    }

    public void BlockAttack(InputAction.CallbackContext context)
    {
        animController.SetBlock(context.ReadValueAsButton());
        _isBlocking = context.ReadValueAsButton();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (_isBlocking || _attackDisabled) return;
        animController.SetAttack(true);
        StartCoroutine(AttackDebounce(3));
        
    }
    
    public void CameraMove(InputAction.CallbackContext context)
    {
        if (context.control.device == Mouse.current && !_camMovementEnabled) return;
        var delta = context.ReadValue<Vector2>();
        cameraControls.UpdateCamAngle(delta, invertCamControls, cameraDampening);
        var targetRotation = cam.transform.rotation.eulerAngles;
        //targetRotation.x = 0;
        //targetRotation.z = 0;
        //rigidBody.MoveRotation(Quaternion.Euler(targetRotation));

    }

    private void Update()
    {
        if (_ragdollMode) return;
        var groundedPlayer = characterController.isGrounded;
        var slowMultiplier = _movementInput == Vector2.zero ? slowDownMultiplier : 1f;
        if(!_ragdollMode) _move = Vector3.Lerp(_move, new Vector3(_movementInput.x, 0, _movementInput.y), Time.deltaTime*timeToFullSpeed * slowMultiplier);
        var movement = cam.transform.TransformDirection(_move);
        movement.y = 0;
        characterController.Move(movement * (Time.deltaTime * speed));
        var forwardCam = cam.transform.TransformDirection(0, 0, 1);
        forwardCam.y = 0;
        if(_move != Vector3.zero || _isCombatEngaged) transform.rotation = _isCombatEngaged?(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forwardCam), rotSpeed *Time.deltaTime)) : (Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotSpeed *Time.deltaTime));

        // Changes the height position of the player..
        if (Math.Abs(_isJumping - 1) < .1f && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animController.SetIsGrounded(false);
        }
        else
        {
            if(animController.GetIsGrounded() != groundedPlayer) animController.SetIsGrounded(true);
        }
        if(!groundedPlayer) playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

    }

    private IEnumerator AttackDebounce(float seconds)
    {
        _attackDisabled = true;
        yield return new WaitForEndOfFrame();
        animController.SetAttack(false);
        yield return new WaitForSeconds(seconds);
        _attackDisabled = false;
    }

}
