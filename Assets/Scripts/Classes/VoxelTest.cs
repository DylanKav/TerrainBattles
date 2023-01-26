using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class VoxelTest : MonoBehaviour
{
    Voxel _voxel = new Voxel(Vector3.zero);
    void Start()
    {
        
        _voxel.Points[1].isOn = true;
        _voxel.Points[5].isOn = true;
        _voxel.Points[7].isOn = true;

        var mesh = new Mesh
        {
            name = "VoxelText"
        };
        
        mesh.vertices = _voxel.CalculateMeshVertices().ToArray();
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        mesh.RecalculateBounds();
        
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnDrawGizmosSelected()
    {
        
        foreach (var point in _voxel.Points)
        {
            Gizmos.color = point.isOn ? Color.green : Color.red;
            Gizmos.DrawSphere(point.Position, 0.05f);
        }
    }
}
