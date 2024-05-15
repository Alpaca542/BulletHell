using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float shootingCooldown;
    public float searchingRadius;

    [Header("Settings")]
    public bool SouldSearch;
    public bool SouldShoot;
    public AnimationCurve ProjectlilesPattern;
    public bool[] SidesToShoot;
    public AnimationCurve BulletPath;
    public float bulletSpeed;
    public bool InvertPatterns;

    [Header("Fields")]
    public GameObject[] guns;
    private NavMeshAgent agent;
    public GameObject SlimeDeathParticles;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public GameObject bullet;
    public GameObject target;
    public Sprite enemyBullet;
    private RaycastHit2D ray;

    [Header("Debug")]
    public bool AmIShooting;
    public bool ShootAPlayer;
    public bool AmIKind;

    public void InvokeDestr1()
    {
        Invoke(nameof(InvokeDestr2), 2f);
    }
    public void InvokeDestr2()
    {
        Destroy(gameObject);
    }
    private void OnEnable()
    {
        System.Random rnd = new System.Random();
        if (rnd.Next(0, 2) == 0)
        {
            ShootAPlayer = true;
        }
        else
        {
            ShootAPlayer = false;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        if (!AmIKind)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Enemy");
        }
    }

    //public void OnGetFromPool()
    //{
    //    gameObject.SetActive(true);
    //}

    //public void OnReturnToPool()
    //{
    //    gameObject.SetActive(false);
    //}
    private void Update()
    {
        if (!AmIKind)
        {
            target = GameObject.FindGameObjectWithTag("Player");

            var kindEnemies = GameObject.FindObjectsOfType<EnemyAI>()
                .Select(obj => obj.GetComponent<EnemyAI>())
                .Where(ai => ai != null && ai.AmIKind)
                .ToList();
            if (!ShootAPlayer && kindEnemies.Count != 0)
            {
                target = kindEnemies[0].gameObject;
            }
            else
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Boss"))
            {
                target = GameObject.FindGameObjectWithTag("Boss");
            }
            else
            {
                target = GameObject.FindGameObjectWithTag("Enemy");
            }
        }
        if (GetComponent<SpriteRenderer>().maskInteraction != SpriteMaskInteraction.None)
        {
            GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            GetComponent<Animator>().SetBool("Alive", true);
        }

        if (SouldSearch)
        {
            Collider2D searcher;
            if (!AmIKind)
            {
                searcher = Physics2D.OverlapCircle(transform.position, searchingRadius, playerLayer);
            }
            else
            {
                searcher = Physics2D.OverlapCircle(transform.position, searchingRadius, enemyLayer);
            }
            if (searcher)
            {
                agent.SetDestination(searcher.transform.position);
            }
        }
        else
        {
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
        }


        if (SouldShoot)
        {
            if (!AmIKind)
            {
                if (target != null)
                {
                    ray = Physics2D.Raycast(transform.position, (target.transform.position - transform.position).normalized, shootingDistance, playerLayer);
                }
            }
            else
            {
                if (target != null)
                {
                    ray = Physics2D.Raycast(transform.position, (target.transform.position - transform.position).normalized, shootingDistance, enemyLayer);
                }
            }
            if (ray.collider != null)
            {
                if (!AmIShooting)
                {
                    AmIShooting = true;
                    InvokeRepeating(nameof(InvokeShoot), shootingCooldown, shootingCooldown);
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
        for(int i = 0; i<4; i++)
        {
            if (SidesToShoot[i])
            {
                foreach (Keyframe kf in ProjectlilesPattern.keys)
                {
                    GameObject blt = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
                    blt.transform.rotation = Quaternion.Euler(new Vector3(0, 0, guns[i].transform.rotation.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(kf.inTangent)));
                    blt.GetComponent<BulletScript>().Path = BulletPath;
                    blt.GetComponent<SpriteRenderer>().sprite = enemyBullet;
                    if (!AmIKind)
                    {
                        blt.GetComponent<BulletScript>().AmIFromPlayer = false;
                        blt.GetComponent<BulletScript>().FromATeammate = false;
                    }
                    else
                    {
                        blt.GetComponent<BulletScript>().FromATeammate = true;
                        blt.GetComponent<BulletScript>().AmIFromPlayer = true;
                    }
                    blt.GetComponent<BulletScript>().InvertPattern = InvertPatterns;
                    if(guns[i].transform.eulerAngles.z == 90 || guns[i].transform.eulerAngles.z == -90 || guns[i].transform.eulerAngles.z == 270 || guns[i].transform.eulerAngles.z == -270)
                    {
                        blt.transform.position = new Vector2(guns[i].transform.position.x + kf.value, guns[i].transform.position.y + kf.time);
                        blt.GetComponent<BulletScript>().startPosition = new Vector2(guns[i].transform.position.x + kf.value, guns[i].transform.position.y + kf.time);
                    }
                    else
                    {
                        blt.transform.position = new Vector2(guns[i].transform.position.x + kf.time, guns[i].transform.position.y + kf.value);
                        blt.GetComponent<BulletScript>().startPosition = new Vector2(guns[i].transform.position.x + kf.time, guns[i].transform.position.y + kf.value);
                    }
                    blt.GetComponent<BulletScript>().speed = bulletSpeed;
                    blt.GetComponent<BulletScript>().StartRotation = new Vector3(0, 0, guns[i].transform.rotation.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(kf.inTangent));
                    blt.transform.Rotate(new Vector3(0, 0, 90));
                }
            }
        }
    }
    public void Die()
    {
        Instantiate(SlimeDeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}