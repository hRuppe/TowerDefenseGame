using System.Collections.Generic;
using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    [SerializeField] float slowDownPercentage;
    [SerializeField] int drainDmg;
    [SerializeField] float damageInterval;
    [SerializeField] int defensivePoints;

    TowerAttackingEnemy towerAttacker;
    PlayerAttackingEnemy playerAttacker;
    Dictionary<BaseEnemy, float> lastDamageTimes = new Dictionary<BaseEnemy, float>();

    private void Start()
    {
        gameManager.instance.defensiveScore += defensivePoints;
        gameManager.instance.updateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
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

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<TowerAttackingEnemy>(out TowerAttackingEnemy towerAttackScript))
        {
            towerAttacker = towerAttackScript;
            DealDamageOverTime(towerAttackScript);
        }
        else if (other.TryGetComponent<PlayerAttackingEnemy>(out PlayerAttackingEnemy playerAttackScript))
        {
            playerAttacker = playerAttackScript;
            DealDamageOverTime(playerAttackScript);
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

    void DealDamageOverTime(BaseEnemy enemy)
    {
        if (!lastDamageTimes.ContainsKey(enemy) || Time.time - lastDamageTimes[enemy] >= damageInterval)
        {
            enemy.takeDamage(drainDmg);
            lastDamageTimes[enemy] = Time.time;
        }
    }
}
