using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class IKFeet : MonoBehaviour
{
    public Animator Controller;
    
    [Range(0, 1f)]
    public float DistanceToGround;

    public LayerMask layerMask;

    [SerializeField] private Transform RightFoot, LeftFoot;
    private void LateUpdate()
    {
        if (!Controller) return;
        //Controller.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        //Controller.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
        //Controller.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        //Controller.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

        //Left Foot
        RaycastHit hit;
        Ray RayRightFoot = new Ray(RightFoot.position + Vector3.up, Vector3.down);
        Ray RayLeftFoot = new Ray(LeftFoot.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(RayLeftFoot, out hit, DistanceToGround + 1f, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += DistanceToGround;
            LeftFoot.position = footPosition;
            LeftFoot.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            //Controller.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            //Controller.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }
        if (Physics.Raycast(RayRightFoot, out hit, DistanceToGround + 1f, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += DistanceToGround;
            RightFoot.position = footPosition;
            RightFoot.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
        }
    }
}
