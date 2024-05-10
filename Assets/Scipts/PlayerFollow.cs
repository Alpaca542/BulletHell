using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerfollow : MonoBehaviour
{
    public GameObject player;
    private Vector3 playervector;
    public float speed;
    private void Update()
    {
        playervector = player.transform.position;
        playervector.z = playervector.z - 10;
        transform.position = Vector3.Lerp(transform.position, playervector, speed * Time.deltaTime);
    }
}