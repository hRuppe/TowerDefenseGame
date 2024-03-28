using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemStats itemStats;
    int expGained = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            gameManager.instance.playerScript.ItemPickup(itemStats); // Updates the items list with item stats
            gameManager.instance.playerScript.GainExperience(expGained); // Add experience points
            Destroy(gameObject);
        }
    }
}
