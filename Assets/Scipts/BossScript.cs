using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.SceneManagement;

public class BossScript : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float shootingCooldown;
    public float searchingRadius;
    public float damage = 1;

    [Header("Settings")]
    public bool SouldSearch;
    public bool SouldShoot;
    public AnimationCurve ProjectlilesPattern;
    public bool[] SidesToShoot;
    public AnimationCurve[] BossAttacks;
    public AnimationCurve[] BossAttacksBulletPatterns;
    public float[] BossAttacksCds;
    public float[] BossAttacksTiming;
    public AnimationCurve BulletPath;
    public float bulletSpeed;
    public ParticleSystem myDeathParticles;

    [Header("Fields")]
    public float[] guns;
    public GameObject SlimeDeathParticles;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public GameObject bullet;
    public Sprite enemyBullet;
    private RaycastHit2D ray;
    private ParticleSystem copy;
    public GameObject gigaPickup;
    public GameObject realBoss;

    [Header("Debug")]
    public bool AmIShooting;
    public bool ShootAPlayer;
    public bool AmIKind;
    public bool Died;

    public void InvokeDestr1()
    {
        Invoke(nameof(InvokeDestr2), 2f);
    }
    public void InvokeDestr2()
    {
        Destroy(gameObject);
    }
    public void AttackSpawnCycle()
    {
        System.Random rnd = new System.Random();
        int vl = rnd.Next(0, BossAttacks.Length);
        BulletPath = BossAttacksBulletPatterns[vl];
        ProjectlilesPattern = BossAttacks[vl];
        shootingCooldown = BossAttacksCds[vl];
        Invoke(nameof(AttackSpawnCycle), BossAttacksTiming[vl]);
    }
    private void OnEnable()
    {
        Camera.main.GetComponent<CameraShake>().StartCrtnRemotelyShake(1f, 0.7f);
        AttackSpawnCycle();
        System.Random rnd = new System.Random();
        if (rnd.Next(0, 2) == 0)
        {
            ShootAPlayer = true;
        }
        else
        {
            ShootAPlayer = false;
        }
    }

    private void Update()
    {
        if (realBoss.GetComponent<SpriteRenderer>().maskInteraction != SpriteMaskInteraction.None)
        {
            realBoss.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            realBoss.GetComponent<Animator>().SetBool("Boss", true);
        }


        if (SouldShoot)
        {
            if (!AmIShooting)
            {
                AmIShooting = true;
                InvokeRepeating(nameof(InvokeShoot), shootingCooldown, shootingCooldown);
            }
        }

    }
    public void InvokeShoot()
    {
        for (int i = 0; i < 4; i++)
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
                blt.GetComponent<BulletScript>().FromATeammate = false;

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

    public void Die()
    {
        if (!Died)
        {
            Camera.main.GetComponent<CameraShake>().StartCrtnRemotelyShake(0.3f, 0.5f);
            Died = true;
            SpawnEnemies spwn = GameObject.FindGameObjectWithTag("Spawner").GetComponent<SpawnEnemies>();
            if (spwn.StageIndex == spwn.AmountOfStages - 1)
            {
                GameObject.FindGameObjectWithTag("DlgMng").GetComponent<DialogueScript>().StartMainLine();
            }
            else
            {
                spwn.BossKilled();
            }
            Instantiate(gigaPickup, transform.position, Quaternion.identity);
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(SlimeDeathParticles, prt);
            prt.transform.position = transform.position;
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