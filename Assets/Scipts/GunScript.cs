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
        //if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q))
        //{
        //    if(angleOfChange != 180)
        //    {
        //        angleOfChange = angleOfChange + 30f;
        //    }
        //    else
        //    {
        //        angleOfChange = 0;
        //    }
        //    //CancelInvoke(nameof(InvokePlayerShooting));
        //    //InvokeRepeating(nameof(InvokePlayerShooting), 0, 0.2f);
        //}

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, ConnectLineTo2);
        if (hit.collider != null && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized, 100f, ConnectLineTo);
            if (hit2.collider != null )
            {
                Connected = true;
                ConnectObject = hit2.collider.gameObject;
                CancelInvoke(nameof(CatchInvoke));
                Invoke(nameof(CatchInvoke), 4f);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            CancelInvoke(nameof(CatchInvoke));
            Connected = false;
        }
        if (Connected && ConnectObject != null && ConnectObject.GetComponent<EnemyAI>().enabled && ConnectObject.gameObject.tag != "Boss")
        {
            GetComponent<LineRenderer>().enabled = true;
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, ConnectObject.transform.position);
        }
        else
        {
            CancelInvoke(nameof(CatchInvoke));
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
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
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, transform.position);
        GetComponent<LineRenderer>().enabled = false;
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
        blt2.GetComponent<BulletScript>().damage = plr.GetComponent<Player>().damage;
        blt2.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt2.GetComponent<SpriteRenderer>().sprite = PlayerBullet;

        Vector2 newPosition = new Vector2(-0.3f, 0.5f);
        Vector3 rotatedVector = Quaternion.AngleAxis(plr.transform.eulerAngles.z, Vector3.forward) * newPosition;
        blt2.transform.localPosition = transform.position + rotatedVector;

        blt2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.eulerAngles.z));
        blt2.GetComponent<Rigidbody2D>().AddForce(blt2.transform.up * 1000f);

        GameObject blt3 = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
        blt3.GetComponent<BulletScript>().AmIFromPlayer = true;
        blt3.transform.localScale = new Vector3(0.03f, 0.03f, 1f);
        blt3.GetComponent<SpriteRenderer>().sprite = PlayerBullet;
        Vector2 newPosition2 = new Vector2(0.3f, 0.5f);
        Vector3 rotatedVector2 = Quaternion.AngleAxis(plr.transform.eulerAngles.z, Vector3.forward) * newPosition2;
        blt3.transform.localPosition = transform.position + rotatedVector2;
        blt3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, plr.transform.eulerAngles.z));
        blt3.GetComponent<Rigidbody2D>().AddForce(blt3.transform.up * 1000f);
    }
}