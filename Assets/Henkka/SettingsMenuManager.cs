using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer MainAudioMixer;


    public void ChangeMasterVolume()
    {
        MainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }

    public void ChangeMusicVolume()
    {
        MainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }

    public void ChangeSFXVolume()
    {
        MainAudioMixer.SetFloat("SFXVol", sfxVol.value);
    }

    public void ExitSettings()
    {
        SettingsMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            {
                ExitSettings();
            }
        }
    }
}
