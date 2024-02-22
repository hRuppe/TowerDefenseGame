using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocationToDefend : MonoBehaviour
{
    [SerializeField] int locationHealth;

    public void TakeDamage(int dmg)
    {
        locationHealth -= dmg; 
    }

}
