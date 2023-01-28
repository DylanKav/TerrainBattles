using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BufferTest : MonoBehaviour
{
    [SerializeField] private ComputeShader VoxelData;
    [SerializeField] private GameObject cube;
    public Vector3Int ChunkDimensions = new(8, 8, 8);
    public Vector3Int ChunkPosition = new(0, 0, 0);
    private ComputeBuffer PointsBuffer;
    private float4[] data;

    void Start()
    {
        
        PointsBuffer = new ComputeBuffer(ChunkDimensions.x * ChunkDimensions.y * ChunkDimensions.z, sizeof(float) * 4, ComputeBufferType.Append);
        VoxelData.SetBuffer(0, "test", PointsBuffer);
        VoxelData.SetVector("startingPosition", new Vector4(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z));
        PointsBuffer.SetCounterValue(0);
        VoxelData.Dispatch(0, 1, 1, 1);
        data = new float4[PointsBuffer.count];
        PointsBuffer.GetData(data);
        Debug.Log(data.Length);

        PointsBuffer?.Release();
        /*foreach (var point in data)
        {
            Debug.Log(point.w);
            if (point.w > 0)
            {
                var cubeClone = Instantiate(cube);
                cubeClone.transform.position = new Vector3(point.x, point.y, point.z);
                
            }
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(ChunkDimensions.x/2 -.5f,ChunkDimensions.y/2 -.5f, ChunkDimensions.z/2 -.5f) + ChunkPosition, ChunkDimensions - Vector3.one);
        if (data == null) return;
        foreach (var point in data)
        {
            Gizmos.color = point.w>0? Color.black : Color.red;
            Gizmos.DrawCube(new Vector3(point.x, point.y, point.z), new Vector3(.1f, .1f, .1f));
        }

    }
}
