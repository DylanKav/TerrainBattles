using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class BufferTest : MonoBehaviour
{
    

    [Header("Sampling Options")] 
    public Vector3 StartPosition = Vector3.zero;
    [Range(1, 100)]
    public int SamplesPerAxis = 16;
    [Range(16, 32)]
    public int ChunkSizePerAxis = 16;
    [Range(0, 16)]
    public int NoiseHeight = 0;

    public float IsoLevel = 0.5f;
    
    [Header("SerializedFields")]
    [SerializeField] private ComputeShader VoxelData;
    [SerializeField] private ComputeShader MarchingCubes;
    [SerializeField] private ComputeShader ExplosionShader;
    [SerializeField] private MeshFilter meshFilter;
    //[SerializeField] RenderTexture noiseTexture;
    [SerializeField] private Texture2D heightMapImage;
    
    [Header("Gizmos")] 
    public bool ShowPoints = false;
    
    /// <summary>
    /// disabling this will improve performance
    /// </summary>
    public bool OutputNoiseTexture = true;
    


    //privates
    private ComputeBuffer pointsBuffer;
    private Vector4[] dataCollected; //for point gizmos.
    //marching cubes
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

    private ComputeBuffer heightBuffer;

    private void Start()
    {
        //Generate Buffers
        CreateBuffers();
        //if (OutputNoiseTexture) VoxelData.SetTexture(0, "outputRenderTexture", noiseTexture);
        VoxelData.SetVector("startPosition", new Vector4(StartPosition.x, StartPosition.y, StartPosition.z));
        VoxelData.SetBool("outputRenderTexture", OutputNoiseTexture);
        VoxelData.SetInt("sampleNum", SamplesPerAxis);
        VoxelData.SetInt("chunkSize", ChunkSizePerAxis);
        VoxelData.SetInt("noiseHeight", NoiseHeight);
        VoxelData.SetBuffer(0, "points", pointsBuffer);
        //VoxelData.SetBuffer(0, "feedbackSampleNum", feedbackBuffer);
        VoxelData.Dispatch(0, SamplesPerAxis, SamplesPerAxis, SamplesPerAxis);

        //collect data
        if (ShowPoints)
        {
            dataCollected = new Vector4[pointsBuffer.count];
        

            pointsBuffer.GetData(dataCollected);
        }

        GenerateMesh();
        StartCoroutine(Explosion());
        //float[] feedbackNum = new float[feedbackBuffer.count];
        //feedbackBuffer.GetData(feedbackNum);
        //Debug.Log(feedbackNum[0]);
        //Release the Buffers!


    }

    private void CreateHeightMap()
    {
        float[] heightData = new float[heightMapImage.height * heightMapImage.width];
        var pixelData = heightMapImage.GetPixels();
        for (int i = 0; i < pixelData.Length; i++)
        {
            heightData[i] = pixelData[i].b;
        }
        heightBuffer.SetData(heightData);
        VoxelData.SetBuffer(0, "heightMap", heightBuffer);
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(5);
        ExplodeTerrain(new Vector3(8, 8, 16), 5f);
    }

    private void CreateBuffers()
    {
        int numVoxelsPerAxis = SamplesPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;
        
        pointsBuffer = new ComputeBuffer(SamplesPerAxis * SamplesPerAxis * SamplesPerAxis, sizeof(float) * 4);
        triangleBuffer = new ComputeBuffer (maxTriangleCount, sizeof (float) * 3 * 3, ComputeBufferType.Append);
        triCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);
        heightBuffer = new ComputeBuffer(heightMapImage.height * heightMapImage.width, sizeof(float));
        //if (OutputNoiseTexture) noiseTexture = new RenderTexture(SamplesPerAxis, SamplesPerAxis, 250);
        //feedbackBuffer = new ComputeBuffer(1, sizeof(float));
    }

    private void ClearBuffers()
    {
        pointsBuffer?.Release();
        triangleBuffer?.Release();
        triCountBuffer?.Release();
        heightBuffer?.Release();
    }

    private void OnApplicationQuit()
    {
        ClearBuffers();
    }



    private void ExplodeTerrain(Vector3 position, float radius)
    {
        if (pointsBuffer == null) Debug.Log("Buffer is null");
        ExplosionShader.SetBuffer(0, "points", pointsBuffer);
        ExplosionShader.SetVector("explosionPos", new Vector4(position.x, position.y, position.z));
        ExplosionShader.SetFloat("explosionRadius", radius);
        ExplosionShader.SetInt("sampleNum", SamplesPerAxis);
        ExplosionShader.SetInt("chunkSize", ChunkSizePerAxis);
        ExplosionShader.Dispatch(0, SamplesPerAxis, SamplesPerAxis, SamplesPerAxis);
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        //Time to march!
        //int numVoxelsPerAxis = SamplesPerAxis - 1;
        //int numThreadsPerAxis = Mathf.CeilToInt (numVoxelsPerAxis / (float) 8);
        triangleBuffer.SetCounterValue (0);
        MarchingCubes.SetInt("numPointsPerAxis", SamplesPerAxis);
        MarchingCubes.SetFloat("isoLevel", IsoLevel);
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireCube(new Vector3(ChunkSizePerAxis/2 + StartPosition.x, ChunkSizePerAxis/2+ StartPosition.y, ChunkSizePerAxis/2+ StartPosition.z), new Vector3(ChunkSizePerAxis, ChunkSizePerAxis, ChunkSizePerAxis));
        if (!ShowPoints) return;
        if (dataCollected == null) return;
        foreach (var point in dataCollected)
        {
            Gizmos.color = point.w > IsoLevel? Color.white : Color.black;
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
