using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GhostTeam : MonoBehaviour
{
    public GameObject realBrother;
    public LayerMask playerLayer;
    private void Update()
    {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        camPos.z = 0;
        transform.position = camPos;
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            GameObject bro = Instantiate(realBrother, transform.position, Quaternion.identity);
            bro.layer = 6;
            bro.tag = "Player";
            bro.GetComponent<EnemyAI>().AmIKind = true;
            Destroy(gameObject);
        }
    }
}
