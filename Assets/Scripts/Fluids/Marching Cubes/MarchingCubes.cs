using System;
using System.Collections.Generic;
using UnityEngine;


public class MarchingCubes
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    
    public void March(Vector3 position, int[] cube)
    {
        var configIndex = GetCubeConfig(cube);
        
        if (configIndex == 0 || configIndex == 255)
            return;

        var edgeIndex = 0;
        // No more than 5 triangles
        for (var i = 0; i < 5; i++)
        {
            for (var p = 0; p < 3; p++)
            {
                var idx = Table.TriangleTable[configIndex, edgeIndex];
                if (idx == -1)
                    return;

                var edgeTable = Table.GetEdgeTable(0.006f);
                var vert1 = position + edgeTable[idx, 0];
                var vert2 = position + edgeTable[idx, 1];

                var vertPosition = (vert1 + vert2) / 2f;
                
                vertices.Add(vertPosition);
                triangles.Add(vertices.Count - 1);
                
                edgeIndex++;
            }
        }
    }

    int GetCubeConfig(int[] cube)
    {
        var idx = 0;
        for (var i = 0; i < 8; i++)
        {
            if (cube[i] == 0)
                idx |= 1 << i;
        }

        return idx;
    }

    public void ClearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }

    public Mesh BuildMesh()
    {
        var mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}