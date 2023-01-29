using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [SerializeField] private Camera orbitalCam;
    [SerializeField] private Transform player;
    [SerializeField] [Range(0, 5)]private float currentDistance = 5f;
    [SerializeField] [Range(0, 5)]private float followStrength = 0.5f;
    public float horCamAngle = 0f;

    public void UpdateCamAngle(Vector2 delta, bool invertCamControls, float cameraDampening)
    {
        horCamAngle += invertCamControls ? -delta.x / cameraDampening : delta.x / cameraDampening;
        
    }

    void Update()
    {
        Vector3 lookDirection = player.position - orbitalCam.transform.position;
        lookDirection.Normalize();

        orbitalCam.transform.rotation = Quaternion.Slerp(orbitalCam.transform.rotation,
            Quaternion.LookRotation(lookDirection), (followStrength * 5) * Time.deltaTime);
        //orbitalCam.transform.LookAt(player.transform);
        var angleToRad = Mathf.Deg2Rad*horCamAngle;
        var targetVector = new Vector3(currentDistance * Mathf.Cos(angleToRad), currentDistance / 2, currentDistance * Mathf.Sin(angleToRad)) + player.position;
        if (Vector3.Distance(orbitalCam.transform.position, targetVector) <= .01f) return;
        
        orbitalCam.transform.position = Vector3.Slerp(orbitalCam.transform.position,
            targetVector, (followStrength * 5) * Time.deltaTime);
        
    }
}
