using UnityEngine;

public class TowerAttackingEnemy : BaseEnemy
{
    protected override void Start()
    {
        // Do everything in start from BaseEnemy class
        base.Start();

        // Starts enemy in moving state
        ChangeState(EnemyState.MovingToLocation);
    }

    protected override void Update()
    {
        base.Update();

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

    public void MoveToLocation()
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

    // Stops the enemy from moving & starts his attack animation loop
    void AttackTower()
    {
        // Stops enemy from moving
        StopAgentMovement();
        enemyRB.freezeRotation = true;

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
