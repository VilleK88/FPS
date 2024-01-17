using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public CoinEvents coinEvents;
    public MiscEvents miscEvents;

    #region Singleton
    public static GameEventsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Initialize all events
        inputEvents = new InputEvents();
        playerEvents = new PlayerEvents();
        coinEvents = new CoinEvents();
        miscEvents = new MiscEvents();
    }
    #endregion
}
