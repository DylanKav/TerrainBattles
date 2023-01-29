using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Chunk[,,] TerrainData = new Chunk[1, 1, 1];
    [SerializeField] private int PerlinMultiplier = 5;
    [SerializeField] private GameObject chunkPrefab;

    private void Start()
    {
        CreateTerrain();
        PerlinNoise();
        DrawAllChunks();
    }

    void CreateTerrain()
    {
        for (int x = 0; x < 1; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                for (int z = 0; z < 1; z++)
                {
                    var chunkInstance = Instantiate(chunkPrefab, this.transform);
                    chunkInstance.name = "CHUNK:" + x + "/" + y + "/" + z;
                    TerrainData[x, y, z] = new Chunk(new Vector3(x * 16, y * 16, z * 16), chunkInstance.GetComponent<MeshFilter>());
                }
            }
        }
    }

    void PerlinNoise()
    {
        foreach (var chunk in TerrainData)
        {
            chunk.GeneratePerlinNoise(PerlinMultiplier);
        }

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                Debug.Log(Mathf.PerlinNoise(i, j));
            }
        }
    }

    void DrawAllChunks()
    {
        foreach (var chunk in TerrainData)
        {
            chunk.DrawChunk();
        }
    }
    
}
