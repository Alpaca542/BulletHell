using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPlacer : MonoBehaviour
{
    public GameObject[] ghosts;
    public GameObject[] reals;
    public void PlaceTeammate(int Who)
    {
        GameObject gh = Instantiate(ghosts[Who], transform.position, Quaternion.identity);
        gh.GetComponent<GhostTeam>().realBrother = reals[Who];
    }
}
