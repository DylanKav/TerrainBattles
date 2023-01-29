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
    
    //serialized
    [SerializeField] private float cameraDampening = 5;
    [SerializeField] private OrbitalCamera cameraControls;
    [SerializeField] private bool invertCamControls = true;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private Camera cam;
    

    public void PlayerMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
        
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
        if (_movementInput == Vector2.zero) return;
        var movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        movement = cam.transform.TransformDirection(movement);
        movement.y = 0;
        rigidBody.MovePosition(transform.position +  movement * (Time.deltaTime * speed));
        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(movement), 30 *Time.deltaTime));
        
    }
}
