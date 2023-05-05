using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GradientPath
{
    [SerializeField, HideInInspector]
    private List<(Vector3, Vector2, Vector2, float)> points;

    public GradientPath()
    {
        points = new List<(Vector3, Vector2, Vector2, float)>();
    }

    public void AddPoint(Vector3 point, Vector2 gradient, Vector2 direction, float speed)
    {
        points.Add((point, gradient, direction, speed));
    }

    public bool IsEmpty()
    {
        return points.Count == 0;
    }

    public List<(Vector3, Vector2, Vector2, float)> GetPoints()
    {
        return new List<(Vector3, Vector2, Vector2, float)>(points);
    }
}
