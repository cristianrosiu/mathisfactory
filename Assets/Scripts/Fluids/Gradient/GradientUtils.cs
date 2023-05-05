using UnityEngine;

public class GradientUtils : MonoBehaviour
{
    public struct HeightAndGradient {
        public float height;
        public Vector2 gradient;
    }

    public static HeightAndGradient CalculateHeightAndGradient(Vector2 pos, TerrainData map)
    {
        var coordX = (int)pos.x;
        var coordY = (int)pos.y;

        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        var u = pos.x - coordX;
        var v = pos.y - coordY;

        // Calculate heights of the four nodes of the droplet's cell
        var heightSW = map.GetHeight(coordX, coordY);
        var heightSE = map.GetHeight(coordX + 1, coordY);
        var heightNW = map.GetHeight(coordX, coordY - 1);
        var heightNE = map.GetHeight(coordX + 1, coordY - 1);
        
        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        var gradientX = (heightNE - heightNW) * (1 - v) + (heightSE - heightSW) * v;
        var gradientY = (heightSW - heightNW) * (1 - u) + (heightSE - heightNE) * u;

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        var height = heightNW * (1 - u) * (1 - v) + heightNE * u * (1 - v) + heightSW * (1 - u) * v + heightSE * u * v;

        return new HeightAndGradient () { height = height, gradient = new Vector2(gradientX, gradientY)};
    }

    public static Vector3 WorldPointToHMap(Vector3 worldPoint, float y, int mapSize, TerrainData map)
    {
        return new Vector3(
            worldPoint.x * mapSize / map.size.x, 
            y,
            worldPoint.z * mapSize / map.size.z
            );
    }
    public static Vector3 MapPointToWorld(Vector2 mapPoint, float y, int mapSize, TerrainData map)
    {
        return new Vector3( 
            mapPoint.x / mapSize * map.size.x, 
            y - 0.02f, 
            mapPoint.y / mapSize * map.size.z
            );
    }
}
