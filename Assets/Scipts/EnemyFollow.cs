using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyAI : MonoBehaviour
{
    [Header("Params")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float searchingRadius;
    public bool SouldSearch;
    public bool SouldShoot;

    [Header("Fields")]
    private NavMeshAgent agent;
    public GameObject SlimeDeathParticles;
    public LayerMask playerLayer;
    public GameObject bullet;
    public GameObject target;

    [Header("Debug")]
    public bool AmIShooting;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        target = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (SouldSearch)//Looks for a player in a radius
        {
            Collider2D searcher = Physics2D.OverlapCircle(transform.position, searchingRadius, playerLayer);
            if (searcher)
            {
                agent.SetDestination(searcher.transform.position);
            }
        }
        else
        {
            agent.SetDestination(target.transform.position);
        }


        if (SouldShoot)//Sends a ray, if it hits smth start a shooting invoke
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, (target.transform.position-transform.position).normalized, shootingDistance, playerLayer);
            if (ray)
            {
                if (!AmIShooting)
                {
                    AmIShooting = true;
                    InvokeRepeating(nameof(InvokeShoot), 0, 1f);
                }
            }
            else
            {
                AmIShooting = false;
                CancelInvoke(nameof(InvokeShoot));
            }
        }

    }
    public void InvokeShoot()
    {
        GameObject blt = Instantiate(bullet, transform.position, Quaternion.identity);
        blt.GetComponent<Rigidbody2D>().AddForce((target.transform.position-transform.position).normalized * 1000f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerBullet")
        {
            health -= collision.gameObject.GetComponent<BulletScript>().damage;
            Destroy(collision.gameObject);
        }
        if (health <= 0)
        {
            Instantiate(SlimeDeathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
