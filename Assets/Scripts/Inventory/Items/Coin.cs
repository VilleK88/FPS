using UnityEngine;
public class Coin : MonoBehaviour
{
    public int value;
    public string coinID;
    [SerializeField] AudioClip collectSound;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.instance.cash += value;
            AddToCashIDsArray(coinID);
            AudioManager.instance.PlayPlayerSound(collectSound);
            gameObject.SetActive(false);
        }
    }
    void AddToCashIDsArray(string newCoinID)
    {
        string[] newCashIDs = new string[GameManager.instance.cashIDs.Length + 1];
        for(int i = 0; i < GameManager.instance.cashIDs.Length; i++)
            newCashIDs[i] = GameManager.instance.cashIDs[i];
        newCashIDs[GameManager.instance.cashIDs.Length] = newCoinID;
        GameManager.instance.cashIDs = newCashIDs;
    }
    [ContextMenu("Generate GUID FOR ID")]
    public void GenerateID()
    {
        coinID = System.Guid.NewGuid().ToString();
    }
}