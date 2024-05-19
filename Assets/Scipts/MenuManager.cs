using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    private static MenuManager instance;
    [SerializeField] private UnityEngine.UI.Slider masterVolumeSlider;
    [SerializeField] private UnityEngine.UI.Slider musicVolumeSlider;
    [SerializeField] private UnityEngine.UI.Slider sfxVolumeSlider;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject restartButton;
    public GameObject SmokeEffect;
    public GameObject EndSmoke;

    public GameObject settingsPanel;
    public GameObject menuPanel;
    public void OnStartClicked()
    {
        SmokeEffect.SetActive(true);
        Invoke(nameof(InvokeOpenLevel), 1f);
    }
    public void OnRestartClicked()
	{
		Invoke(nameof(InvokeMenuLevel), 1f);
	}
    public void InvokeOpenLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void InvokeMenuLevel()
    {
        SceneManager.LoadScene("Menu");
    }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            playButton.SetActive(true);
            restartButton.SetActive(false);
			gameObject.SetActive(true);
		}
        else if (scene.name == "GameScene")
		{
			playButton.SetActive(false);
			restartButton.SetActive(true);
            gameObject.SetActive(false);
		}
		SetVolumeSlider(AudioCategory.master, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.master));
		SetVolumeSlider(AudioCategory.sfx, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.sfx));
		SetVolumeSlider(AudioCategory.music, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.music));
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
	public void OnMasterVolumeChanged(float value)
	{
		GameManager.Instance.audioSystem.SetVolume(AudioCategory.master, value);
	}

	public void OnMusicVolumeChanged(float value)
	{
		GameManager.Instance.audioSystem.SetVolume(AudioCategory.music, value);
	}

	public void OnSfxVolumeChanged(float value)
	{
		GameManager.Instance.audioSystem.SetVolume(AudioCategory.sfx, value);
	}

    public void SetVolumeSlider(AudioCategory category, float value)
    {
        switch (category)
		{
			case AudioCategory.master:
				masterVolumeSlider.value = value;
				break;
			case AudioCategory.music:
                musicVolumeSlider.value = value;
                break;
            case AudioCategory.sfx:
				sfxVolumeSlider.value = value;
				break;
        }
	}
}
