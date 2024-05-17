using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [Header("Params")]
    public Image fill;
    public float speed;
    public float health;
    public bool usedBoost1;
    public bool usedBoost2;
    public float basedamage;
    private float AllTheDamage;
    private Rigidbody2D rb;
    public float baseSpeed;
    private float currentVelocity;
    public float rotationSmoothTime = 0.2f;
    private Animator anim;
    public float healthBeforeChanged;
    public Slider healthBar;
    public float damage = 1f;
    public Gradient healthGradient;
    public bool healing;
    public bool boosted;
    public Volume volume;
    public Color32[] vignettereffects;
    public Text[] BoostTextes;
    public GameObject[] BoostImgs;
    public void Boost(int which)
    {
        Vignette vgn;
        if (volume.profile.TryGet<Vignette>(out vgn))
        {
            vgn.active = true;
            vgn.color.value = vignettereffects[which];
        }

        if (which == 0)
        {
            speed += 8;
            StartCoroutine(CrtnEndBoost(which));
            BoostImgs[which].SetActive(true);
        }
        else if (which == 1)
        {
            damage += 2;
            StartCoroutine(CrtnEndBoost(which));
            BoostImgs[which].SetActive(true);
        }
        else if (which == 2)
        {
            baseSpeed += 2;
            basedamage ++;
            damage += 5;
            speed += 5;
            StartCoroutine(CrtnEndBoost(which));
            BoostImgs[which].SetActive(true);
        }
        else if (which == 3)
        {
            health += 20;
            healthBar.value = health;
            healthBar.GetComponent<Animation>().Play();
            BoostImgs[which].SetActive(true);
            fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
            StartCoroutine(CrtnEndBoost(which));
        }
    }
    public void InvokeEndHeal()
    {
        Vignette vgn;
        if (volume.profile.TryGet<Vignette>(out vgn))
        {
            vgn.active = false;
        }
    }

    public IEnumerator CrtnEndBoost(int which)
    {
        boosted = true;
        float timer = 5;
        while(timer > 0.1)
        {
            timer -= 0.1f;
            BoostTextes[which].text = timer.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        damage = basedamage;
        speed = baseSpeed;
        Vignette vgn;
        if (volume.profile.TryGet<Vignette>(out vgn))
        {
            vgn.active = false;
        }
        BoostImgs[which].SetActive(false);
        BoostTextes[which].text = "0";
        boosted = false;
    }

    private void Awake()
    {
        healthBeforeChanged = health;
        healthBar.value = health;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);

        baseSpeed = speed;
        basedamage = damage;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(CrtnTakeDamage());
    }

    public void TakeDamage(float dmg)
    {
        AllTheDamage += dmg;
        health -= dmg;
        health = Mathf.Clamp(health, 0, 100);
        if (health <= 0)
        {
            Die();
        }
        if (!healing)
        {
            StartCoroutine(CrtnTakeDamage());
        }
    }

    public IEnumerator CrtnTakeDamage()
    {
        healing = true;
        while (AllTheDamage >= 0.4f)
        {
            healthBar.value -= 0.2f;
            AllTheDamage -= 0.2f;
            fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
            yield return new WaitForSeconds(0.01f);
        }
        healing = false;
    }
    void FixedUpdate()
    {
        // Handle movement
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(dirX, dirY).normalized * speed;
        rb.velocity = movement;

        // Handle rotation
        if (Mathf.Abs(movement.x) > 0.2f || Mathf.Abs(movement.y) > 0.2f)
        {
            anim.SetBool("IsMoving", true);
            float targetAngle = Mathf.Atan2(-movement.x, movement.y) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref currentVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }
    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}
