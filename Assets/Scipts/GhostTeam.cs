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
            if (realBrother.name.Contains("Basic"))
            {
                GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>().AmountOfSlimes[0] -= 1;
            }
            else if (realBrother.name.Contains("AllSides"))
            {
                GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>().AmountOfSlimes[1] -= 1;
            }
            else if (realBrother.name.Contains("Mosquito"))
            {
                GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>().AmountOfSlimes[2] -= 1;
            }
            else if (realBrother.name.Contains("Diagonal"))
            {
                GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>().AmountOfSlimes[3] -= 1;
            }
            GameObject bro = Instantiate(realBrother, transform.position, Quaternion.identity);
            bro.layer = 6;
            bro.tag = "Player";
            bro.GetComponent<EnemyAI>().health *= 2;
            bro.GetComponent<EnemyAI>().AmIKind = true;
            Destroy(gameObject);
        }
    }
}
