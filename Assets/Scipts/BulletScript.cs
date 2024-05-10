using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage;
    public GameObject BulletDeathParticles;
    private void Start()
    {
        Invoke(nameof(DieInTime), 3f);
    }
    public void DieInTime()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
    }
}