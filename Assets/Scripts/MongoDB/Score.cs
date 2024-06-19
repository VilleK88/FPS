using TMPro;
using UnityEngine;
public class Score : MonoBehaviour
{
    #region Singleton
    public static Score Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion
    public GameAccount accountData;
    [Header("UI")]
    public TextMeshProUGUI killsText;
    //public TextMeshProUGUI goldText;
    private void Start()
    {
        if (AccountManager.Instance != null)
        {
            if (AccountManager.Instance.loggedIn)
            {
                accountData.kills = AccountManager.Instance.loggedInAccount.kills;
                killsText.enabled = true;
                if (killsText.enabled)
                    killsText.text = "Kills: " + accountData.kills;
            }
        }
    }
    public void UpdateKills()
    {
        killsText.text = "Kills: " + accountData.kills++;
    }
}