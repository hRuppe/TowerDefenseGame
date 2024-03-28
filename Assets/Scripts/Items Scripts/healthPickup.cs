using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerController = other.GetComponent<playerController>();
            if (playerController != null)
            {
                // Calculate the health to add based on the player's max health
                float healthToAddFloat = Mathf.Min(100f - playerController.playerHealth, itemStats.healthAmount);
                int healthToAdd = Mathf.FloorToInt(healthToAddFloat); // Convert float to int

                // If player's health is less than 100, add health
                if (playerController.playerHealth < 100)
                {
                    playerController.playerHealth += healthToAdd;
                }
                else
                {
                    // Otherwise, add itemStats to player's items list
                    playerController.ItemPickup(itemStats);
                }

                // Destroy health pack object
                Destroy(gameObject);
            }
        }
    }
}
