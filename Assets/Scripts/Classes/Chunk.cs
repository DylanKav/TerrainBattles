using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chunk
{
    private Voxel[,,] ChunkData = new Voxel[16, 16, 16];

    public Vector3 StartingPosition = Vector3.zero;

    public MeshFilter _meshFilter;

    public Chunk(Vector3 position, MeshFilter meshFilter)
    {
        StartingPosition = position;
        _meshFilter = meshFilter;
        CreateChunk();
    }

    private void CreateChunk()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    ChunkData[x, y, z] = new Voxel(new Vector3(x, y, z) + StartingPosition);
                }
            }
        }
    }

    void CreateHole(Vector3 position, float radius)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    if (Vector3.Distance(position, new Vector3(x, y, z) + StartingPosition) <= (radius+2))
                    {
                        ChunkData[x, y, z].TogglePointsByDistance(position, radius);
                        //if(ChunkData[x,y,z].Points[0].isOn) Debug.Log("IM ON!!");
                    }
                }
            }
        }
        DrawChunk();
    }

    public void DrawChunk()
    {
        var mesh = new Mesh
        {
            name = "CHUNK:" + StartingPosition.x + "/" + StartingPosition.y + "/" + StartingPosition.z
        };
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        foreach (var voxel in ChunkData)
        {
            foreach (var vec in voxel.CalculateMeshVertices())
            {
                vertices.Add(vec);
            }
        }

        foreach (var vertex in vertices)
        {
            if (tris.Count == 0)
            {
                tris.Add(0);
            }
            else
            {
                tris.Add(tris[^1] + 1);
            }
        }
        

        

        mesh.vertices = vertices.ToArray();
        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
        }

        mesh.uv = uvs;
        Debug.Log(mesh.uv.Length + " " + mesh.vertices.Length);
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        
        _meshFilter.sharedMesh = mesh;
    }

    // Update is called once per frame
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(8, 8, 8) + StartingPosition, new Vector3(16, 16, 16));
    }

    public void GeneratePerlinNoise(int multiplier)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    ChunkData[x,y,z].PerlinsNoise(multiplier);
                }
            }
        }
    }
    
    /*
    private void OnDrawGizmosSelected()
    {
        foreach (var voxel in ChunkData)
        {
            if (voxel == null) return;
            foreach (var point in voxel.Points)
            {
                
                Gizmos.color = point.isOn ? Color.green : Color.red;
                if(point.isOn) Gizmos.DrawSphere(point.Position, 0.05f);
                
            }
        }
    }
    */
}
