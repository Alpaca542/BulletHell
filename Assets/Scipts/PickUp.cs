using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    public int which;
    public float HealthBoost;
    public GameObject pickUpParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.GetComponent<Player>() != null)
        {
            if (!collision.GetComponent<Player>().boosted)
            {
                collision.GetComponent<Player>().Boost(which);
                collision.GetComponent<Player>().TakeDamage(-HealthBoost);

                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Heal();


                GameObject prt = ((DieInTime)GameManager.Instance.pool.Get<DieInTime>()).gameObject;
                CopyComponent(pickUpParticles, prt);
                prt.transform.position = transform.position;
                Destroy(gameObject);
            }
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