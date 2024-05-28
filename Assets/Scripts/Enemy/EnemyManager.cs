using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EnemyManager : MonoBehaviour
{
    #region Singleton
    public static EnemyManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion
    public StatePatternEnemy[] enemies;
    public Image indicatorImage;
    public Sprite patrolImage;
    public Sprite alertImage;
    public Sprite trackingImage;
    public Sprite combatImage;
    private void Start()
    {
        //enemies = FindObjectsOfType<StatePatternEnemy>();
        if (GameManager.instance.loadPlayerPosition)
            LoadEnemiesData();
    }
    public void SaveEnemiesData()
    {
        GameManager.instance.enemyPositionX = new float[enemies.Length];
        GameManager.instance.enemyPositionY = new float[enemies.Length];
        GameManager.instance.enemyPositionZ = new float[enemies.Length];
        GameManager.instance.enemyRotationX = new float[enemies.Length];
        GameManager.instance.enemyRotationY = new float[enemies.Length];
        GameManager.instance.enemyRotationZ = new float[enemies.Length];
        GameManager.instance.patrolWaypointIndex = new int[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                GameManager.instance.enemyPositionX[i] = enemies[i].transform.position.x;
                GameManager.instance.enemyPositionY[i] = enemies[i].transform.position.y;
                GameManager.instance.enemyPositionZ[i] = enemies[i].transform.position.z;
                GameManager.instance.enemyRotationX[i] = enemies[i].transform.rotation.eulerAngles.x;
                GameManager.instance.enemyRotationY[i] = enemies[i].transform.rotation.eulerAngles.y;
                GameManager.instance.enemyRotationZ[i] = enemies[i].transform.rotation.eulerAngles.z;
                GameManager.instance.patrolWaypointIndex[i] = enemies[i].patrolState.waypointIndex;
            }
        }
    }
    public void LoadEnemiesData()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].transform.position = new Vector3(GameManager.instance.enemyPositionX[i], GameManager.instance.enemyPositionY[i], GameManager.instance.enemyPositionZ[i]);
                enemies[i].transform.rotation = Quaternion.Euler(GameManager.instance.enemyRotationX[i], GameManager.instance.enemyRotationY[i], GameManager.instance.enemyRotationZ[i]);
                enemies[i].patrolState.waypointIndex = GameManager.instance.patrolWaypointIndex[i];
            }
        }
    }
    public IEnumerator BackToPatrol()
    {
        indicatorImage.sprite = patrolImage;
        yield return new WaitForSeconds(3);
        indicatorImage.enabled = false;
    }
    public void CloseEnemyHealthbars()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyHealth enemyHealth = enemies[i].GetComponent<EnemyHealth>();
            if (enemyHealth != null && enemyHealth.healthbar.activeSelf)
                enemyHealth.HideHealth();
        }
    }
    public bool CanAnyoneSeeThePlayer()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].canSeePlayer == true)
                return true;
        }
        return false;
    }
    public bool CloseIndicatorImage()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].currentState != enemies[i].patrolState)
                return false;
        }
        return true;
    }
}