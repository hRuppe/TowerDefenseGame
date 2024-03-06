using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickup : MonoBehaviour
{
    [SerializeField] int healthAmount;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController playerController = other.GetComponent<playerController>();
            if (playerController != null)
            {
                // Calculate the health to add based on the player's max health
                int healthToAdd = Mathf.Min(100 - playerController.playerHealth, healthAmount);

                // If player's health is less than 100, add health
                if (playerController.playerHealth < 100)
                {
                    playerController.playerHealth += healthToAdd;
                }
                else
                {
                    // Otherwise, add health pack to player's items list
                    playerController.ItemPickup(gameObject);
                }

                // Destroy health pack object
                Destroy(gameObject);
            }
        }
    }
}
