using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingEnemy : BaseEnemy
{
    float distanceToPlayer; 
    GameObject player; 
    Vector3 currentPlayerPos;
    SpearDamage spearDamageScript;

    protected override void Start()
    {
        // Do everything in start from BaseEnemy class
        base.Start();

        // Get reference to player
        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Find/assign spear collider
        spearDamageScript = GetComponentInChildren<SpearDamage>();

        ChangeState(EnemyState.MovingToPlayer);
    }

    void Update()
    {
        UpdatePlayerPos(); 

        switch (currentState)
        {
            case EnemyState.MovingToPlayer:
                MoveEnemyTowardPlayer(); 
                break;
            case EnemyState.AttackingPlayer:
                AttackPlayer();
                break;
        }
    }

    void MoveEnemyTowardPlayer()
    {
        // Get player position & set enemy destination
        agent.SetDestination(currentPlayerPos);
        
        if (agent.remainingDistance > 0f && agent.remainingDistance <= attackRange)
        {
            ChangeState(EnemyState.AttackingPlayer);
        }
    }

    void AttackPlayer()
    {
        // If player gets out of attack range chnage state back to MovingToPlayer
        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);
        if (distanceToPlayer > attackRange)
        {
            // Set all attack bools to false
            foreach (string attackName in attackAnimationNames)
            {
                anim.SetBool(attackName, false);
            }
            ChangeState(EnemyState.MovingToPlayer);
        }
        else
        {
            // Look at the player while attacking
            transform.LookAt(player.transform.position);

            // Randomly choose an attack animation
            int randomIndex = Random.Range(0, attackAnimationNames.Length);

            // Set all attack bools to false so only one animation plays at a time
            foreach (string attackName in attackAnimationNames)
            {
                anim.SetBool(attackName, false);
            }
            // Start random attack animation
            anim.SetBool(attackAnimationNames[randomIndex], true);
        }
    }


    public void TryDamagePlayer()
    {
        if (spearDamageScript.GetSpearContactedPlayer())
        {
            gameManager.instance.playerScript.playerHealth -= attackDmg;
            StartCoroutine(gameManager.instance.playerDamageFlash()); 
        }

        spearDamageScript.ResetSpearContactedPlayer();
    }

    void UpdatePlayerPos()
    {
        currentPlayerPos = player.transform.position;
    }
}
