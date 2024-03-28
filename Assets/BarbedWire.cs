using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BarbedWire : MonoBehaviour
{
    [SerializeField] float slowDownPercentage;

    TowerAttackingEnemy towerAttacker;
    PlayerAttackingEnemy playerAttacker; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            // Modify speed
            if (other.TryGetComponent<TowerAttackingEnemy>(out TowerAttackingEnemy towerAttackScript))
            {
                towerAttacker = towerAttackScript;
                towerAttacker.ChangeEnemySpeed(slowDownPercentage);
            }
            else if (other.TryGetComponent<PlayerAttackingEnemy>(out PlayerAttackingEnemy playerAttackScript))
            {
                playerAttacker = playerAttackScript;
                playerAttacker.ChangeEnemySpeed(slowDownPercentage);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset enemy speed
        if (towerAttacker != null)
        {
            towerAttacker.ResetSpeed();
            towerAttacker = null; 
        }
        else if (playerAttacker != null)
        {
            playerAttacker.ResetSpeed();
            playerAttacker = null; 
        }
    }
}
