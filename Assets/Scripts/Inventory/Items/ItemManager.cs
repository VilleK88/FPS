using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private void Start()
    {
        Coin[] coinsInScene = FindObjectsOfType<Coin>();
        foreach(Coin coin in coinsInScene)
        {
            if(GameManager.instance.cashIDs.Contains(coin.coinID))
            {
                Destroy(coin.gameObject);
            }
        }
    }
}
