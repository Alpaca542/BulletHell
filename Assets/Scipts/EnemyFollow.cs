using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float shootingCooldown;
    public float searchingRadius;
    public float damage = 1f;

    [Header("Settings")]
    public bool SouldSearch;
    public bool SouldShoot;
    public bool SouldGoUp;
    public bool ShouldGoSide;
    public bool ShouldGoDiag;
    public AnimationCurve ProjectlilesPattern;
    public bool[] SidesToShoot;
    public AnimationCurve BulletPath;
    public float bulletSpeed;
    public bool InvertPatterns;
    public ParticleSystem myDeathParticles;

    [Header("Fields")]
    public float[] guns;
    private NavMeshAgent agent;
    public GameObject SlimeDeathParticles;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public GameObject bullet;
    public GameObject target;
    public Sprite enemyBullet;
    private RaycastHit2D ray;
    private ParticleSystem copy;
    public GameObject[] PickUps;
    public AudioClip DieSound;

    [Header("Debug")]
    public bool AmIShooting;
    public bool ShootAPlayer;
    public bool AmIKind;
    public float distance = 0;
    public float distance2 = 0;
    public bool Died;

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
        distance = rnd.Next(-40, 40) / 10;
        distance2 = rnd.Next(20, 70) / 10;
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
        if (Died)
        {
            CancelInvoke(nameof(InvokeShoot));
        }
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
                if (SouldGoUp)
                {
                    agent.SetDestination(new Vector2(searcher.transform.position.x, searcher.transform.position.y+ distance2));
                }
                else if (ShouldGoSide)
                {
                    agent.SetDestination(new Vector2(target.transform.position.x + distance2, target.transform.position.y));
                }
                else if (ShouldGoDiag)
                {
                    agent.SetDestination(new Vector2(searcher.transform.position.x + distance, searcher.transform.position.y + distance2));
                }
                else
                {
                    agent.SetDestination(new Vector2(searcher.transform.position.x + distance, searcher.transform.position.y + distance));
                }
            }
        }
        else
        {
            if (target != null)
            {
                if (SouldGoUp)
                {
                    agent.SetDestination(new Vector2(target.transform.position.x, target.transform.position.y + distance2));
                }
                else if (ShouldGoSide)
                {
                    agent.SetDestination(new Vector2(target.transform.position.x + distance2, target.transform.position.y));
                }
                else if (ShouldGoDiag)
                {
                    agent.SetDestination(new Vector2(target.transform.position.x + distance, target.transform.position.y + distance2));
                }
                else
                {
                    agent.SetDestination(new Vector2(target.transform.position.x + distance, target.transform.position.y + distance));
                }
            }
        }


        if (SouldShoot && !Died)
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
        for (int i = 0; i < 4; i++)
        {
            if (SidesToShoot[i])
            {
                foreach (Keyframe kf in ProjectlilesPattern.keys)
                {
                    GameObject blt = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
                    blt.transform.rotation = Quaternion.Euler(new Vector3(0, 0, guns[i] + Mathf.Rad2Deg * Mathf.Atan(kf.inTangent)));
                    blt.GetComponent<BulletScript>().Path = BulletPath;
                    blt.GetComponent<BulletScript>().damage = damage;
                    blt.transform.localScale = new Vector2(0.04f, 0.04f);
                    blt.GetComponent<SpriteRenderer>().sprite = enemyBullet;

                    blt.GetComponent<BulletScript>().AmIFromPlayer = false;
                    if (AmIKind)
                    {
                        blt.GetComponent<BulletScript>().FromATeammate = true;
                    }
                    else
                    {
                        blt.GetComponent<BulletScript>().FromATeammate = false;
                    }

                    if (guns[i] == 270)
                    {
                        blt.transform.position = new Vector2(transform.position.x + kf.value, transform.position.y - kf.time);
                    }
                    else if (guns[i] == 90)
                    {
                        blt.transform.position = new Vector2(transform.position.x - kf.value, transform.position.y + kf.time);
                    }
                    else if (guns[i] == 180)
                    {
                        blt.transform.position = new Vector2(transform.position.x - kf.time, transform.position.y - kf.value);
                    }
                    else if (guns[i] == 0)
                    {
                        blt.transform.position = new Vector2(transform.position.x + kf.time, transform.position.y + kf.value);
                    }
                    blt.GetComponent<BulletScript>().startPosition = blt.transform.position;
                    blt.GetComponent<BulletScript>().speed = bulletSpeed;
                    blt.GetComponent<BulletScript>().StartRotation = new Vector3(0, 0, guns[i] + Mathf.Rad2Deg * Mathf.Atan(kf.outTangent) + 90f);
                }
            }
        }
    }
    public void Die()
    {
        if (!Died)
        {
            GameManager.Instance.audioSystem.PlayClip(DieSound, new AudioClipSettings { category = AudioCategory.sfx, forcePlay = true, looping = false });
            Camera.main.GetComponent<CameraShake>().StartCrtnRemotelyShake(0.12f, 0.3f);
            Died = true;
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(SlimeDeathParticles, prt);
            prt.transform.position = transform.position;
            System.Random rnd1 = new System.Random();
            if (rnd1.Next(0, 5) == 0)
            {
                System.Random rnd2 = new System.Random();
                Instantiate(PickUps[rnd2.Next(0, 4)], transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    void CopyComponent(GameObject original, GameObject toWhat)
    {
        ParticleSystem originalPS = original.GetComponent<ParticleSystem>();
        ParticleSystem copyPS = toWhat.GetComponent<ParticleSystem>();

        ParticleSystem.ColorOverLifetimeModule originalColorOverLifetime = originalPS.colorOverLifetime;
        ParticleSystem.ColorOverLifetimeModule copyColorOverLifetime = copyPS.colorOverLifetime;

        ParticleSystem.MainModule originalMainModule = originalPS.main;
        ParticleSystem.MainModule copyMainModule = copyPS.main;

        copyColorOverLifetime.enabled = originalColorOverLifetime.enabled;
        copyColorOverLifetime.color = originalColorOverLifetime.color;


        copyMainModule.startSize = originalMainModule.startSize;
        copyMainModule.startSpeed = originalMainModule.startSpeed;
        copyMainModule.maxParticles = originalMainModule.maxParticles;
    }
}