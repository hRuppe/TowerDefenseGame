using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingEnemy : BaseEnemy
{
    float distanceToPlayer; 
    GameObject player; 
    Vector3 currentPlayerPos; 

    protected override void Start()
    {
        // Do everything in start from BaseEnemy class
        base.Start();

        // Get reference to player
        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
        }

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
        // Check if player is still within attack range
        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);
        if (distanceToPlayer > attackRange)
        {
            // Player out of range, change states
            ChangeState(EnemyState.MovingToPlayer);
        }
        else
        {
            // Perform attack logic
        }
    }

    void UpdatePlayerPos()
    {
        currentPlayerPos = player.transform.position;
    }
}
