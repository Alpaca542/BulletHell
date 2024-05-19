
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private AudioClip _music;
	[SerializeField] private AudioClip _bossMusic;

	public static GameManager Instance;
    public readonly ObjectPool pool = new ObjectPool();
	public readonly AudioSystem audioSystem = new AudioSystem();
	private GameObject _menu;
	private GameObject _musicObject;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		_menu = FindObjectOfType<MenuManager>(true).gameObject;
		audioSystem.SetVolume(AudioCategory.master, 0.5f);
		audioSystem.SetVolume(AudioCategory.sfx, 0.5f);
		audioSystem.SetVolume(AudioCategory.music, 0.5f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menu.SetActive(!_menu.activeInHierarchy);
		}
	}

	void Start()
    {
		LoadPoolablePrefabs();
		if (_musicObject == null)
		{
			_musicObject = audioSystem.PlayClip(_music, new AudioClipSettings { looping = true, forcePlay = true, category = AudioCategory.music }).gameObject;
			_musicObject.name = "MainMusic";
			DontDestroyOnLoad(_musicObject.gameObject);
		}
	}

	public void SetMainMusic()
	{
		AudioSource audioSource = _musicObject.GetComponent<AudioSource>();
		audioSource.clip = _music;
		audioSource.time = 0;
		audioSource.Play();
	}

	public void SetBossMusic()
	{
		AudioSource audioSource = _musicObject.GetComponent<AudioSource>();
		audioSource.clip = _bossMusic;
		audioSource.time = 0;
		audioSource.Play();
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
