using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour, IPoolable
{
    public float damage;
    public float speed = 3f;
    public AnimationCurve Path;
    public GameObject BulletDeathParticles;
    private float t = 0.0f;
    public Vector3 startPosition;
    public Vector3 StartRotation;
    public bool InvertPattern;

    public bool AmIFromPlayer;
    public bool FromATeammate;

    private void Start()
    {
        startPosition = transform.position;
    }
    private void OnEnable()
    {
        t = 0;
        startPosition = transform.position;
    }

    public void DieInTime()
    {
        GameManager.Instance.pool.Return(this);
    }

	public void OnGetFromPool()
	{
        gameObject.SetActive(true);
		startPosition = transform.position;
        CancelInvoke(nameof(DieInTime));
		Invoke(nameof(DieInTime), 4f);
	}

	public void OnReturnToPool()
	{
		gameObject.SetActive(false);
	}

	private void Update()
    {
        if(!AmIFromPlayer || FromATeammate)
        {
            t += Time.deltaTime;

            if (t > Path.keys[Path.length - 1].time)
            {
                t = 0.0f;
                startPosition = transform.position;
            }


            float yPos = Path.Evaluate(t);
            if (InvertPattern)
            {
                Vector2 newPosition = new Vector2(t * speed, yPos * speed);
                Vector2 rotatedVector = startPosition + (Quaternion.AngleAxis(StartRotation.z+90f, Vector3.forward) * newPosition);
                transform.position = rotatedVector;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && AmIFromPlayer)
        {
            EnemyAI enem = collision.gameObject.GetComponent<EnemyAI>();
            enem.health -= damage;
            if (enem.health <= 0)
            {
                enem.Die();
            }
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(BulletDeathParticles, prt);
            prt.transform.position = transform.position;
            GameManager.Instance.pool.Return(this);
        }
        else if(collision.gameObject.tag == "Player" && !AmIFromPlayer)
        {
            if(collision.gameObject.GetComponent<Player>() != null)
            {
                Player plr = collision.gameObject.GetComponent<Player>();
                plr.TakeDamage(damage);
                GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
                CopyComponent(BulletDeathParticles, prt);
                prt.transform.position = transform.position;
                GameManager.Instance.pool.Return(this);
            }
            else
            {
                EnemyAI enem = collision.gameObject.GetComponent<EnemyAI>();
                enem.health -= damage;
                if (enem.health <= 0)
                {
                    enem.Die();
                }
                GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
                CopyComponent(BulletDeathParticles, prt);
                prt.transform.position = transform.position;
                GameManager.Instance.pool.Return(this);
            }
        }
        else if (collision.gameObject.tag == "Boss" && AmIFromPlayer)
        {
            BossScript enem = collision.gameObject.GetComponent<BossScript>();
            enem.health -= damage;
            if (enem.health <= 0)
            {
                enem.Die();
            }
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(BulletDeathParticles, prt);
            prt.transform.position = transform.position;
            GameManager.Instance.pool.Return(this);
        }
        else if (collision.gameObject.tag == "Obstacle")
        {
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(BulletDeathParticles, prt);
            prt.transform.position = transform.position;
            GameManager.Instance.pool.Return(this);
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