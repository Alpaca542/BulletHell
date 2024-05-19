using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetingsManager : MonoBehaviour
{
    public Slider volumeslider;
    public Toggle SfxToggle;

    public void SetVolume(float volu)
    {
        PlayerPrefs.SetFloat("volume", volu);
    }
    public void ResetBtn()
    {
        PlayerPrefs.SetFloat("volume", 1);
        PlayerPrefs.SetString("sfx", "True");
        if (PlayerPrefs.GetString("sfx") == "True")
        {
            SfxToggle.isOn = true;
        }
        else
        {
            SfxToggle.isOn = false;
        }
        volumeslider.value = PlayerPrefs.GetFloat("volume");
    }
    public void SetJoystick(bool jst)
    {
        PlayerPrefs.SetString("sfx", jst.ToString());
    }
    public void Start()
    {
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1);
        }
        if (!PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetString("sfx", "True");
        }


        volumeslider.value = PlayerPrefs.GetFloat("volume");
        if (PlayerPrefs.GetString("sfx") == "True")
        {
            SfxToggle.isOn = true;
        }
        else
        {
            SfxToggle.isOn = false;
        }
    }
}