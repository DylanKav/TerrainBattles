using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private GameObject[,,] TerrainData;
    

    [Header("Terrain Options")] 
    [SerializeField] private Vector3 terrainSize = new Vector3Int(32, 3, 32);
    [Range(1, 100)]
    [SerializeField] int samplesPerAxis = 16;
    [Range(16, 32)]
    [SerializeField] int chunkSizePerAxis = 16;
    [Range(0, 16)]
    [SerializeField] int noiseHeight = 0;
    [SerializeField] float isoLevel = 0.5f;
    [SerializeField] private Texture heightMapImage;
    [SerializeField] private GameObject chunkPrefab;

    [Header("Rendering Options")] 
    [SerializeField]
    private Camera mainCamera;
    [SerializeField] private float viewDistance = 5;
    private Vector3 cameraCurrentChunk;

    private Vector3 CameraCurrentChunk
    {
        get
        {
            return cameraCurrentChunk;
        }
        set
        {
            if (cameraCurrentChunk == value) return;
            StopAllCoroutines();
            cameraCurrentChunk = value;
            DrawVisibleChunks(value);
        }
    }

    public float ViewDistance
    {
        get
        {
            return viewDistance;
        }
        set
        {
            viewDistance = value;
        }
    }
    

    private void Start()
    {
        TerrainData = new GameObject[(int)terrainSize.x, (int)terrainSize.y, (int)terrainSize.z];
        CreateTerrain();
        //StartCoroutine(EnableTerrain());
        
    }

    private void Update()
    {
        var position = mainCamera.transform.position;
        var x = (int)position.x / chunkSizePerAxis;
        var y = (int)position.y / chunkSizePerAxis;
        var z = (int)position.z / chunkSizePerAxis;
        CameraCurrentChunk = new Vector3(x, y, z);
    }

    void CreateTerrain()
    {
        
        RenderTexture heightMapRT = new RenderTexture(heightMapImage.width, heightMapImage.height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        heightMapRT.enableRandomWrite = true;
        Graphics.Blit(heightMapImage, heightMapRT);
        heightMapRT.Create();
        
        for (int x = 0; x < terrainSize.x; x++)
        {
            for (int y = 0; y < terrainSize.y; y++)
            {
                for (int z = 0; z < terrainSize.z; z++)
                {
                    var chunkInstance = Instantiate(chunkPrefab, this.transform);
                    chunkInstance.name = "CHUNK:" + x + "/" + y + "/" + z;
                    var chunk = chunkInstance.GetComponent<BufferTest>();
                    chunk.StartPosition = new Vector3(x * chunkSizePerAxis, y * chunkSizePerAxis, z * chunkSizePerAxis);
                    chunk.SamplesPerAxis = samplesPerAxis;
                    chunk.ChunkSizePerAxis = chunkSizePerAxis;
                    chunk.NoiseHeight = noiseHeight;
                    chunk.IsoLevel = isoLevel;
                    chunk.HeightmapTexture = heightMapRT;
                    TerrainData[x, y, z] = chunkInstance;
                }
            }
        }
    }

    IEnumerator EnableTerrain()
    {
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    yield return new WaitForSeconds(.001f);
                    if(TerrainData[x, y, z] != null) TerrainData[x, y, z].SetActive(true);
                }
            }
        }
    }

    private void DrawVisibleChunks(Vector3 playerChunk)
    {
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    if (TerrainData[x, y, z] == null) return;
                    var chunkComponent = TerrainData[x, y, z].GetComponent<BufferTest>();
                    if (Vector3.Distance(playerChunk, new Vector3(x, y, z)) <= viewDistance)
                    {
                        
                        chunkComponent.MakeVisible();
                    }
                    else
                    {
                        chunkComponent.ClearMesh();
                    }
                }
            }
        }
    }
}
