using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TowerAttackingEnemy : BaseEnemy
{
    protected override void Start()
    {
        // Do everything in start from BaseEnemy class
        base.Start();

        // Starts enemy in moving state
        ChangeState(EnemyState.MovingToLocation); 
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.MovingToLocation:
                MoveToLocation();
                break;
            case EnemyState.AttackingLocation:
                AttackTower();
                break;
        }
    }

    void MoveToLocation()
    {
        // If not already set, sets the destination for the enemy (should only enter this the 1st time it enters MoveToLocation)
        if (agent.destination != positionToAttack)
        {
            SetPositionToAttack();
            agent.SetDestination(positionToAttack);
        }

        // Check if the current position is within attack range
        if (Vector3.Distance(transform.position, positionToAttack) <= attackRange)
        {
            inAttackRange = true;
        }

        // Check if enemy is within range
        if (inAttackRange)
        {
            ChangeState(EnemyState.AttackingLocation);
        }
    }

    // Sets positionToAttack variable to random point within the attacking location
    public void SetPositionToAttack()
    {
        BoxCollider collider = locationCollider.GetComponent<BoxCollider>();

        Vector3 localMin = collider.center - collider.size / 2;
        Vector3 localMax = collider.center + collider.size / 2;

        while (!locationCollider.bounds.Contains(positionToAttack))
        {
            float randomX = Random.Range(localMin.x, localMax.x);
            float randomY = Random.Range(localMin.y, localMax.y);
            float randomZ = Random.Range(localMin.z, localMax.z);

            positionToAttack = locationCollider.transform.TransformPoint(new Vector3(randomX, randomY, randomZ));
        }
    }

    // Stops the enemy from moving & starts his attack animation loop
    void AttackTower()
    {
        // Stops enemy from moving
        agent.isStopped = true;
        enemyRB.freezeRotation = true; 

        // Starts attack animation
        anim.SetBool("Attack", true);
    }
}
