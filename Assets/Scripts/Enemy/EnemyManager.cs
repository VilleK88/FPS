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
    public TextMeshProUGUI indicatorText;
    public Image indicatorImage;
    public Sprite patrolImage;
    public Sprite alertImage;
    public Sprite trackingImage;
    public Sprite combatImage;
    private void Start()
    {
        enemies = FindObjectsOfType<StatePatternEnemy>();
        indicatorText.text = "";
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