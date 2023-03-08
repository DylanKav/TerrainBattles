using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [SerializeField] private Camera orbitalCam;
    [SerializeField] private Transform player;
    [SerializeField] [Range(5, 30)]private float currentDistance = 5f;
    [SerializeField] private int maxZoomDistance = 30;
    [SerializeField] private int minZoomDistance = 5;
    [SerializeField] private bool isSlerpedMovement = false;
    [SerializeField] [Range(2, 15)]private float followStrength = 0.5f;
    public float horCamAngle = 0f;
    public float verCamAngle = 90f;
    public Vector3 FocusPointOffset = Vector3.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float Zoom
    {
        get => currentDistance;
        set
        {
            currentDistance += (-1)*value;
            if (currentDistance < minZoomDistance) currentDistance = minZoomDistance;
            if (currentDistance > maxZoomDistance) currentDistance = maxZoomDistance;
        }
    }
    public void UpdateCamAngle(Vector2 delta, bool invertCamControls, float cameraDampening)
    {
        horCamAngle += invertCamControls ? -delta.x / cameraDampening : delta.x / cameraDampening;
        verCamAngle += invertCamControls ? -delta.y / cameraDampening : delta.y / cameraDampening;
        if (verCamAngle <= 85) verCamAngle = 85;
        if (verCamAngle >= 160) verCamAngle = 160;
    }

    void Update()
    {
        Vector3 lookDirection = player.position - orbitalCam.transform.position;
        lookDirection.Normalize();

        orbitalCam.transform.rotation = isSlerpedMovement?Quaternion.Slerp(orbitalCam.transform.rotation,
            Quaternion.LookRotation(lookDirection), (followStrength * 5) * Time.deltaTime) : Quaternion.LookRotation(lookDirection);
        //orbitalCam.transform.LookAt(player.transform);
        var angleToRad = Mathf.Deg2Rad*horCamAngle;
        var verticalMultiplier = Mathf.Sin(verCamAngle * Mathf.Deg2Rad);
        var camPositionSign = verCamAngle < 90 ? -1 : 1;
        var yPos = (currentDistance * (1 - verticalMultiplier)) * camPositionSign;
        var targetVector = new Vector3((verticalMultiplier * currentDistance) * Mathf.Cos(angleToRad), yPos,
            (verticalMultiplier * currentDistance) * Mathf.Sin(angleToRad)) + (player.position + FocusPointOffset);
        if (Vector3.Distance(orbitalCam.transform.position, targetVector) <= .01f) return;
        
        
        orbitalCam.transform.position = isSlerpedMovement? Vector3.Slerp(orbitalCam.transform.position,
            targetVector, (followStrength * 5) * Time.deltaTime) : targetVector;
        
    }
}
