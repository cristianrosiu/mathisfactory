using UnityEngine;

public class Grid : MonoBehaviour
{   
    // Grid resolution
    public float res = 0.5f;
    public GameObject[,,] grd;
    public Vector3 GridSize { get; set; }
    
    [SerializeField] private Vector3 gridWorldSize;
    
    // Prefab for the grid point
    [SerializeField] private GameObject grdPoint;
    
    private void Awake()
    {
       InitializeGrid();
    }
    
    void InitializeGrid()
    {
        // Creates the 3D grid
        GridSize = gridWorldSize / res;
        grd = new GameObject[(int)GridSize.x + 1, (int)GridSize.y + 1, (int)GridSize.z + 1];
        // Defines and instantiates the grid points
        for (var x = 0; x < (int)GridSize.x + 1; x++)
        {
            for (var y = 0; y < (int)GridSize.y + 1; y++)
            {
                for (var z = 0; z < (int)GridSize.z + 1; z++)
                {
                    grd[x, y, z] = Instantiate(grdPoint, new Vector3(x*res, y*res, z*res), Quaternion.identity);
                    grd[x, y, z].transform.parent = transform;
                    grd[x, y, z].transform.localPosition = new Vector3(x*res, y*res, z*res);
                }
            }
        }
    }
}
