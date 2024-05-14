using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
		Invoke(nameof(DieInTime), 10f);
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
            Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
            GameManager.Instance.pool.Return(this);
        }
        else if(collision.gameObject.tag == "Player" && !AmIFromPlayer)
        {
            if(collision.gameObject.GetComponent<Player>() != null)
            {
                Player plr = collision.gameObject.GetComponent<Player>();
                plr.health -= damage;
                if (plr.health <= 0)
                {
                    plr.Die();
                }
            }
            else
            {
                EnemyAI enem = collision.gameObject.GetComponent<EnemyAI>();
                enem.health -= damage;
                if (enem.health <= 0)
                {
                    enem.Die();
                }
            }
            Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
            GameManager.Instance.pool.Return(this);
        }
    }
}