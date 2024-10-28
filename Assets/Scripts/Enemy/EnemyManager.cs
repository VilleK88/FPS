using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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
    Player player;
    public LayerMask enemyLayer;
    public float enemyDataRadius = 100f;
    private void Start()
    {
        player = PlayerManager.instance.GetPlayer().GetComponent<Player>();
        //enemies = FindObjectsOfType<StatePatternEnemy>();
        if (GameManager.instance.loadPlayerPosition)
            LoadEnemiesData();
    }
    public void SaveEnemiesData()
    {
        GameManager.instance.nearbyEnemies.Clear();
        Vector3 playerPosition = player.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, enemyDataRadius, enemyLayer);
        foreach(Collider collider in colliders)
        {
            StatePatternEnemy enemy = collider.GetComponent<StatePatternEnemy>();
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                EnemyData enemyData = new EnemyData
                {
                    enemyDataID = enemy.enemyID,
                    enemyPositionX = enemy.transform.position.x,
                    enemyPositionY = enemy.transform.position.y,
                    enemyPositionZ = enemy.transform.position.z,
                    waypointIndexData = enemy.patrolState.waypointIndex,
                    dead = enemyHealth.dead,
                    alreadyFoundDead = enemyHealth.alreadyFoundDead
                };
                GameManager.instance.nearbyEnemies.Add(enemyData);
            }
        }
        /*GameManager.instance.enemyPositionX = new float[enemies.Length];
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
        }*/
    }
    public void LoadEnemiesData()
    {
        Vector3 playerPosition = player.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, enemyDataRadius, enemyLayer);
        foreach (Collider collider in colliders)
        {
            StatePatternEnemy enemy = collider.GetComponent<StatePatternEnemy>();
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                EnemyData enemyData = GameManager.instance.nearbyEnemies.Find(enemyData => enemyData.enemyDataID == enemy.enemyID);
                if(enemyData != null)
                {
                    enemy.transform.position = new Vector3(enemyData.enemyPositionX, enemyData.enemyPositionY, enemyData.enemyPositionZ);
                    enemy.patrolState.waypointIndex = enemyData.waypointIndexData;
                    enemyHealth.dead = enemyData.dead;
                    enemyHealth.alreadyFoundDead = enemyData.alreadyFoundDead;
                    if (enemyHealth.dead)
                        enemyHealth.DieData();
                }
            }
        }
        /*for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].transform.position = new Vector3(GameManager.instance.enemyPositionX[i], GameManager.instance.enemyPositionY[i], GameManager.instance.enemyPositionZ[i]);
                enemies[i].transform.rotation = Quaternion.Euler(GameManager.instance.enemyRotationX[i], GameManager.instance.enemyRotationY[i], GameManager.instance.enemyRotationZ[i]);
                enemies[i].patrolState.waypointIndex = GameManager.instance.patrolWaypointIndex[i];
            }
        }*/
    }
    public IEnumerator BackToPatrol()
    {
        indicatorImage.sprite = patrolImage;
        yield return new WaitForSeconds(3);
        indicatorImage.enabled = false;
    }
    public void CloseEnemyHealthbars()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                EnemyHealth enemyHealth = enemies[i].GetComponent<EnemyHealth>();
                if (enemyHealth != null && enemyHealth.healthbar.activeSelf)
                    enemyHealth.HideHealth();
            }
        }
    }
    public bool CanAnyoneSeeThePlayer()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].canSeePlayer == true)
                return true;
        }
        return false;
    }
    public bool CloseIndicatorImage()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].currentState != enemies[i].patrolState)
                return false;
        }
        return true;
    }
}