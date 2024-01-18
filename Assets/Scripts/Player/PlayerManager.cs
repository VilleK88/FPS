using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public GameObject player;
    //public VectorValue startingPosition;

    public GameObject GetPlayer()
    {
        return player;
    }

    private void Start()
    {
        /*if(GameManager.instance.loadPlayerPosition)
        {
            LoadPlayerTransformPosition();
            Debug.Log("PlayerManagerin LoadPlayerTransformPosition debuggaa");
            GameManager.instance.loadPlayerPosition = false;
        }
        else
        {
            player.transform.position = startingPosition.initialValue;
            Debug.Log("PlayerManagerin startingPosition.initialValue debuggaa");
        }*/
    }

    /*public void SavePlayerTransformPosition()
    {
        GameManager.instance.x = player.transform.position.x;
        GameManager.instance.y = 4;
        GameManager.instance.z = player.transform.position.z;
    }

    public void LoadPlayerTransformPosition()
    {
        player.transform.position = new Vector3(GameManager.instance.x, GameManager.instance.y, GameManager.instance.z);
    }*/
}
