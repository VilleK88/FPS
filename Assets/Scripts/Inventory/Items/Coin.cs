using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;
    [SerializeField] AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.instance.cash += value;
            AudioManager.instance.PlaySound(collectSound);
            gameObject.SetActive(false);
        }
    }
}
