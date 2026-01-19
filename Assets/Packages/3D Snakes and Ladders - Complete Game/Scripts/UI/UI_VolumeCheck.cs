using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_VolumeCheck : MonoBehaviour
{
    public GameObject soundIcon, noSoundIcon;
    bool isSound;

    private void Start()
    {
        if(PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            AudioListener.volume = 1;
            isSound = true;
            soundIcon.SetActive(true);
            noSoundIcon.SetActive(false);
        }
        else
        {
            AudioListener.volume = 0;
            isSound = false;
            soundIcon.SetActive(false);
            noSoundIcon.SetActive(true);
        }
    }

    public void VolumeChange()
    {
        if (isSound)
        {
            AudioListener.volume = 0;
            soundIcon.SetActive(false);
            noSoundIcon.SetActive(true);
            PlayerPrefs.SetInt("Sound", 0);
        }
        else
        {
            AudioListener.volume = 1;
            soundIcon.SetActive(true);
            noSoundIcon.SetActive(false);
            PlayerPrefs.SetInt("Sound", 1);
        }
        isSound = !isSound;
    }
}
