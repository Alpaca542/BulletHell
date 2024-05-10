using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage;
    private void Start()
    {
        Invoke(nameof(DieInTime), 3f);
    }
    public void DieInTime()
    {
        Destroy(gameObject);
    }
}
