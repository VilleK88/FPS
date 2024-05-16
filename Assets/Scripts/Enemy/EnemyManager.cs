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
    //public Image imageIndicator;
    public TextMeshProUGUI indicatorText;
    private void Start()
    {
        enemies = FindObjectsOfType<StatePatternEnemy>();
        indicatorText.text = "";
    }
    public IEnumerator BackToPatrol()
    {
        indicatorText.text = "Enemy goes back to patrol state...";
        TextMeshProUGUI currentIndicatorText = indicatorText;
        yield return new WaitForSeconds(3);
        if(currentIndicatorText == indicatorText)
            indicatorText.text = "";
    }
}