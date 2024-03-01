using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearDamage : MonoBehaviour
{
    private bool spearContactedPlayer = false; 

    public void OnTriggerEnter(Collider other)
    {
        // Sets spearContactedPlayer to true if the spear collider hits the player
        if (other.CompareTag("Player"))
        {
            spearContactedPlayer = true; 
        }
    }

    // Getter for spearContactedPlayer bool
    public bool GetSpearContactedPlayer()
    {
        return spearContactedPlayer;
    }

    // Resets spearContactedPlayer bool
    public void ResetSpearContactedPlayer()
    {
        spearContactedPlayer = false; 
    }
}
