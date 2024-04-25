using System.Linq;
using UnityEngine;
public class ItemManager : MonoBehaviour
{
    private void Start()
    {
        Coin[] coinsInScene = FindObjectsOfType<Coin>();
        foreach(Coin coin in coinsInScene)
        {
            if(GameManager.instance.cashIDs.Contains(coin.coinID))
                Destroy(coin.gameObject);
        }
        ItemPickup[] itemPickupsInScene = FindObjectsOfType<ItemPickup>();
        foreach(ItemPickup itemPickup in itemPickupsInScene)
        {
            if (GameManager.instance.itemPickUpIDs.Contains(itemPickup.pickUpItemID))
                Destroy(itemPickup.gameObject);
        }
    }
}