using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingCoins = 5;

    public int currentCoins { get; private set; }

    private void Awake()
    {
        currentCoins = startingCoins;
    }

    private void OnEnable()
    {
        GameEventsManager.instance.coinEvents.onCoinGained += CoinGained;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.coinEvents.onCoinGained -= CoinGained;
    }

    private void Start()
    {
        GameEventsManager.instance.coinEvents.CoinChange(currentCoins);
    }

    void CoinGained(int coin)
    {
        currentCoins += coin;
        GameEventsManager.instance.coinEvents.CoinChange(currentCoins);
    }
}
