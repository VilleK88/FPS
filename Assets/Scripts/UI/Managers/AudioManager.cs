using System.Collections;
using System.Collections.Generic;
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
    public AudioSource audioSourceEnemyCloseSFX;
    public AudioSource audioSourceEnemyDistantSFX;
    public AudioSource audioSourceExplosionSFX;
    public float originalVolume;
    public static int activeSounds = 0;
    public int maxActiveSounds = 10;
    private List<AudioClip> queuedClips = new List<AudioClip>();
    private List<float> distances = new List<float>();
    private void Start()
    {
        originalVolume = audioSourcePlayerSFX.volume;
    }
    private void Update()
    {
        if(queuedClips.Count > 0 && activeSounds < maxActiveSounds)
        {
            int closestIndex = distances.IndexOf(distances.Min());
            PlayEnemySound(queuedClips[closestIndex], audioSourceEnemyCloseSFX.transform);
            queuedClips.RemoveAt(closestIndex);
            distances.RemoveAt(closestIndex);
        }
    }
    public void PlayPlayerSound(AudioClip clip)
    {
        audioSourcePlayerSFX.PlayOneShot(clip);
    }
    public void PlayEnemySound(AudioClip clip, Transform enemyTransform)
    {
        //audioSourceEnemySFX.PlayOneShot(clip);
        Transform playerTransform = PlayerManager.instance.GetPlayer().transform;
        float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
        if(activeSounds >= maxActiveSounds)
        {
            queuedClips.Add(clip);
            distances.Add(distance);
            return;
        }
        audioSourceEnemyCloseSFX.transform.position = enemyTransform.position;
        audioSourceEnemyCloseSFX.PlayOneShot(clip);
        activeSounds++;
        StartCoroutine(ResetSoundCount(clip.length));
    }
    public void PlayExplosionSound(AudioClip clip)
    {
        audioSourceExplosionSFX.PlayOneShot(clip);
    }
    private IEnumerator ResetSoundCount(float delay)
    {
        yield return new WaitForSeconds(delay);
        activeSounds--;
    }
}