using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BufferTest : MonoBehaviour
{
    [Header("Sampling Options")]
    [Range(1, 50)]
    public int SamplesPerAxis = 16;
    [Range(16, 32)]
    public int ChunkSizePerAxis = 16;
    [Range(0, 16)]
    public int NoiseHeight = 0;
    
    [Header("SerializedFields")]
    [SerializeField] private ComputeShader VoxelData;
    [SerializeField] private ComputeShader MarchingCubes;
    [SerializeField] private MeshFilter meshFilter;


    //privates
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer feedbackBuffer; //for debugging shader.
    private Vector4[] dataCollected; //for point gizmos.
    //marching cubes
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

    private void CreateBuffers()
    {
        int numVoxelsPerAxis = SamplesPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;
        
        pointsBuffer = new ComputeBuffer(SamplesPerAxis * SamplesPerAxis * SamplesPerAxis, sizeof(float) * 4);
        triangleBuffer = new ComputeBuffer (maxTriangleCount, sizeof (float) * 3 * 3, ComputeBufferType.Append);
        
        triCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);
        //feedbackBuffer = new ComputeBuffer(1, sizeof(float));
    }

    private void ClearBuffers()
    {
        pointsBuffer?.Release();
        feedbackBuffer?.Release();
        triangleBuffer?.Release();
        triCountBuffer?.Release();
        Debug.Log("Buffers released");
    }

    private void Start()
    {
        //Generate Buffers
        CreateBuffers();
        
        VoxelData.SetInt("sampleNum", SamplesPerAxis);
        VoxelData.SetInt("chunkSize", ChunkSizePerAxis);
        VoxelData.SetInt("noiseHeight", NoiseHeight);
        VoxelData.SetBuffer(0, "points", pointsBuffer);
        //VoxelData.SetBuffer(0, "feedbackSampleNum", feedbackBuffer);
        VoxelData.Dispatch(0, SamplesPerAxis, SamplesPerAxis, SamplesPerAxis);
        
        
        //collect data
        dataCollected = new Vector4[pointsBuffer.count];
        pointsBuffer.GetData(dataCollected);
        
        //float[] feedbackNum = new float[feedbackBuffer.count];
        //feedbackBuffer.GetData(feedbackNum);
        //Debug.Log(feedbackNum[0]);
        //Release the Buffers!
        
        //Time to march!
        triangleBuffer.SetCounterValue (0);
        MarchingCubes.SetInt("numPointsPerAxis", SamplesPerAxis);
        MarchingCubes.SetBuffer(0, "points", pointsBuffer);
        MarchingCubes.SetBuffer(0, "triangles", triangleBuffer);
        MarchingCubes.Dispatch(0, SamplesPerAxis, SamplesPerAxis, SamplesPerAxis);
        
        //Draw the mesh
        ComputeBuffer.CopyCount (triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData (triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData (tris, 0, 0, numTris);

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
        meshFilter.sharedMesh = mesh;
        ClearBuffers();
    }

    private void OnDrawGizmos()
    {
        if (dataCollected == null) return;
        foreach (var point in dataCollected)
        {
            Gizmos.color = Math.Abs(point.w - 1f) < .1f ? Color.white : Color.black;
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
