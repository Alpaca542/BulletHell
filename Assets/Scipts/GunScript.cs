using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GameObject plr;
    public int[] AmountOfSlimes;
    public float angleOfChange;
    public bool Connected = false;
    public LayerMask ConnectLineTo, ConnectLineTo2;
    public GameObject ConnectObject;
    public Sprite PlayerBullet;
    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q))
        {
            if(angleOfChange != 180)
            {
                angleOfChange = angleOfChange + 30f;
            }
            else
            {
                angleOfChange = 0;
            }
            //CancelInvoke(nameof(InvokePlayerShooting));
            //InvokeRepeating(nameof(InvokePlayerShooting), 0, 0.2f);
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, ConnectLineTo2);
        if (hit.collider != null && Input.GetMouseButton(0))
        {
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 10f, ConnectLineTo);
            if (hit2.collider != null )
            {
                if(hit2.collider.gameObject != ConnectObject)
                {
                    Connected = true;
                    ConnectObject = hit2.collider.gameObject;
                    Invoke(nameof(CatchInvoke), 2f);
                }
            }
        }
        else
        {
            CancelInvoke(nameof(CatchInvoke));
            Connected = false;
        }
        if (Connected && ConnectObject != null)
        {
            GetComponent<LineRenderer>().enabled = true;
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, ConnectObject.transform.position);
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }
    }
    public void CatchInvoke()
    {
        if (ConnectObject.name.Contains("Basic"))
        {
            AmountOfSlimes[0] += 1;
        }
        else if (ConnectObject.name.Contains("AllSides"))
        {
            AmountOfSlimes[1] += 1;
        }
        else if (ConnectObject.name.Contains("Mosquito"))
        {
            AmountOfSlimes[2] += 1;
        }
        else if (ConnectObject.name.Contains("Diagonal"))
        {
            AmountOfSlimes[3] += 1;
        }
        ConnectObject.GetComponent<Animation>().Play();
        ConnectObject.GetComponent<EnemyAI>().InvokeDestr1();
    }

    private void Start()
    {
        InvokeRepeating(nameof(InvokePlayerShooting), 0, 0.2f);
    }
    public void InvokePlayerShooting()
    {
        GameObject blt2 = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
        blt2.GetComponent<BulletScript>().AmIFromPlayer = true;
        blt2.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt2.GetComponent<SpriteRenderer>().sprite = PlayerBullet;
        if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == -90 || transform.eulerAngles.z == 270 || transform.eulerAngles.z == -270)
        {
            blt2.transform.localPosition = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.5f);
        }
        else
        {
            blt2.transform.localPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.2f);
        }
        blt2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.eulerAngles.z - angleOfChange));
        blt2.GetComponent<Rigidbody2D>().AddForce(blt2.transform.up * 1000f);

        GameObject blt3 = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
        blt3.GetComponent<BulletScript>().AmIFromPlayer = true;
        blt3.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt3.GetComponent<SpriteRenderer>().sprite = PlayerBullet;
        blt3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.eulerAngles.z + angleOfChange));
        if(transform.eulerAngles.z == 90 || transform.eulerAngles.z == -90 || transform.eulerAngles.z == 270 || transform.eulerAngles.z == -270)
        {
            blt3.transform.localPosition = new Vector2(transform.position.x - 0.2f, transform.position.y - 0.5f);
        }
        else
        {
            blt3.transform.localPosition = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.2f);
        }
        blt3.GetComponent<Rigidbody2D>().AddForce(blt3.transform.up * 1000f);
    }
}