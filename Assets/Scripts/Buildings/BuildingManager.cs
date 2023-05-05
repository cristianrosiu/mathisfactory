 using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public HouseController selectedBuilding;
    public Terrain terrain;
    public GameObject arrowPrefab;
    
    [SerializeField] private Transform volcanoParent;

    public List<HouseController> buildings;
    public HouseController[]  prefabs;

    // Offset vectors for computing neighbouring positions
    private Vector2[] offsets = new Vector2[8];
    private Vector2[] endpointOffsets = new Vector2[8];
    private bool selectGradient = false;
    
    
    private static BuildingManager _instance;
    
    public static BuildingManager Instance { get { return _instance; } }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    private void Start()
    {
        int offset = 1;

        // Initialize offsets
        offsets[0] = new Vector2(offset, 0);
        offsets[1] = new Vector2(-offset, 0);

        offsets[2] = new Vector2(0, offset);
        offsets[3] = new Vector2(0, -offset);

        offsets[4] = new Vector2(offset, offset);
        offsets[5] = new Vector2(-offset, -offset);
        offsets[6] = new Vector2(-offset, offset);
        offsets[7] = new Vector2(offset, -offset);

        int endpointOffset = 5;
        endpointOffsets[0] = new Vector2(endpointOffset, 0);
        endpointOffsets[1] = new Vector2(-endpointOffset, 0);

        endpointOffsets[2] = new Vector2(0, endpointOffset);
        endpointOffsets[3] = new Vector2(0, -endpointOffset);

        endpointOffsets[4] = new Vector2(endpointOffset, endpointOffset);
        endpointOffsets[5] = new Vector2(-endpointOffset, -endpointOffset);
        endpointOffsets[6] = new Vector2(-endpointOffset, endpointOffset);
        endpointOffsets[7] = new Vector2(endpointOffset, -endpointOffset);

        SelectBuilding(0);
    }

    // Switch to building placement mode
    public void SelectBuilding(int id)
    {
        selectedBuilding = prefabs[id];
        selectedBuilding.Id = id;
        selectGradient = false;
    }

    public void ResetBuildings()
    {
        foreach (var building in buildings)
            if(building != null)
                Destroy(building.gameObject);
        buildings.Clear();
        SelectBuilding(0);
        
    }

    // Switch to gradient display mode
    public void SelectGradient()
    {
        selectedBuilding = null;
        selectGradient = true;
    }

    public void PerformAction()
    {
        if (selectedBuilding)
            ConstructBuilding();

        else if (selectGradient)
            DisplaySteepestDescent();
      
    }

    public void ConstructBuilding()
    {
        if (LavaManager.Instance.IsSimulationStarted() && !LavaManager.Instance.IsSimulationDone())
            return;

        if (selectedBuilding && LevelManager.Instance.CanPlaceBuilding(selectedBuilding.Id))
        {
            foreach (var source in CoreServices.InputSystem.DetectedInputSources)
            {
                // Ignore anything that is not a hand because we want articulated hands
                if (source.SourceType == InputSourceType.Hand)
                {
                    foreach (var pointer in source.Pointers)
                    {
                        if (pointer is IMixedRealityNearPointer || pointer.Result is null)
                            continue;

                        var hitObject = pointer.Result.Details.Object;
                        if (!hitObject)
                            continue;

                        var endPoint = pointer.Result.Details.Point;
                        
                        var building = Instantiate(selectedBuilding, endPoint, Quaternion.identity);
                        building.transform.parent = GameObject.FindWithTag("Volcano").transform;
                        building.transform.localPosition = endPoint + new Vector3(0, 0.02f, 0);
                        building.Score += (building.transform.localPosition.y / 0.045f);
                        buildings.Add(building.GetComponent<HouseController>());
                        break;
                        
                    }
                }
            }
            LevelManager.Instance.DecreaseLevelBuildingCount(selectedBuilding.Id);
        }
    }

    private void DisplaySteepestDescent()
    {
        if (LavaManager.Instance.IsSimulationStarted() && !LavaManager.Instance.IsSimulationDone())
            return;

        // For testing purposes we just display derivatives if we didnt select a building
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == InputSourceType.Hand)
            {
                foreach (var pointer in source.Pointers)
                {
                    if (pointer is IMixedRealityNearPointer || pointer.Result is null)
                        continue;

                    var hitObject = pointer.Result.Details.Object;
                    if (!hitObject)
                        continue;

                    var endPoint = pointer.Result.Details.Point;
                    DrawGradients(endPoint);
                }
            }
        }
    }

    private void DrawGradients(Vector3 selectPoint)
    { 
        // HMAP point of selectPoint
        Vector3 hmapPoint = GradientUtils.WorldPointToHMap(selectPoint, selectPoint.y, terrain.terrainData.heightmapResolution, terrain.terrainData);
        Vector2 endPoint = new Vector2(hmapPoint.x, hmapPoint.z);

        // Obtain all points we want to display arrows for
        Vector2[] points = new Vector2[9];
        points[0] = endPoint;
        for (int i = 0; i != 8; ++i)
            points[i + 1] = endPoint + endpointOffsets[i];
        
        for (int i = 0; i != 9; ++i)
        {
            // TODO: Make sure points are within the world boundaries

            // Get world point
            float height = terrain.terrainData.GetHeight((int)points[i].x, (int)points[i].y);
            Vector3 worldPoint = GradientUtils.MapPointToWorld(points[i], height, terrain.terrainData.heightmapResolution, terrain.terrainData);

            // Gradient direction point
            Vector3 gradientDirection = FindGradientDirection(points[i]);

            // Only draw arrow where there is a gradient (no flat surfaces)
            if (worldPoint != gradientDirection)
            {
                var offsetVector = new Vector3(0, -0.02f, 0);
                // Create arrow and update orientation
                var arrow = Instantiate(arrowPrefab, worldPoint, Quaternion.identity);
                arrow.transform.parent = volcanoParent;
                arrow.transform.LookAt(gradientDirection);
                arrow.transform.eulerAngles = new Vector3(-arrow.transform.eulerAngles.x, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.z);
                StartCoroutine(DestroyArrow(arrow));
            }
        }
    }

    // Destroy arrows after displaying them for 3 seconds
    IEnumerator DestroyArrow(GameObject arrow)
    {
        yield return new WaitForSeconds(3);

        Destroy(arrow);

        yield return null;
    }


    private Vector3 FindGradientDirection(Vector2 start)
    {
        // Find neighbour with lowest y-value (steepest descent)
        // Note that this is in HeightMap coordinates
        Vector2 gradientPoint = start;
        float lowestHeight = terrain.terrainData.GetHeight((int)start.x, (int)start.y);

        // Look at its 8 neighbours and pick neighbour with steepest descent
        for (int i = 0; i != 8; ++i)
        {
            int posX = (int)start.x + (int)offsets[i].x;
            int posY = (int)start.y + (int)offsets[i].y;
            float currentHeight = terrain.terrainData.GetHeight(posX, posY);

            if (currentHeight < lowestHeight) {
                gradientPoint = new Vector2(posX, posY);
                lowestHeight = currentHeight;
            }
        }

        // Translate back to world coordinates
        Vector3 gradientDirection = GradientUtils.MapPointToWorld(gradientPoint, lowestHeight, terrain.terrainData.heightmapResolution, terrain.terrainData);

        return gradientDirection;
    }
}


