using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BufferTest : MonoBehaviour
{
    [SerializeField] private ComputeShader VoxelData;
    [SerializeField] private ComputeShader MarchingCubes;
    [SerializeField] private GameObject cube;
    public Vector3Int ChunkDimensions = new(8, 8, 8);
    public Vector3Int ChunkPosition = new(0, 0, 0);
    private ComputeBuffer PointsBuffer;
    private ComputeBuffer OutputBuffer;
    private ComputeBuffer TriangleBuffer;
    private ComputeBuffer TriCountBuffer;
    private float4[] data;

    private void Start()
    {
        UpdateChunk();
        GenerateMesh();
        ReleaseBuffers();
    }

    void CreateBuffers()
    {
        int numPoints = ChunkDimensions.x * ChunkDimensions.y * ChunkDimensions.z;
        int numVoxelsPerAxis = ChunkDimensions.x - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;
        
        PointsBuffer = new ComputeBuffer(ChunkDimensions.x * ChunkDimensions.y * ChunkDimensions.z, sizeof(float) * 4);
        OutputBuffer = new ComputeBuffer(ChunkDimensions.x * ChunkDimensions.y * ChunkDimensions.z, sizeof(float) * 4, ComputeBufferType.Append);
        TriangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof (float) * 3 * 3, ComputeBufferType.Append);
        TriCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);
    }
    void UpdateChunk()
    {
        CreateBuffers();
        PointsBuffer.SetCounterValue(0);
        OutputBuffer.SetCounterValue(0);
        VoxelData.SetBuffer(0, "test", PointsBuffer);
        VoxelData.SetBuffer(0, "appendBuffer", OutputBuffer);
        VoxelData.SetVector("startingPosition", new Vector4(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z));
        
        VoxelData.Dispatch(0, 1, 1, 1);
        data = new float4[OutputBuffer.count];
        
    }

    private void GenerateMesh()
    {
        TriangleBuffer.SetCounterValue(0);
        TriCountBuffer.SetCounterValue(0);
        OutputBuffer.GetData(data);
        MarchingCubes.SetBuffer(0, "indices", PointsBuffer);
        MarchingCubes.SetBuffer(0, "triangles", TriangleBuffer);
        MarchingCubes.Dispatch(0, 1, 1, 1);
        
        ComputeBuffer.CopyCount (TriangleBuffer, TriCountBuffer, 0);
        int[] triCountArray = { 0 };
        TriCountBuffer.GetData (triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        TriangleBuffer.GetData (tris, 0, 0, numTris);

        Mesh mesh = new Mesh();
        mesh.Clear ();

        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals ();

        this.GetComponent<MeshFilter>().sharedMesh = mesh;

    }

    private void ReleaseBuffers()
    {
        PointsBuffer?.Release();
        OutputBuffer?.Release();
        TriangleBuffer?.Release();
        TriCountBuffer?.Release();
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
    
    struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this [int i] {
            get {
                switch (i) {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
}
