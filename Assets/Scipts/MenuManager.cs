using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private static MenuManager instance;
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private UnityEngine.UI.Slider masterVolumeSlider;
    [SerializeField] private UnityEngine.UI.Slider musicVolumeSlider;
    [SerializeField] private UnityEngine.UI.Slider sfxVolumeSlider;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject restartButton;

    public void OnStartClicked()
    {
        GameManager.Instance.LoadScene("GameScene");
    }

    public void OnRestartClicked()
	{
		GameManager.Instance.LoadScene("GameScene");
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
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
	}

	private void Start()
    {
		if (!PlayerPrefs.HasKey("Started") && SceneManager.GetActiveScene().name == "Menu")
		{
			PlayerPrefs.SetInt("Started", 1);
		}
	}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Menu")
        {
            playButton.SetActive(true);
            restartButton.SetActive(false);
            headingText.text = "SWAMP INVASTION";
			headingText.GetComponent<Animator>().enabled = false;
			gameObject.SetActive(true);
		}
        else if (scene.name == "GameScene")
		{
			playButton.SetActive(false);
			restartButton.SetActive(true);
			headingText.text = "SWAMP INVASTION";
            headingText.GetComponent<Animator>().enabled = false;
			gameObject.SetActive(false);
		}
        else if (scene.name == "Lose")
		{
			headingText.text = "YOU LOSE";
            headingText.GetComponent<Animator>().enabled = true;
			gameObject.SetActive(true);
        }
		SetVolumeSlider(AudioCategory.master, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.master));
		SetVolumeSlider(AudioCategory.sfx, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.sfx));
		SetVolumeSlider(AudioCategory.music, GameManager.Instance.audioSystem.GetVolumeRaw(AudioCategory.music));
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
