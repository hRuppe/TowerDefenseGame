using UnityEngine;

public class healthPickup : MonoBehaviour
{
    [SerializeField] ItemStats Medkit;
    int expGained = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tag == "MedKit")
        {
            playerController playerController = other.GetComponent<playerController>();

            if (playerController != null)
            {
                // Check if the player has a medkit in their inventory
                bool hasMedkit = false;
                ItemStats medkit = null;

                foreach (ItemStats item in playerController.itemList)
                {
                    if (item.itemName == Medkit.itemName && item.itemCount > 0)
                    {
                        hasMedkit = true;
                        medkit = item;
                        break;
                    }
                }

                // If the player has a medkit, use it
                if (hasMedkit && medkit != null)
                {
                    // Consume the medkit from the inventory
                    medkit.itemCount--;

                    // Calculate the health to add based on the medkit's healthAmount
                    float healthToAddFloat = Mathf.Min(100f - playerController.playerHealth, Medkit.healthAmount);
                    int healthToAdd = Mathf.FloorToInt(healthToAddFloat); // Convert float to int

                    // Increase the player's health
                    playerController.playerHealth += healthToAdd;

                    // Display a message or play a sound to indicate the medkit has been used
                    Debug.Log("Medkit used. Health added: " + healthToAdd);
                }
                else
                {
                    // If the player doesn't have a medkit, display a message or play a sound
                    Debug.Log("No medkit available in inventory.");
                }

                // Add experience points
                gameManager.instance.playerScript.GainExperience(expGained);

                // Destroy health pack object
                Destroy(gameObject);
            }
        }
    }
}
