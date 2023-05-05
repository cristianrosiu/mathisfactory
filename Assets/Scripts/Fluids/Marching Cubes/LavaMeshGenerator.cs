using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LavaMeshGenerator : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Grid grid;

    private MarchingCubes marchingCubes;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        grid = GetComponent<Grid>();
        marchingCubes = new MarchingCubes();
    }

    private void Update()
    {
        if (LavaManager.Instance.IsSimulationDone())
            return;
        UpdateMesh();
    }

    /// <summary>
    /// Creates the lava Mesh by marching over the grid
    /// </summary>
    void CreateLavaMesh()
    {
        for (var x = 0; x < (int)grid.GridSize.x; x++)
        {
            for (var y = 0; y < (int)grid.GridSize.y; y++)
            {
                for (var z = 0; z < (int)grid.GridSize.z; z++)
                {
                    var point = grid.grd[x,y,z].GetComponent<GridPoint>();
                    
                    var cube = new int[8];
                    for (var i = 0; i < 8; i++)
                    {
                        var corner = new Vector3Int(x, y, z) + Table.CornerTable[i];
                        cube[i] = grid.grd[corner.x, corner.y, corner.z].GetComponent<GridPoint>().IsoValue;
                    }
                    
                    marchingCubes.March(point.transform.localPosition, cube);
                }
            }
        }

        meshFilter.mesh = marchingCubes.BuildMesh();
    }

    public void UpdateMesh()
    {
        marchingCubes.ClearMeshData();
        CreateLavaMesh();
    }

    public void ResetMesh()
    {
        meshFilter.mesh.Clear();
        marchingCubes.ClearMeshData();

        foreach (var point in grid.grd)
            point.GetComponent<GridPoint>().IsoValue = 0;
    }
}
