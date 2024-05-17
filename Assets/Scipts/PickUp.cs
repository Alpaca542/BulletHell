using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    public int which;
    public string PickUpLesson1;
    public Sprite PickUpLessonSprite1;
    public string PickUpLesson2;
    public Sprite PickUpLessonSprite2;
    public float HealthBoost;
    public GameObject pickUpParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.GetComponent<Player>() != null)
        {
            if (!collision.GetComponent<Player>().usedBoost1 && which != 2)
            {
                collision.GetComponent<Player>().usedBoost1 = true;
                GameObject.FindGameObjectWithTag("DlgMng").GetComponent<DialogueScript>().StartCrtnRemotely(PickUpLesson1, PickUpLessonSprite1, true);
                Time.timeScale = 0.05f;
            }
            else if(!collision.GetComponent<Player>().usedBoost2 && which == 2)
            {
                collision.GetComponent<Player>().usedBoost2 = true;
                GameObject.FindGameObjectWithTag("DlgMng").GetComponent<DialogueScript>().StartCrtnRemotely(PickUpLesson2, PickUpLessonSprite2, true);
                Time.timeScale = 0.05f;
            }
            if (!collision.GetComponent<Player>().boosted)
            {
                collision.GetComponent<Player>().Boost(which);

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