using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GameObject plr;
    public float angleOfChange;
    public bool Connected = false;
    public LayerMask ConnectLineTo, ConnectLineTo2;
    public GameObject ConnectObject;
    //public GameObject bullet;
    void Update()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        if ((plr.transform.rotation == Quaternion.Euler(0, 180, 0) || plr.transform.rotation == Quaternion.Euler(0, -180, 0)))
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, (90 - rot_z) * Time.timeScale);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, (rot_z - 90) * Time.timeScale);
        }
        if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - plr.transform.position.x) < 0.2f && Input.GetAxis("Horizontal") == 0 && plr.transform.rotation != Quaternion.Euler(0, 180, 0) && plr.transform.rotation != Quaternion.Euler(0, -180, 0))
        {
            plr.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - plr.transform.position.x) > 0.2f && Input.GetAxis("Horizontal") == 0 && plr.transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            plr.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
   //     if (Input.GetMouseButtonDown(0))
   //     {
			//GameObject blt = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
   //         blt.GetComponent<BulletScript>().AmIFromPlayer = true;
   //         blt.transform.position = transform.position;
   //         blt.transform.rotation = transform.rotation;
   //         blt.GetComponent<Rigidbody2D>().AddForce(blt.transform.up*1000f);
   //     }
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
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, ConnectLineTo2);
        if (hit.collider != null && Input.GetMouseButton(0))
        {
            //CancelInvoke(nameof(ContinueTime));
            //ContinueTime();
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 10f, ConnectLineTo);
            if (hit2.collider != null )
            {
                Connected = true;
                ConnectObject = hit2.collider.gameObject;
            }
        }
        else
        {
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
    private void Start()
    {
        InvokeRepeating(nameof(InvokePlayerShooting), 0, 0.2f);
    }
    public void InvokePlayerShooting()
    {
        GameObject blt2 = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
        blt2.GetComponent<BulletScript>().AmIFromPlayer = true;
        blt2.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt2.transform.localPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.2f);
        blt2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.rotation.z));
        blt2.transform.Rotate(new Vector3(0, 0, angleOfChange));
        blt2.GetComponent<Rigidbody2D>().AddForce(blt2.transform.up * 1000f);

        GameObject blt3 = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
        blt3.GetComponent<BulletScript>().AmIFromPlayer = true;
        blt3.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt3.transform.localPosition = new Vector2(transform.position.x - 0.5f, transform.position.y-0.2f);
        blt2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.rotation.z));
        blt3.transform.Rotate(new Vector3(0, 0, -angleOfChange));
        blt3.GetComponent<Rigidbody2D>().AddForce(blt3.transform.up * 1000f);
    }
}