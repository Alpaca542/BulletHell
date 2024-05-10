using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Params")]
    public float speed;
    public float health;
    private NavMeshAgent agent;
    public GameObject target;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        agent.SetDestination(target.transform.position);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "buller")
        {
            health -= collision.gameObject.GetComponent<BulletScript>().damage;
        }
    }
}
