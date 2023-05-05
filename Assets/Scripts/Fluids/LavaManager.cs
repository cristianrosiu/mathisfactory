using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GradientCreator))]
public class LavaManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float spawnRadius;
    [SerializeField] private GradientCreator creator;
    [SerializeField] private int numberOfLavaPoints = 20;
    [SerializeField] private GameObject lavaPrefab;
    [SerializeField] private Transform volcanoParent;

    private bool unPause = true;

    public List<GradientPath> paths;
    private List<GameObject> lavaGameObjects;

    public LavaMeshGenerator meshGenerator;
    
    private static LavaManager _instance;
    
    public static LavaManager Instance { get { return _instance; } }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
        creator = gameObject.GetComponent<GradientCreator>();
        lavaGameObjects = new List<GameObject>();
        paths = new List<GradientPath>();
    }

    /// <summary>
    /// Clears all paths and lava prefabs and respawns everything.
    /// </summary>
    public void ResetLava()
    {   
        // Clean up current lava
        foreach(var obj in lavaGameObjects) 
            Destroy(obj);
            
        paths.Clear();
        lavaGameObjects.Clear();

        unPause = true;
        // Spawn new lava spheres 
        SpawnLava();
    }

    public void ClearLava()
    {
         // Clean up current lava
        foreach(var obj in lavaGameObjects) 
            Destroy(obj);
        paths.Clear();
        lavaGameObjects.Clear();
    }
    
    /// <summary>
    /// Based on the number of lava points it:
    ///     1. Picks a random point on the rim of volcano
    ///     2. Calculates the gradient path points
    ///     3. Instantiates the lava
    /// </summary>
    public Vector3[] SpawnLava()
    {
        var spawnPositions = new Vector3[numberOfLavaPoints];
        var lavaPoints = 0;
        while (lavaPoints < numberOfLavaPoints)
        {
            var startingPosition = RandomPointInSphere();
            spawnPositions[lavaPoints] = startingPosition;
            // Generate the gradient path
            creator.CreateGradient(startingPosition);
            if (creator.path.IsEmpty())
                continue;            
            // Instantiate the lava game object and set its properties
            var sphere = Instantiate(lavaPrefab, creator.path.GetPoints()[0].Item1, Quaternion.identity);
            sphere.GetComponent<LavaController>().path = creator.path;
            sphere.transform.parent = volcanoParent;
            lavaGameObjects.Add(sphere);
            
            // Add the path to the list of all paths
            paths.Add(creator.path);

            lavaPoints++;
        }

        return spawnPositions;
    }
    public void SpawnLava(Vector3[] startPositions)
    {
        foreach (var pos in startPositions)
        {
            // Generate the gradient path
            creator.CreateGradient(pos);
            if (creator.path.IsEmpty())
                continue;            
            // Instantiate the lava game object and set its properties
            var sphere = Instantiate(lavaPrefab, creator.path.GetPoints()[0].Item1, Quaternion.identity);
            sphere.GetComponent<LavaController>().path = creator.path;
            sphere.transform.parent = volcanoParent;
            lavaGameObjects.Add(sphere);
            
            // Add the path to the list of all paths
            paths.Add(creator.path);
        }
    }
    
    /// <summary>
    /// Iterates over all lava points and starts the simulation
    /// </summary>
    public void SimulateLava()
    {
        foreach(var point in lavaGameObjects)
            point.GetComponent<LavaController>().startRolling = unPause;
        unPause = !unPause;
    }
    
    /// <summary>
    /// Picks a random point on the rim of volcano
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomPointInSphere()
    {
        return new Vector3(Random.Range(spawnPosition.position.x - spawnRadius, spawnPosition.position.x + spawnRadius),
                                  0,
                                  Random.Range(spawnPosition.position.z - spawnRadius, spawnPosition.position.z + spawnRadius));
    }


    public bool IsSimulationDone()
    {
        foreach (var obj in lavaGameObjects)
            if (!obj.GetComponent<LavaController>().IsDone())
                return false;
    

        return true;
    }

    public bool IsSimulationStarted()
    {
        foreach (var obj in lavaGameObjects)
        {
            if (obj.GetComponent<LavaController>().startRolling)
                return true;
        }

        return false;
    }

    public void RewindLava(int changRate)
    {
        foreach (var lava in lavaGameObjects)
            lava.GetComponent<LavaController>().Rewind(changRate);
        meshGenerator.UpdateMesh();
    }

    private void OnDrawGizmos()
    {
        foreach (var path in paths)
        {
            foreach (var point in path.GetPoints())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(point.Item1, 0.002f);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawnPosition.position, spawnRadius);
    }
}
