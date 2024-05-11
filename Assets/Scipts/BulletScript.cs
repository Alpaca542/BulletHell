using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage;
    public GameObject BulletDeathParticles;
    private void Start()
    {
        Invoke(nameof(DieInTime), 3f);
    }
    public void DieInTime()
    {
        Destroy(gameObject);
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
        else if ((collision.gameObject.tag == "PlayerBullet" && gameObject.tag == "EnemyBullet") || (collision.gameObject.tag == "EnemyBullet" && gameObject.tag == "PlayerBullet"))
        {
            Instantiate(BulletDeathParticles, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}