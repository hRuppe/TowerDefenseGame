using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LocationToDefend : MonoBehaviour
{
    [SerializeField] int locationHealth;

    private void Start()
    {
        // Set defensive location max health
        gameManager.instance.defensiveLocationHealthBar.maxValue = locationHealth;
        gameManager.instance.defensiveLocationHealthBar.value = locationHealth;
    }

    public void TakeDamage(int dmg)
    {
        locationHealth -= dmg;

        UpdateDefenseLocationHealthBar(locationHealth); 

        if (locationHealth <= 0)
        {
            gameManager.instance.YouLose(); 
        }
    }

    private void UpdateDefenseLocationHealthBar(float currentHealth)
    {
        gameManager.instance.defensiveLocationHealthBar.value = currentHealth; 
    }

}
