using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RowAccessor<T>
{
    public T[] GetColumn(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public T[] GetRow(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public int Level {get; set;}
    
    private int[] currentLevelValues;
    
    public int[,] levelTable = new int[,] {
        { 1, 1, 1 },
        { 2, 1, 1 },
        { 3, 2, 1 }
    };
    
    public static LevelManager Instance { get { return _instance; } }
    
    public event Action<int[]> OnLevelChanged;
    private void Awake()
    {      
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
        {   
            _instance = this;
        }
    }

    private void Start() {
        SetLevel(0);
    }

    public void SetLevel(int value)
    {
        Level = value;
        currentLevelValues = new RowAccessor<int>().GetRow(levelTable, value);
        OnLevelChanged?.Invoke(currentLevelValues);
    }

    public void NextLevel()
    {
        Level++;
        currentLevelValues = new RowAccessor<int>().GetRow(levelTable, Level);
        OnLevelChanged?.Invoke(currentLevelValues);
    }

    public void ResetLevel()
    {
        currentLevelValues = new RowAccessor<int>().GetRow(levelTable, Level);
        OnLevelChanged?.Invoke(currentLevelValues);
    }

    public int[] GetLevelValues()
    {
        return currentLevelValues;
    }

    public void DecreaseLevelBuildingCount(int value)
    {
        currentLevelValues[value]--;
        OnLevelChanged?.Invoke(currentLevelValues);
    }

    public bool CanPlaceBuilding(int value)
    {
        return currentLevelValues[value] > 0;
    }

    public float CalculatePlayerScore()
    {
        var sum = 0f;
        foreach(var building in BuildingManager.Instance.buildings)
        {
            if(building == null)
                continue;
            sum += building.Score;
            ScoreManager.Instance.Add(building.Score);
        }
       
        return  Mathf.Clamp(sum,0, GetMaximumScore());

    }

    public float GetScoreToPass()
    {
        float maxScore = 0;
        for(var i = 0; i < currentLevelValues.Length; i++)
        {
            switch(BuildingManager.Instance.prefabs[i].Id)
            {
                case 0:
                    maxScore += levelTable[Level, i]*20;
                    break;
                case 1:
                    maxScore += levelTable[Level, i]*10;
                    break;
                case 2:
                    maxScore += levelTable[Level, i]*5;
                    break;    
            }
        }
        return maxScore;
    }

    public float GetMaximumScore()
    {
        float maxScore = 0;
        for(var i = 0; i < currentLevelValues.Length; i++)
        {
            switch(BuildingManager.Instance.prefabs[i].Id)
            {
                case 0:
                    maxScore += levelTable[Level, i]*(20 + 1);
                    break;
                case 1:
                    maxScore += levelTable[Level, i]*(10 + 1);
                    break;
                case 2:
                    maxScore += levelTable[Level, i]*(5 + 1);
                    break;    
            }
        }
        return maxScore;
    }
}
