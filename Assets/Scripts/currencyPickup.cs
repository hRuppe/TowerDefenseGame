using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currencyPickup : MonoBehaviour
{
    [SerializeField] int currencyAmount;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Increase the player's currency
            other.GetComponent<playerController>().IncreaseCurrency(currencyAmount);

            // Update the UI to reflect the new currency amount
            gameManager.instance.updateCurrency();

            // Destroy the currency pickup GameObject
            Destroy(gameObject);
        }
    }
}
