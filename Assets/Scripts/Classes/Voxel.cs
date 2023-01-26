using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Voxel
{
    public Point3[] Points = new Point3[8];
    List<Vector3> vertices = new List<Vector3>();

    
    public Voxel(Vector3 position)
    {
        Points[0] = new Point3(position);
        Points[1] = new Point3(position + new Vector3(1, 0, 0));
        Points[2] = new Point3(position + new Vector3(1, 0, 1));
        Points[3] = new Point3(position + new Vector3(0, 0, 1));
        Points[4] = new Point3(position + new Vector3(0, 1, 0));
        Points[5] = new Point3(position + new Vector3(1, 1, 0));
        Points[6] = new Point3(position + new Vector3(1, 1, 1));
        Points[7] = new Point3(position + new Vector3(0, 1, 1));

        /*
        Points[0].Position = position;
        Points[1].Position = position + new Vector3(1, 0, 0);
        Points[2].Position = position + new Vector3(1, 0, 1);
        Points[3].Position = position + new Vector3(0, 0, 1);
        Points[4].Position = position + new Vector3(0, 1, 0);
        Points[5].Position = position + new Vector3(1, 1, 0);
        Points[6].Position = position + new Vector3(1, 1, 1);
        Points[7].Position = position + new Vector3(0, 1, 1);
        */

    }

    public int NumberOfTriangles
    {
        get
        {
            //if((vertices.Count / 3) != 0) Debug.Log("TRIS = " + vertices.Count / 3);
            return vertices.Count / 3;
        }
    }

    public Vector3 Position
    {
        get
        {
            return Points[0].Position + new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void ToggleVoxel(bool state)
    {
        foreach (var point in Points)
        {
            point.ToggleState(state);
        }
    }

    public void TogglePointsByDistance(Vector3 position, float radius)
    {
        //var newPoints = new Point3[8];
        for (var index = 0; index < Points.Length; index++)
        {
            var point = Points[index];
            if (Vector3.Distance(point.Position, position) <= radius)
            {
                point.ToggleState(true);
            }

            //newPoints[index] = point;
        }

        //Points = newPoints;
    }

    public List<Vector3> CalculateMeshVertices()
    {
        vertices.Clear();
        
        string binaryValue;
        binaryValue = (Points[7].isOn ? "1" : "0") + (Points[6].isOn ? "1" : "0") + (Points[5].isOn ? "1" : "0") + (Points[4].isOn ? "1" : "0") + (Points[3].isOn ? "1" : "0") +
                      (Points[2].isOn ? "1" : "0") + (Points[1].isOn ? "1" : "0") + (Points[0].isOn ? "1" : "0");

        //Debug.Log(binaryValue);
        var indexToLookAt = Convert.ToInt32(binaryValue, 2);

        var edges = TriangulationTable.Triangulate(indexToLookAt);

        foreach (var edgeIndex in edges)
        {
            if (edgeIndex == -1) continue;
            var edgeVertices = GetVerticesOfEdge(edgeIndex);

            Vector3 vertexPos = (Points[edgeVertices[0]].Position + Points[edgeVertices[1]].Position) / 2;
            vertices.Add(vertexPos);
        }


        return vertices;
    }

    private int[] GetVerticesOfEdge(int edgeNo)
    {
        int[] result = new int[2];
        switch (edgeNo)
        {
            case 0:
                result[0] = 0;
                result[1] = 1;
                break;
            case 1:
                result[0] = 1;
                result[1] = 2;
                break;
            case 2:
                result[0] = 2;
                result[1] = 3;
                break;
            case 3:
                result[0] = 0;
                result[1] = 3;
                break;
            case 4:
                result[0] = 4;
                result[1] = 5;
                break;
            case 5:
                result[0] = 5;
                result[1] = 6;
                break;
            case 6:
                result[0] = 6;
                result[1] = 7;
                break;
            case 7:
                result[0] = 7;
                result[1] = 4;
                break;
            case 8:
                result[0] = 0;
                result[1] = 4;
                break;
            case 9:
                result[0] = 1;
                result[1] = 5;
                break;
            case 10:
                result[0] = 2;
                result[1] = 6;
                break;
            case 11:
                result[0] = 3;
                result[1] = 7;
                break;
            default:
                result[0] = -1;
                result[1] = -1;
                break;
        }

        return result;
    }

    public void PerlinsNoise(int multiplier)
    {
        if (Mathf.PerlinNoise(Points[0].Position.x / 50, Points[0].Position.z / 50) * multiplier <=
            Points[0].Position.y-1) return;
        foreach (var point in Points)
        {
            if (Mathf.PerlinNoise(point.Position.x /50, point.Position.z /50)*multiplier >= point.Position.y)
            {
                point.isOn = true;
            }
        }
    }
    
}
