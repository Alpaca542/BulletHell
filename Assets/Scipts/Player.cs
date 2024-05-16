using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Params")]
    public Image fill;
    public float speed;
    public float health;
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
    private void Awake()
    {
        healthBeforeChanged = health;
        healthBar.value = health;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);

        baseSpeed = speed;
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
    public void Heal()
    {
        healthBar.GetComponent<Animation>().Play();
        healthBar.value = health;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
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
