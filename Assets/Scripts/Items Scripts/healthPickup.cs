using UnityEngine;

public class healthPickup : MonoBehaviour
{
    [SerializeField] ItemStats itemStats;
    int expGained = 5;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tag == "MedKit")
        {
            playerController playerController = other.GetComponent<playerController>();
            if (playerController != null)
            {
                // Calculate the health to add based on the player's max health
                float healthToAddFloat = Mathf.Min(100f - playerController.playerHealth, itemStats.healthAmount);
                int healthToAdd = Mathf.FloorToInt(healthToAddFloat); // Convert float to int

                if (gameManager.instance.playerScript.hasPickedUpHealthPack == false)
                {
                    gameManager.instance.tutorialUI.text = "This is a Medkit! If you are low on health this med kit will heal you instantly. If your health is full it goes to your invenetory! You can press H to heal yourself if you have a Medkit in your inventory, Press H to Continue";
                    gameManager.instance.tutorialUI.gameObject.SetActive(true);
                    Time.timeScale = 0f;
                }

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
                gameManager.instance.playerScript.GainExperience(expGained); // Add experience points

                // Destroy health pack object
                Destroy(gameObject);
            }
        }
    }
}
