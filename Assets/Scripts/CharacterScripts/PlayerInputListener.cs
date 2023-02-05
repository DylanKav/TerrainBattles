using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour
{
    //privates

    private bool _camMovementEnabled;
    private Vector2 startMouse = new Vector2(0,0);
    private Vector2 _movementInput;
    private bool _canJump = true;
    private bool _isGrounded = true;
    
    
    [Header("Custom Control Vars")]
    [SerializeField] private float cameraDampening = 5;
    [SerializeField] private bool invertCamControls = true;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotSpeed = 20f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityValue = 9.81f;
    
    [Header("Mandatory Fields")]
    [SerializeField] private OrbitalCamera cameraControls;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera cam;
    [SerializeField] private AnimationController animController;
    
    [Header("Variables used for FX")]
    private bool _controlDisabled = false;

    public float _isJumping = 0;
    private Vector3 playerVelocity;

    public void SetRagdollMode(bool value)
    {
        //used for ragdoll mode.
        _controlDisabled = value;
        //characterController.constraints = RigidbodyConstraints.None;
    }
    
    public void PlayerMove(InputAction.CallbackContext context)
    {
        if (_controlDisabled) return;
        _movementInput = context.ReadValue<Vector2>();
        animController.IsRunning = _movementInput != Vector2.zero;

    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (_controlDisabled) return;
        _isJumping = context.ReadValue<float>();
    }
    
    public void MouseEnableCamera(InputAction.CallbackContext context)
    {
        _camMovementEnabled = context.ReadValueAsButton();
        if (_camMovementEnabled) startMouse = Mouse.current.position.ReadValue();
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
        var groundedPlayer = characterController.isGrounded;

        Vector3 move = new Vector3(_movementInput.x, 0, _movementInput.y);
        var movement = cam.transform.TransformDirection(move);
        movement.y = 0;
        characterController.Move(movement * (Time.deltaTime * speed));
        
        if(move != Vector3.zero) transform.rotation = (Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotSpeed *Time.deltaTime));

        // Changes the height position of the player..
        if (Math.Abs(_isJumping - 1) < .1f && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
        
    }

}
