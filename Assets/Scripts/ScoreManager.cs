using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public float Score { get; set; }
    
    public static ScoreManager Instance
    {
        get 
        { 
            if (_instance == null)
            {
                var go = new GameObject();
                _instance = go.AddComponent<ScoreManager>();
                DontDestroyOnLoad(go);
            }
            return _instance; 
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }   
        else
            Destroy(gameObject);

    }
    
    public void Subtract(float value)
    { 
        Add(-value);
    }

    public void Add(float value)
    {
        Score += value;
    }

    public void ResetScore()
    {
        Score = 0;
    }
}
