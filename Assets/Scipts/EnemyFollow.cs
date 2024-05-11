using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float searchingRadius;

    [Header("Settings")]
    public bool SouldSearch;
    public bool SouldShoot;
    public AnimationCurve ProjectlilesPattern;
    public int AmountOfProjectilesForEachSide;

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
        transform.right = target.transform.position - transform.position;
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
        //90 0 -90
        foreach(Keyframe kf in ProjectlilesPattern.keys)
        {
            GameObject blt = Instantiate(bullet, new Vector2(transform.position.x + kf.value, transform.position.y + kf.time), Quaternion.Euler(new Vector3(0, 0, kf.inTangent+90)));
            blt.GetComponent<Rigidbody2D>().AddForce(blt.transform.up * 1000f);
        }
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
