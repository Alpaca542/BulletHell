
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private AudioClip _music;
    [SerializeField] private AudioClip menumusic;
    [SerializeField] private AudioClip _bossMusic;
	[SerializeField] private GameObject _sceneLoadAnimationPrefab;
	[SerializeField] private GameObject _sceneLoadEndAnimationPrefab;

	public static GameManager Instance;
    public readonly ObjectPool pool = new ObjectPool();
	public readonly AudioSystem audioSystem = new AudioSystem();
	private GameObject _menu;
	private GameObject _musicObject;
	private GameObject _sceneLoadEndAnimationObject;
	private Coroutine _loadSceneCoroutine;
	private bool _loadSceneCoroutineRunning = false;

	private void Awake()
	{
        Time.timeScale = 1f;
        if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			_menu = FindObjectOfType<MenuManager>(true).gameObject;
			SceneManager.sceneLoaded += OnSceneWasLoaded;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!_menu.activeInHierarchy)
			{
                Time.timeScale = 0.02f;
            }
			else
			{
                Time.timeScale = 1f;
            }
            _menu.SetActive(!_menu.activeInHierarchy);
		}
    }

	void OnEnable()
    {
		LoadPoolablePrefabs();
		if (_musicObject == null)
		{
			_musicObject = audioSystem.PlayClip(menumusic, new AudioClipSettings { looping = true, forcePlay = true, category = AudioCategory.music }).gameObject;
			_musicObject.name = "MainMusic";
			DontDestroyOnLoad(_musicObject.gameObject);
		}
	}

	private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
        pool.Clear();
		_sceneLoadEndAnimationObject = Instantiate(_sceneLoadEndAnimationPrefab);
		_sceneLoadEndAnimationObject.transform.position = Camera.main.transform.position + new Vector3(0f, 10f, 1f);
		Invoke(nameof(RemoveSceneLoadEndAnimation), 3f);
    }

	public void SetMainMusic()
	{
		AudioSource audioSource = _musicObject.GetComponent<AudioSource>();
        StartCoroutine(MixAudio(audioSource, _music));
    }

    public void LoadScene(string name)
	{
        Time.timeScale = 1f;
        if (!_loadSceneCoroutineRunning)
			_loadSceneCoroutine = StartCoroutine(LoadSceneCoroutine(name));
	}

    private IEnumerator MixAudio(AudioSource nowPlaying, AudioClip target)
    {
        float percentage = 0f;
        while (nowPlaying.volume > 0)
        {
            nowPlaying.volume = Mathf.Lerp(audioSystem.GetVolume(AudioCategory.music), 0, percentage);
            percentage += Time.deltaTime;
            yield return null;
        }
        nowPlaying.clip = target;
        nowPlaying.time = 0;
        if (!nowPlaying.isPlaying)
        {
            nowPlaying.Play();
        }
        percentage = 0f;
        while (nowPlaying.volume < audioSystem.GetVolume(AudioCategory.music))
        {
            nowPlaying.volume = Mathf.Lerp(0, audioSystem.GetVolume(AudioCategory.music), percentage);
            percentage += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LoadSceneCoroutine(string name)
	{
		_loadSceneCoroutineRunning = true;
		GameObject sceneLoadAnimationObject = Instantiate(_sceneLoadAnimationPrefab);
		sceneLoadAnimationObject.transform.position = Camera.main.transform.position + new Vector3(0f, -10f, 1f);
		Animation sceneLoadAnimation = sceneLoadAnimationObject.GetComponent<Animation>();
		while (sceneLoadAnimation.isPlaying)
		{
			yield return null;
		}
		SceneManager.LoadScene(name);
		_loadSceneCoroutineRunning = false;
	}

	private void RemoveSceneLoadEndAnimation()
	{
		if (_sceneLoadEndAnimationObject != null)
			Destroy(_sceneLoadEndAnimationObject);
	}

    public void SetMenuMusic()
    {
        AudioSource audioSource = _musicObject.GetComponent<AudioSource>();
        StartCoroutine(MixAudio(audioSource, menumusic));
    }

    private void LoadPoolablePrefabs()
	{
		Object[] objs = Resources.LoadAll("PoolablePrefabs");
		foreach (Object obj in objs)
		{
			if (obj is not GameObject)
			{
				Debug.LogError($"Object {obj.name} is not a GameObject");
				continue;
			}
#if UNITY_EDITOR
			if (UnityEditor.PrefabUtility.GetPrefabAssetType(obj) == UnityEditor.PrefabAssetType.NotAPrefab)
			{
				Debug.LogError($"GameObject {obj.name} is not a prefab");
				continue;
			}
#endif
			GameObject go = (GameObject)obj;
			Component[] components = go.GetComponents(typeof(Component)).Where(component => component is IPoolable).ToArray();
			if (components.Length == 0)
			{
				Debug.LogError($"Prefab {go.name} does not implement IPoolable");
				continue;
			}
			if (components.Length > 1)
			{
				Debug.LogError($"Prefab {go.name} implements IPoolable multiple times");
				continue;
			}
			pool.AddPool(components[0].GetType(), new GameObjectFactory(components[0].gameObject));
		}
	}
}
