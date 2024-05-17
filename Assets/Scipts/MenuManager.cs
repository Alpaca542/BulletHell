using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject SmokeEffect;
    public GameObject EndSmoke;

    public GameObject settingsPanel;
    public GameObject menuPanel;
    public void OnStartClicked()
    {
        SmokeEffect.SetActive(true);
        Invoke(nameof(InvokeOpenLevel), 1f);
    }
    public void InvokeOpenLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void Start()
    {
        EndSmoke.SetActive(true);
        if (!PlayerPrefs.HasKey("Started") && SceneManager.GetActiveScene().name == "Menu")
        {
            EndSmoke.SetActive(false);
            PlayerPrefs.SetInt("Started", 1);
        }
        else
        {
            Invoke(nameof(InvokeSmokeStop), 3f);
        }
    }
    public void InvokeSmokeStop()
    {
        EndSmoke.SetActive(false);
    }
    public void OpenSetting()
    {
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}