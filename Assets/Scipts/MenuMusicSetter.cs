using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicSetter : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.SetMenuMusic();
    }
}
