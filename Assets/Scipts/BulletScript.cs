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
    private Vector3 startPosition;
    public Vector3 StartRotation;

    //private void Start()
    //{
    //    startPosition = transform.position;
    //    Invoke(nameof(DieInTime), 10f);
    //}

    public void DieInTime()
    {
        GameManager.Instance.pool.Return(this);
    }

	public void OnGetFromPool()
	{
        gameObject.SetActive(true);
		startPosition = transform.position;
		Invoke(nameof(DieInTime), 10f);
	}

	public void OnReturnToPool()
	{
		gameObject.SetActive(false);
	}

	private void Update()
    {
        if(gameObject.tag == "EnemyBullet")
        {
            t += Time.deltaTime;

            if (t > Path.keys[Path.length - 1].time)
                t = 0.0f;

            float yPos = Path.Evaluate(t);
            if(StartRotation.z == -90 || StartRotation.z == 270)
            {
                Vector2 newPosition = new Vector2(startPosition.x + t * speed, startPosition.y + yPos * speed);
                transform.position = newPosition;
            }
            else if (StartRotation.z == 90 || StartRotation.z == -270)
            {
                Vector2 newPosition = new Vector2(startPosition.x - t * speed, startPosition.y - yPos * speed);
                transform.position = newPosition;
            }
            else if (StartRotation.z == 180 || StartRotation.z == -180)
            {
                Vector2 newPosition = new Vector2(startPosition.x - yPos * speed, startPosition.y - t * speed);
                transform.position = newPosition;
            }
            else if (StartRotation.z == 0)
            {
                Vector2 newPosition = new Vector2(startPosition.x + yPos * speed, startPosition.y + t * speed);
                transform.position = newPosition;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "PlayerBullet")
        {
            EnemyAI enem = collision.gameObject.GetComponent<EnemyAI>();
            enem.health -= damage;
            if (enem.health <= 0)
            {
                enem.Die();
            }
            Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Player" && gameObject.tag == "EnemyBullet")
        {
            Player plr = collision.gameObject.GetComponent<Player>();
            plr.health -= damage;
            if (plr.health <= 0)
            {
                plr.Die();
            }
            Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        //else if ((collision.gameObject.tag == "PlayerBullet" && gameObject.tag == "EnemyBullet") || (collision.gameObject.tag == "EnemyBullet" && gameObject.tag == "PlayerBullet"))
        //{
        //    Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
        //    Destroy(collision.gameObject);
        //    Destroy(gameObject);
        //}
    }
}