using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currencyPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;
    int expGained = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tag == "Coin")
        {
            // Increase the player's currency
            other.GetComponent<playerController>().IncreaseCurrency(itemStats.currencyAmount);

            // Update the UI to reflect the new currency amount
            gameManager.instance.updateCurrency();

            if (gameManager.instance.playerScript.hasPickedUpCoin == false)
            {
                gameManager.instance.tutorialUI.text = "You picked up a coin! You can use these to buy MedKits and Barbed wire Press B to open the Buy menu! Then go to the Shop Menu!";
                gameManager.instance.tutorialUI.gameObject.SetActive(true);
                Time.timeScale = 0f;
                gameManager.instance.playerScript.hasPickedUpCoin = true;
            }

            gameManager.instance.playerScript.GainExperience(expGained); // Add experience points

            // Destroy the currency pickup GameObject
            Destroy(gameObject);
        }
    }
}
