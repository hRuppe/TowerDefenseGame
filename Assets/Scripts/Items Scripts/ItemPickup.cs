using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemStats itemStats;
    int expGained = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                // Check if the player already has the item in their inventory
                bool hasItem = false;
                foreach (ItemStats item in player.itemList)
                {
                    if (item.itemName == itemStats.itemName)
                    {
                        // Increment the itemCount if the player already has the item
                        item.itemCount++;
                        hasItem = true;
                        break;
                    }
                }

                // If the player doesn't have the item, add it to their inventory
                if (!hasItem)
                {
                    player.ItemPickup(itemStats); // Updates the items list with item stats
                }

                // Add experience points
                player.GainExperience(expGained);

                Destroy(gameObject);
            }
        }
    }
}
