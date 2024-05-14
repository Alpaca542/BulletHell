using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Params")]
    public float speed;
    public float health;
    private Rigidbody2D rb;
    private float currentVelocity;
    private float rotationSmoothTime = 0.1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        // Handle movement
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(dirX, dirY).normalized * speed;
        rb.velocity = movement;

        // Handle rotation
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(-movement.x, movement.y) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref currentVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
