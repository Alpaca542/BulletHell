using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPoolable
{
	public void OnGetFromPool();
	public void OnReturnToPool();
}

public interface IPoolFactory
{
	IPoolable Create();
}

public class GameObjectFactory : IPoolFactory
{
	private readonly GameObject _prefab;

	public GameObjectFactory(GameObject prefab)
	{
		_prefab = prefab;
	}

	public IPoolable Create()
	{
		GameObject go = Object.Instantiate(_prefab);
		Component component = go.GetComponents(typeof(Component)).Where(component => component is IPoolable).ToArray()[0];
		return (IPoolable)component;
	}
}

public class ObjectPool
{
	private class Pool
	{
		public IPoolFactory factory;
		public ConcurrentBag<IPoolable> items;
	}

	private Dictionary<System.Type, Pool> _pools = new Dictionary<System.Type, Pool>();

	public void AddPool(System.Type type, IPoolFactory factory)
	{
		_pools.Add(type, new Pool() { factory = factory, items = new ConcurrentBag<IPoolable>() });
	}

	public IPoolable Get<T>() where T : IPoolable
	{
		System.Type type = typeof(T);
		IPoolable item;
		if (!_pools[type].items.TryTake(out item))
			item = _pools[type].factory.Create();
		item.OnGetFromPool();
		return item;
	}

	public void Return<T>(T item) where T : IPoolable
	{
		item.OnReturnToPool();
		_pools[typeof(T)].items.Add(item);
	}

	public void Clear()
	{
		foreach (var pool in _pools)
		{
			pool.Value.items.Clear();
		}
	}
}
