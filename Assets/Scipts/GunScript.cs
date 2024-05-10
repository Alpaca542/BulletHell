using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GameObject plr;
    public GameObject bullet;
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
        if (Input.GetMouseButtonDown(0))
        {
            GameObject blt = Instantiate(bullet, transform.position, Quaternion.identity);
            blt.GetComponent<Rigidbody2D>().AddForce(blt.transform.up*1000f);
        }
    }
}