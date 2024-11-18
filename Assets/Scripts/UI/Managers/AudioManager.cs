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
    public AudioSource audioSourcePlayerSFX;
    public AudioSource audioSourceEnemySFX;
    public AudioSource audioSourceExplosionSFX;
    public float originalVolume;
    private void Start()
    {
        originalVolume = audioSourcePlayerSFX.volume;
    }
    public void PlayPlayerSound(AudioClip clip)
    {
        audioSourcePlayerSFX.PlayOneShot(clip);
    }
    public void PlayEnemySound(AudioClip clip)
    {
        audioSourceEnemySFX.PlayOneShot(clip);
    }
    public void PlayExplosionSound(AudioClip clip)
    {
        audioSourceExplosionSFX.PlayOneShot(clip);
    }
}