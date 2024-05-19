using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class soundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource sound;
    public AudioClip[] soundslist;
    public bool shouldILoop = false;
    public bool amIPlaying = false;
    public bool shouldIPlayOnStart = false;

    void Awake()
    {
        sound = GetComponent<AudioSource>();
        if (shouldIPlayOnStart)
        { PlaySound(0); }
    }

    // Update is called once per frame

    public void PlaySound(int whichsound)
    {
        sound.clip = soundslist[whichsound];
        sound.volume = PlayerPrefs.GetFloat("volume");
        sound.Play();
    }

    public void letmeplay()
    {
        amIPlaying = false;
    }
}