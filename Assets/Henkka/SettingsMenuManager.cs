using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class SettingsMenuManager : MonoBehaviour
{
    #region Singleton
    public static SettingsMenuManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion
    public GameObject SettingsMenu;
    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [Header("Sound")]
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer MainAudioMixer;
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity;
    public Slider mouseSensitivitySlider;
    private const string SensitivityPrefKey = "MouseSensitivity";
    GameObject player;
    MouseLook mouseLookScript;
    private void Start()
    {
        masterVol.value = PlayerPrefs.GetFloat("MasterVolume", masterVol.value);
        musicVol.value = PlayerPrefs.GetFloat("MusicVolume", musicVol.value);
        sfxVol.value = PlayerPrefs.GetFloat("SFXVolume", sfxVol.value);
        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeSFXVolume();
        masterVol.onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
        musicVol.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        sfxVol.onValueChanged.AddListener(delegate { ChangeSFXVolume(); });
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        LoadSensitivity();
        if(mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = mouseSensitivity;
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensivityChanged);
        }
        if(PlayerManager.instance != null)
        {
            player = PlayerManager.instance.GetPlayer();
            if (player != null)
            {
                mouseLookScript = player.GetComponentInChildren<MouseLook>();
                if (mouseLookScript != null)
                    mouseLookScript.mouseSensitivity = mouseSensitivity;
            }
        }
    }
    public void ChangeMasterVolume()
    {
        MainAudioMixer.SetFloat("MasterVol", masterVol.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVol.value);
        PlayerPrefs.Save();
    }
    public void ChangeMusicVolume()
    {
        MainAudioMixer.SetFloat("MusicVol", musicVol.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVol.value);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume()
    {
        MainAudioMixer.SetFloat("SFXVol", sfxVol.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVol.value);
        PlayerPrefs.Save();
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    void OnSensivityChanged(float value)
    {
        mouseSensitivity = value;
        SaveSensitivity();

    }
    void SaveSensitivity()
    {
        PlayerPrefs.SetFloat(SensitivityPrefKey, mouseSensitivity);
        PlayerPrefs.Save();
    }
    void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey(SensitivityPrefKey))
            mouseSensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey);
    }
}