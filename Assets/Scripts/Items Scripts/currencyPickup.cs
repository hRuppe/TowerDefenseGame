using UnityEngine;

public class currencyPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;
    int expGained = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Increase the player's currency
            other.GetComponent<playerController>().IncreaseCurrency(itemStats.currencyAmount);

            // Update the UI to reflect the new currency amount
            gameManager.instance.updateCurrency();

            gameManager.instance.playerScript.GainExperience(expGained); // Add experience points

            // Destroy the currency pickup GameObject
            Destroy(gameObject);
        }
    }
}
