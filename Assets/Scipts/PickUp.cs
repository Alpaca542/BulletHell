using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float HealthBoost;
    public float DamageBoost;
    public float SpeedBoost;
    public GameObject pickUpParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.GetComponent<Player>() != null)
        {
            collision.GetComponent<Player>().speed += SpeedBoost;
            collision.GetComponent<Player>().damage += DamageBoost;
            collision.GetComponent<Player>().health += HealthBoost;
            GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
            CopyComponent(pickUpParticles, prt);
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