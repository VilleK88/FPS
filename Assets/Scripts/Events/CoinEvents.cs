using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CoinEvents
{
    public event Action<int> onCoinGained;
    public void CoinGained(int coin)
    {
        if(onCoinGained != null)
        {
            onCoinGained(coin);
        }
    }

    public event Action<int> onCoinChange;
    public void CoinChange(int coin)
    {
        if(onCoinChange != null)
        {
            onCoinChange(coin);
        }
    }
}
