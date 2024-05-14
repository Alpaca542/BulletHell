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

    private void Update()
    {
        slimeAmounts[0].text = "x" + gn.AmountOfSlimes[0].ToString();
        slimeAmounts[1].text = "x" + gn.AmountOfSlimes[1].ToString();
        slimeAmounts[2].text = "x" + gn.AmountOfSlimes[2].ToString();
        slimeAmounts[3].text = "x" + gn.AmountOfSlimes[3].ToString();
    }
    public void PlaceTeammate(int Who)
    {
        if(gh != null)
        {
            Destroy(gh);
        }
        gh = Instantiate(ghosts[Who], transform.position, Quaternion.identity);
        gh.GetComponent<GhostTeam>().realBrother = reals[Who];
    }
}
