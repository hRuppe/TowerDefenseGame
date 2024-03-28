using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    [SerializeField] public ItemStats itemStats; // Reference to the item stats of the item to be purchased

    private void Start()
    {
        // Remove existing listeners before adding a new one
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        // Add listener to the button click event
        button.onClick.AddListener(() => PurchaseItem());
    }

    void PurchaseItem()
    {
        // Check if the player has enough currency to purchase the item
        if (gameManager.instance.playerScript.GetCurrency() >= itemStats.itemPrice)
        {
            // Deduct the item price from the player's currency
            gameManager.instance.playerScript.DecreaseCurrency(itemStats.itemPrice);

            // Update the UI to reflect the new currency amount
            gameManager.instance.updateCurrency();

            // Add the purchased item to the player's itemList
            gameManager.instance.playerScript.itemList.Add(itemStats);

        }
        else
        {
            Debug.Log("Not enough currency to purchase the item.");
        }
    }
}
