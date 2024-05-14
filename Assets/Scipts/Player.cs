using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Params")]
    public float speed;
    public float health;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(dirX, dirY) * speed;
        if((Mathf.Abs(dirX) < 0.2f || Mathf.Abs(dirY) < 0.2f) && (Mathf.Abs(dirX) > 0.2f || Mathf.Abs(dirY) > 0.2f))
        {
            if (dirX > 0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            else if (dirX < -0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else if (dirY > 0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (dirY < -0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
        }
        else
        {
            if (dirX > 0.2f && dirY < -0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90-45));
            }
            else if (dirX > 0.2f && dirY > 0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90 + 45));
            }
            else if (dirX < -0.2f && dirY < -0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 + 45));
            }
            else if (dirX < -0.2f && dirY > 0.2f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 - 45));
            }
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
