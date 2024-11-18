using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance;

    private void Awake()
    {
        
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    #endregion
    public AudioSource audioSourceSFX;
    public float originalVolume;
    private void Start()
    {
        originalVolume = audioSourceSFX.volume;
    }
    public void PlaySound(AudioClip clip)
    {
        audioSourceSFX.PlayOneShot(clip);
    }
}