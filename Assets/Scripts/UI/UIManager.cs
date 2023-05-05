using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameObject dialogPrefabLarge;

    [SerializeField]
    [Tooltip("Assign DialogSmall_192x96.prefab")]
    private GameObject dialogPrefabSmall;

    /// <summary>
    /// Small Dialog example prefab to display
    /// </summary>
    public GameObject DialogPrefabSmall
    {
        get => dialogPrefabSmall;
        set => dialogPrefabSmall = value;
    }

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject toolPanel;
    [SerializeField] private TMP_Text[] buildingFreuqencies;
    
    
    // Singleton instance
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    
    private void Awake() 
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);

        else 
        {
            _instance = this;

            // Get the dialog prefab from mrtk package
            string dialogPath = "Assets/Resources/DialogMedium_192x128.prefab";
            dialogPrefabLarge = AssetDatabase.LoadAssetAtPath(dialogPath, typeof(GameObject)) as GameObject;
        }
    }

    private void Start() {
        LevelManager.Instance.OnLevelChanged += SetTextFrequency;
    }
    private void OnDisable() => LevelManager.Instance.OnLevelChanged -= SetTextFrequency;

    public void OnInitializeGame()
    {
        mainPanel.SetActive(true);
    }

    public void OnEndGame()
    {
        gamePanel.SetActive(false);
        endPanel.SetActive(true);
    }

    public void SwitchBuild(bool value)
    {
        buildingPanel.SetActive(value);
        gamePanel.SetActive(!value);
        if (value)
            buildingPanel.GetComponent<RadialView>().enabled = true;
        else
            gamePanel.GetComponent<RadialView>().enabled = true;
    }

    public void SetTextFrequency(int[] frequencies)
    {
        for (var i = 0; i < frequencies.Length; i++)
            buildingFreuqencies[i].text = frequencies[i].ToString(); 
    }

    public void Help() 
    {
        string helpText = "Help text goes here\n And here\n And here\n";
        Dialog.Open(dialogPrefabLarge, DialogButtonType.OK, "Help", helpText, true);
    }  

    /// <summary>
    /// Opens confirmation dialog example
    /// </summary>
    public void OpenConfirmationDialogSmall()
    {
        var dialog = Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Overlay Confirmation", "In case the virtual mode is not correctly overlayed on top of the real model, please align them and then press Ok", false);
        if (dialog != null)
            dialog.OnClosed += GameManager.Instance.InitializeGame;
            
    }

    public void OpenEndPanel(bool isWin)
    {
        endPanel.SetActive(true);
        if (isWin)
        {
            endPanel.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);
            endPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text = "You Won!";
        }
        else
        {
            endPanel.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
            endPanel.transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(false);
            endPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text = "You Lose!";
        }

        endPanel.transform.GetChild(1).transform.GetChild(1).GetComponent<TMP_Text>().text =
            "Score: " + LevelManager.Instance.CalculatePlayerScore();
        endPanel.transform.GetChild(1).transform.GetChild(2).GetComponent<TMP_Text>().text =
            "Score to pass: " + LevelManager.Instance.GetScoreToPass();
        endPanel.transform.GetChild(1).transform.GetChild(3).GetComponent<TMP_Text>().text =
            "Max Score: " + LevelManager.Instance.GetMaximumScore();
        endPanel.GetComponent<RadialView>().enabled = true;

    }
}
