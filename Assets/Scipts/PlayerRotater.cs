using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotater : MonoBehaviour
{
    public GameObject plr;
    void Update()
    {
        //Camera.main.ScreenToWorldPoint(Input.mousePosition)
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        if ((plr.transform.rotation == Quaternion.Euler(0, 180, 0) || plr.transform.rotation == Quaternion.Euler(0, -180, 0)))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (90 - rot_z) * Time.timeScale);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (rot_z - 90) * Time.timeScale);
        }
    }
}