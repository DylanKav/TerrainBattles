using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private GameObject[,,] TerrainData = new GameObject[16, 2, 16];
    [SerializeField] private GameObject chunkPrefab;
    [Header("Terrain Options")]
    [Range(1, 100)]
    [SerializeField] int samplesPerAxis = 16;
    [Range(16, 32)]
    [SerializeField] int chunkSizePerAxis = 16;
    [Range(0, 16)]
    [SerializeField] int noiseHeight = 0;
    [SerializeField] float isoLevel = 0.5f;

    private void Start()
    {
        CreateTerrain();
        StartCoroutine(EnableTerrain());
        
    }

    void CreateTerrain()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 8; z++)
                {
                    var chunkInstance = Instantiate(chunkPrefab, this.transform);
                    chunkInstance.name = "CHUNK:" + x + "/" + y + "/" + z;
                    var chunk = chunkInstance.GetComponent<BufferTest>();
                    chunk.StartPosition = new Vector3(x * chunkSizePerAxis, y * chunkSizePerAxis, z * chunkSizePerAxis);
                    chunk.SamplesPerAxis = samplesPerAxis;
                    chunk.ChunkSizePerAxis = chunkSizePerAxis;
                    chunk.NoiseHeight = noiseHeight;
                    chunk.IsoLevel = isoLevel;
                    TerrainData[x, y, z] = chunkInstance;
                }
            }
        }
    }

    IEnumerator EnableTerrain()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    yield return new WaitForSeconds(.001f);
                    if(TerrainData[x, y, z] != null) TerrainData[x, y, z].SetActive(true);
                }
            }
        }
    }
}
