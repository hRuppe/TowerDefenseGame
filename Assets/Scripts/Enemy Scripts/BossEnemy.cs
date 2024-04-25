using System.Collections;
using System.Collections.Generic;
using System.Net;
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

        // Controls what each state has the enemy do
        switch (currentState)
        {
            case EnemyState.MovingToLocation:
                LookForPlayerWhileMovingToLocation();
                break;
            case EnemyState.AttackingLocation:

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
            ChangeState(EnemyState.MovingToLocation);
        }
    }

    bool HasLineOfSight()
    {
        UpdatePlayerPos(); 

        Vector3 playerDir = (currentPlayerPos - transform.position);
        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);
        float angleToPlayer = Vector3.Angle(transform.forward, playerDir);
        RaycastHit hit;

        Debug.Log("Distance to player: " + distanceToPlayer);
        Debug.Log("Angle to player: " + angleToPlayer);

        // Check view distance & sight angle
        if (distanceToPlayer <= viewDistance && angleToPlayer <= sightAngle)
        {
            // Shoot raycast to make sure objects are not between
            if (Physics.Raycast(transform.position, playerDir, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<playerController>(out playerController pc))
                {
                    Debug.Log("Enemy has line of sight");
                    return true;
                }
            }
        }
        
        return false;
    }

    void OnDrawGizmos()
    {
        Vector3 playerDir = (currentPlayerPos - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, playerDir);
  
        RaycastHit hit;

        // Check view distance & sight angle
        if (distanceToPlayer <= viewDistance && angleToPlayer <= sightAngle)
        {
            // Shoot raycast to make sure objects are not between
            if (Physics.Raycast(transform.position, playerDir, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<playerController>(out playerController pc))
                {
                    Gizmos.color = Color.yellow;
                }
            }
        }
        else
        {
            Gizmos.color = Color.white;
        }

        // Draws 2 rays showing the fov of the enemy
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, -sightAngle / 2f, 0f) * transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, sightAngle / 2f, 0f) * transform.forward * viewDistance);

        // Draws lines to connect rays together
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, -sightAngle / 2f, 0f) * transform.forward * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, sightAngle / 2f, 0f) * transform.forward * viewDistance);
        Gizmos.DrawLine(transform.position + Quaternion.Euler(0f, -sightAngle / 2f, 0f) * transform.forward * viewDistance,
                        transform.position + Quaternion.Euler(0f, sightAngle / 2f, 0f) * transform.forward * viewDistance);

    }


    void AttackPlayer()
    {
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
        // Implementation remains the same
    }

    void UpdatePlayerPos()
    {
        currentPlayerPos = player.transform.position;
    }
}
