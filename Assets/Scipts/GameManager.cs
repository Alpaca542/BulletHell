
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private AudioClip _music;
	public static GameManager Instance;

    public readonly ObjectPool pool = new ObjectPool();
	public readonly AudioSystem audioSystem = new AudioSystem();

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
	}

	void Start()
    {
		LoadPoolablePrefabs();
		audioSystem.PlayClip(_music, new AudioClipSettings { looping = true, forcePlay = true, category = AudioCategory.music });
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
