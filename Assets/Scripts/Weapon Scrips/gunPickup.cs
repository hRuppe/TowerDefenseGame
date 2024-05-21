using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gunStat;

    int expGained = 10;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.gunPickup(gunStat); // Updated the gunList with the gunStats
            gameManager.instance.playerScript.GainExperience(expGained); // Add experience points
            Destroy(gameObject);
        }
    }

}
