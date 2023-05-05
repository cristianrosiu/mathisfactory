using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private LavaMeshGenerator lavaMesh;
    [SerializeField] private GameObject grid;
    [SerializeField] private float winThreshold = 0.4f;
    [SerializeField] private Terrain terrain;
    [SerializeField] private MeshRenderer levelCurves;

    private bool play = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else 
            _instance = this;
    }

    private void Start()
    {
        LevelManager.Instance.GetMaximumScore();
        UIManager.Instance.OpenConfirmationDialogSmall();
    }

    private IEnumerator LevelPlaying()
    {
        
        while (play && !LavaManager.Instance.IsSimulationDone())
            yield return null;

        OnGameOver();
    }

    public void Play()
    {
        play = true;
        LavaManager.Instance.ResetLava();
        LavaManager.Instance.SpawnLava();
        LavaManager.Instance.SimulateLava();
        grid.SetActive(true);
        StartCoroutine(LevelPlaying());
    }

    public void Reset()
    {
        play = false;
        LavaManager.Instance.ClearLava();
        ScoreManager.Instance.ResetScore();
        BuildingManager.Instance.ResetBuildings();
        LevelManager.Instance.ResetLevel();
        lavaMesh.ResetMesh();
        grid.SetActive(false);
    }
    public void InitializeGame(DialogResult obj)
    {
        UIManager.Instance.OnInitializeGame();
    }

    public void OnGameOver()
    {
        if (LevelManager.Instance.CalculatePlayerScore() >= LevelManager.Instance.GetScoreToPass())
            UIManager.Instance.OpenEndPanel(true);
        else
            UIManager.Instance.OpenEndPanel(false);
    }

    public void OnWinGame()
    {
        if (LevelManager.Instance.Level >= 3)
            return;
        LevelManager.Instance.NextLevel();
        Reset();
    }

    public void SwitchVolcano()
    {
        terrain.enabled = !terrain.enabled;
    }

     public void SwitchLevelCurves()
    {
        levelCurves.enabled = !levelCurves.enabled;
    }
}
