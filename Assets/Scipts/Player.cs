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
    private float healthBeforeChanged;
    private Rigidbody2D rb;
    private float currentVelocity;
    public float rotationSmoothTime = 0.2f;
    private Animator anim;
    public Slider healthBar;
    public float damage = 1f;
    public Gradient healthGradient;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void TakeDamage(float dmg)
    {
        healthBeforeChanged = health;
        health -= dmg;
        health = Mathf.Clamp(health, 0, 100);
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(CrtnTakeDamage());
    }
    public IEnumerator CrtnTakeDamage()
    {
        healthBar.value = healthBeforeChanged;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        yield return new WaitForSeconds(0.05f);
        healthBar.value = healthBeforeChanged - (healthBeforeChanged-health)/3;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        yield return new WaitForSeconds(0.05f);
        healthBar.value = healthBeforeChanged - ((healthBeforeChanged - health) / 3) * 2;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        yield return new WaitForSeconds(0.05f);
        healthBar.value = health;
        fill.color = healthGradient.Evaluate(healthBar.normalizedValue);

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
