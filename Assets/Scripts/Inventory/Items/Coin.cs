using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;
    public int coinID;
    [SerializeField] AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.instance.cash += value;
            CollectCoin();
            AddToCashIDsArray(coinID);
            AudioManager.instance.PlaySound(collectSound);
            gameObject.SetActive(false);
        }
    }

    void CollectCoin()
    {
        GameEventsManager.instance.coinEvents.CoinGained(value);
        GameEventsManager.instance.miscEvents.CoinCollected();
    }

    void AddToCashIDsArray(int newCoinID)
    {
        int[] newCashIDs = new int[GameManager.instance.cashIDs.Length + 1];
        for(int i = 0; i < GameManager.instance.cashIDs.Length; i++)
        {
            newCashIDs[i] = GameManager.instance.cashIDs[i];
        }
        newCashIDs[GameManager.instance.cashIDs.Length] = newCoinID;
        GameManager.instance.cashIDs = newCashIDs;
    }

    public void GenerateID()
    {
        coinID = UnityEngine.Random.Range(0, 100000000);
    }
}
