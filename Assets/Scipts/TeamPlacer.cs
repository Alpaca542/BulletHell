using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPlacer : MonoBehaviour
{
    public GameObject[] ghosts;
    public GameObject[] reals;
    public GameObject gh;
    public GunScript gn;
    public Text[] slimeAmounts;
    public GameObject cnvv;
    public SpawnEnemies enemsp;
    private Coroutine crtn;

    private void Update()
    {
        slimeAmounts[0].text = "x" + gn.AmountOfSlimes[0].ToString();
        slimeAmounts[1].text = "x" + gn.AmountOfSlimes[1].ToString();
        slimeAmounts[2].text = "x" + gn.AmountOfSlimes[2].ToString();
        slimeAmounts[3].text = "x" + gn.AmountOfSlimes[3].ToString();
    }
    public void PlaceTeammate(int Who)
    {
        if (GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>().AmountOfSlimes[Who] > 0)
        {
            if (gh != null)
            {
                Destroy(gh);
            }
            gh = Instantiate(ghosts[Who], transform.position, Quaternion.identity);
            gh.GetComponent<GhostTeam>().realBrother = reals[Who];
        }
        else
        {
            slimeAmounts[Who].color = new Color32(255, 0, 0, 255);
            if(crtn != null)
            {
                StopCoroutine(crtn);
            }
            crtn = StartCoroutine(InvokeRedText(Who));
        }
    }
    public IEnumerator InvokeRedText(int Who)
    {
        yield return new WaitForSeconds(0.3f);
        slimeAmounts[Who].color = new Color32(0, 0, 0, 255);
    }
    public void StartTheBattle()
    {
        cnvv.SetActive(false);
        enemsp.SpawnABoss();
    }
}
