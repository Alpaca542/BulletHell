using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieInTime : MonoBehaviour
{
    public float TimeInSeconds;
    private void Start()
    {
        Invoke(nameof(Die), TimeInSeconds);
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
