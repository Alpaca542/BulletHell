using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float health;
    public float shootingDistance;
    public float shootingCooldown;
    public float searchingRadius;

    [Header("Settings")]
    public bool SouldSearch;
    public bool SouldShoot;
    public AnimationCurve ProjectlilesPattern;
    public bool[] SidesToShoot;
    public AnimationCurve BulletPath;
    public float bulletSpeed;
    public bool InvertPatterns;

    [Header("Fields")]
    public GameObject[] guns;
    private NavMeshAgent agent;
    public GameObject SlimeDeathParticles;
    public LayerMask playerLayer;
    public GameObject bullet;
    public GameObject target;

    [Header("Debug")]
    public bool AmIShooting;

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    //public void OnGetFromPool()
    //{
    //    gameObject.SetActive(true);
    //}

    //public void OnReturnToPool()
    //{
    //    gameObject.SetActive(false);
    //}
    private void Update()
    {
        if (SouldSearch)//Looks for a player in a radius
        {
            Collider2D searcher = Physics2D.OverlapCircle(transform.position, searchingRadius, playerLayer);
            if (searcher)
            {
                agent.SetDestination(searcher.transform.position);
            }
        }
        else
        {
            agent.SetDestination(target.transform.position);
        }


        if (SouldShoot)//Sends a ray, if it hits smth start a shooting invoke
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, (target.transform.position-transform.position).normalized, shootingDistance, playerLayer);
            if (ray)
            {
                if (!AmIShooting)
                {
                    AmIShooting = true;
                    InvokeRepeating(nameof(InvokeShoot), shootingCooldown, shootingCooldown);
                }
            }
            else
            {
                AmIShooting = false;
                CancelInvoke(nameof(InvokeShoot));
            }
        }

    }
    public void InvokeShoot()
    {
        for(int i = 0; i<4; i++)
        {
            if (SidesToShoot[i])
            {
                foreach (Keyframe kf in ProjectlilesPattern.keys)
                {
                    //GameObject blt = Instantiate(bullet, new Vector2(guns[i].transform.position.x + kf.value, guns[i].transform.position.y + kf.time), Quaternion.Euler(new Vector3(0, 0, guns[i].transform.rotation.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(kf.inTangent))));
                    GameObject blt = ((BulletScript)GameManager.Instance.pool.Get<BulletScript>()).gameObject;
                    blt.transform.rotation = Quaternion.Euler(new Vector3(0, 0, guns[i].transform.rotation.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(kf.inTangent)));
                    blt.GetComponent<BulletScript>().Path = BulletPath;
                    blt.GetComponent<BulletScript>().InvertPattern = InvertPatterns;
                    if(guns[i].transform.eulerAngles.z == 90 || guns[i].transform.eulerAngles.z == -90 || guns[i].transform.eulerAngles.z == 270 || guns[i].transform.eulerAngles.z == -270)
                    {
                        blt.transform.position = new Vector2(guns[i].transform.position.x + kf.value, guns[i].transform.position.y + kf.time);
                        blt.GetComponent<BulletScript>().startPosition = new Vector2(guns[i].transform.position.x + kf.value, guns[i].transform.position.y + kf.time);
                    }
                    else
                    {
                        blt.transform.position = new Vector2(guns[i].transform.position.x + kf.time, guns[i].transform.position.y + kf.value);
                        blt.GetComponent<BulletScript>().startPosition = new Vector2(guns[i].transform.position.x + kf.time, guns[i].transform.position.y + kf.value);
                    }
                    blt.GetComponent<BulletScript>().speed = bulletSpeed;
                    blt.GetComponent<BulletScript>().StartRotation = guns[i].transform.eulerAngles;
                    blt.transform.Rotate(new Vector3(0, 0, 90));
                    //blt.GetComponent<Rigidbody2D>().AddForce(blt.transform.up * 1000f);
                }
            }
        }
    }
    public void Die()
    {
        Instantiate(SlimeDeathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}