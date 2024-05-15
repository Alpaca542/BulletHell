using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieInTime : MonoBehaviour, IPoolable
{
    public float TimeInSeconds;
    public void OnGetFromPool()
    {
        gameObject.SetActive(true);
        CancelInvoke(nameof(Die));
        Invoke(nameof(Die), TimeInSeconds);
    }

    public void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }
    public void Die()
    {
        GameManager.Instance.pool.Return(this);
    }
}
