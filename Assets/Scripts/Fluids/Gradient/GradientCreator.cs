using System;
using UnityEditor.UIElements;
using UnityEngine;
public class GradientCreator : MonoBehaviour
{
    [SerializeField]
    private Terrain volcano;

    [SerializeField]
    private float inertia = .15f;
    
    [SerializeField]
    private int maxDropletLifetime = 100;

    private TerrainData data;
    private int mapSize;
    
    [SerializeField]
    public GradientPath path;

    private float gravity = 4f;

    public void Awake()
    {
        data = volcano.terrainData;
        mapSize = data.heightmapResolution;
    }

    public void CreateGradient(Vector3 startingPosition)
    {
        path = new GradientPath();
        var point = GradientUtils.WorldPointToHMap(startingPosition, startingPosition.y, mapSize, data);

        if (point.x < 0 || point.x >= mapSize || point.z < 0 || point.z >= mapSize)
            return;

        var pos = new Vector2(point.x, point.z);
        var dir = new Vector2(0, 0);
        var speed = 5f;

        for (var lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
        {
            // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
            var hag = GradientUtils.CalculateHeightAndGradient(pos, data);
            var oldHeight = hag.height;

            dir = dir * inertia - hag.gradient*(1-inertia);
            var len = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);

            
            if (len != 0)
                dir /= len;

            pos += dir;

            // Stop simulating droplet if it's not moving or has flowed over edge of map
            if ((dir.x == 0 && dir.y == 0) || pos.x < 0 || pos.x >= mapSize - 1 || pos.y < 0 || pos.y >= mapSize - 1)
                break;
            
            
            // Find the droplet's new height and calculate the deltaHeight
            var newHeight = GradientUtils.CalculateHeightAndGradient(pos, data).height;
            var deltaHeight = newHeight - oldHeight;
          
            // Update droplet's speed and water content
            speed = Mathf.Sqrt (speed * speed + deltaHeight * gravity);
            
            path.AddPoint(GradientUtils.MapPointToWorld(pos, newHeight, mapSize, data), GradientUtils.CalculateHeightAndGradient(pos, data).gradient, dir, speed);
        }
    }
}


