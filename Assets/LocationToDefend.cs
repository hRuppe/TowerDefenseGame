using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocationToDefend : MonoBehaviour
{
    [SerializeField] int locationHealth;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enemy entered trigger"); 
        enemyAI enemy = other.GetComponent<enemyAI>();

        enemy.inAttackRange = true; 
    }
}
