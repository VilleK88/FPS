using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public GameObject enemyAudioSourceObject;
    public float originalVolume;
    public LayerMask enemyLayer;
    private Transform lastClosestEnemy;
    private float groupSoundInterval = 0.1f;
    private float lastGroupSoundTime;
    private void Start()
    {
        originalVolume = audioSourcePlayerSFX.volume;
    }
    public void PlayPlayerSound(AudioClip clip)
    {
        audioSourcePlayerSFX.PlayOneShot(clip);
    }
    public void PlayEnemySound(AudioClip clip, Transform enemyTransform)
    {
        enemyAudioSourceObject.transform.position = enemyTransform.position;
        audioSourceEnemySFX.PlayOneShot(clip);
    }
    public void PlayExplosionSound(AudioClip clip)
    {
        audioSourceExplosionSFX.PlayOneShot(clip);
    }
}