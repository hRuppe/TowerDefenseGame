using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossEnemy : BaseEnemy
{
    float distanceToPlayer;
    GameObject player;
    Vector3 currentPlayerPos;
    SpearDamage spearDamageScript;
    [Header("---- Player Finding Settings ----")]
    [SerializeField] float sightAngle = 45f; // Field of view angle
    [SerializeField] float viewDistance = 10f; // Maximum distance for line of sight


    protected override void Start()
    {
        base.Start();

        // Get reference to player
        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Assign spear collider
        spearDamageScript = GetComponentInChildren<SpearDamage>();

        // Starts enemy in the move to location state
        ChangeState(EnemyState.MovingToLocation);
    }

    protected override void Update()
    {
        base.Update();

        UpdatePlayerPos();

        // Controls what each state has the enemy do
        switch (currentState)
        {
            case EnemyState.MovingToLocation:
                LookForPlayerWhileMovingToLocation();
                break;
            case EnemyState.AttackingLocation:
                AttackTower(); 
                break; 
            case EnemyState.MovingToPlayer:
                MoveEnemyTowardPlayer();
                break;
            case EnemyState.AttackingPlayer:
                AttackPlayer();
                break;
        }
    }

    public void LookForPlayerWhileMovingToLocation()
    {
        // Check if it can see the player first
        if (HasLineOfSight())
        {
            Debug.Log("has line of sight"); 
            ChangeState(EnemyState.MovingToPlayer); 
        }

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

        // Change state if enemy is within location attack range
        if (inAttackRange)
        {
            ChangeState(EnemyState.AttackingLocation);
        }
    }

    void MoveEnemyTowardPlayer()
    {
        // Get player position & set enemy destination
        UpdatePlayerPos();
        agent.SetDestination(currentPlayerPos); 

        if (agent.remainingDistance > 0f && agent.remainingDistance <= attackRange)
        {
            ChangeState(EnemyState.AttackingPlayer);
        }
        else if (!HasLineOfSight())
        {
            anim.SetBool("Look Around", true); 
            Debug.Log("lost line of sight"); 
            ChangeState(EnemyState.MovingToLocation);
        }
    }

    bool HasLineOfSight()
    {
        UpdatePlayerPos();

        Vector3 playerDir = (currentPlayerPos - transform.position).normalized;
        Vector3 enemyForward = transform.forward;
        float dotProduct = Vector3.Dot(playerDir, enemyForward);
        float angleToPlayer = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);

        if (distanceToPlayer <= viewDistance && angleToPlayer <= sightAngle)
        {
            // Perform raycast check here
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        // Update player direction
        Vector3 playerDir = (currentPlayerPos - transform.position).normalized;
        float angleToPlayer = Mathf.Acos(Vector3.Dot(transform.forward, playerDir)) * Mathf.Rad2Deg;

        // Check if player is within view distance and angle
        if (distanceToPlayer <= viewDistance && angleToPlayer <= sightAngle)
        {
            Gizmos.color = Color.yellow; // Player is in sight
        }
        else
        {
            Gizmos.color = Color.white; // Player is not in sight
        }

        // Draw vision cone
        float halfSightAngle = sightAngle / 2f;
        Vector3 leftRayDirection = Quaternion.Euler(0f, -halfSightAngle, 0f) * transform.forward;
        Vector3 rightRayDirection = Quaternion.Euler(0f, halfSightAngle, 0f) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * viewDistance);
        Gizmos.DrawLine(transform.position + leftRayDirection * viewDistance, transform.position + rightRayDirection * viewDistance);
    }


    void AttackPlayer()
    {
        UpdatePlayerPos(); 
        // If player gets out of attack range change state back to MovingToPlayer 
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

    // Stops the enemy from moving & starts his attack animation loop
    void AttackTower()
    {
        if (HasLineOfSight())
        { 
            // Set all attack bools to false
            foreach (string attackName in attackAnimationNames)
            {
                anim.SetBool(attackName, false);
            }

            UnfreezeRBRotation();
            transform.LookAt(player.transform); 
            ResumeAgentMovement();
            
            ChangeState(EnemyState.MovingToPlayer);
        }

        // Stops enemy from moving
        StopAgentMovement();
        FreezeRBRotation(); 

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

    void FreezeRBRotation()
    {
        enemyRB.freezeRotation = true; 
    }

    void UnfreezeRBRotation()
    {
        enemyRB.freezeRotation = false; 
    }
}
