using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossEnemy : BaseEnemy
{
    float distanceToPlayer;
    GameObject player;
    Vector3 currentPlayerPos;
    SpearDamage spearDamageScript;
    bool playerInPoisonAOE;
    float timeSinceLostLOS = 0f;

    [Header("---- Poison Attack Settings ----")]
    [SerializeField] int poisonDamage;
    [SerializeField] float poisonRange;

    [Header("---- Poison Attack Components ----")]
    [SerializeField] ParticleSystem poisonVFX; 
    [SerializeField] Transform PoisonSpawnPos;
    [SerializeField] AudioClip poisonSFX;

    [Header("---- Battlecry Attack Components ----")]
    [SerializeField] Transform leftSpawnPos;
    [SerializeField] Transform rightSpawnPos;
    [SerializeField] GameObject allyToSpawn;
    [SerializeField] ParticleSystem spawnVFX;
    [SerializeField] AudioClip spawnSFX; 

    [Header("---- Enemy Sight Settings ----")]
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
        // Run all base enemy's update code
        base.Update();

        // Update player posiiton
        UpdatePlayerPos();

        TrackTimeSinceEnemyHasSeenPlayer();
        
        // Check if player in poison range
        CheckIfPlayerIsInPoisonRange();

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

    private void TrackTimeSinceEnemyHasSeenPlayer()
    {
        if (HasLineOfSight())
        {
            timeSinceLostLOS = 0;
        }
        else
        {
            timeSinceLostLOS += Time.deltaTime;
        }
    }

    public void LookForPlayerWhileMovingToLocation()
    {
        // Check if it can see the player first
        if (HasLineOfSight())
        {
            Debug.Log("has line of sight");
            inAttackRange = false; 
            SetAllAttackBoolsFalse();
            ChangeState(EnemyState.MovingToPlayer); 
        }

        // If not already set, sets the position for the enemy to run to (should only enter this the 1st time it enters MoveToLocation)
        if (agent.destination != positionToAttack)
        {
            SetPositionToAttack();
        }

        agent.SetDestination(positionToAttack);

        // Check if the current position is within attack range
        if (Vector3.Distance(transform.position, positionToAttack) <= attackRange)
        {
            inAttackRange = true;
        }

        // Change state if enemy is within location attack range
        if (inAttackRange)
        {
            inAttackRange = false; 
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
        else if (!HasLineOfSight() && timeSinceLostLOS > 3.5f)
        {
            Debug.Log("lost line of sight");

            // Random 20% chance that it triggers the battle cry attack instead of look around animation
            float randomValue = Random.value;
            if (randomValue < 0.2f)
            {
                anim.SetTrigger("BattleCryAttack");
            }
            else
            {
                anim.SetTrigger("Look Around");
            }

            ChangeState(EnemyState.MovingToLocation);
        }
    }

    bool HasLineOfSight()
    {
        UpdatePlayerPos();

        Vector3 playerDir = (currentPlayerPos - new Vector3(0, 1.6f, 0) - transform.position).normalized;
        Vector3 enemyForward = transform.forward;
        float dotProduct = Vector3.Dot(playerDir, enemyForward);
        float angleToPlayer = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);

        if (distanceToPlayer <= viewDistance && angleToPlayer <= sightAngle)
        {
            // Layer mask to ignore the enemy's collider
            int layerMask = ~(1 << gameObject.layer);

            // Adjusts raycast to match enemy eye level
            Vector3 raycastOrigin = transform.position + Vector3.up * 1.8f;

            // Raycast to check for objects between enemy & player
            RaycastHit hit;

            Debug.DrawRay(raycastOrigin, playerDir * distanceToPlayer, Color.yellow);

            if (Physics.Raycast(raycastOrigin, playerDir, out hit, viewDistance, layerMask))
            {
                // Returns false if raycast hits something other than player
                if (hit.collider.gameObject != player)
                {
                    return false;
                }
            }

            timeSinceLostLOS = 0; 
            return true;
        }

        return false;
    }


    void AttackPlayer()
    {
        UpdatePlayerPos(); 
        // If player gets out of attack range change state back to MovingToPlayer 
        distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);

        if (distanceToPlayer > attackRange)
        {
            // Set all attack bools to false
            SetAllAttackBoolsFalse(); 

            ChangeState(EnemyState.MovingToPlayer);
        }
        else
        {
            // Look at the player while attacking
            transform.LookAt(player.transform.position);

            // Randomly choose an attack animation
            int randomIndex = Random.Range(0, attackAnimationNames.Length);

            // Set all attack bools to false so only one animation plays at a time
            SetAllAttackBoolsFalse();

            // Start random attack animation
            float randomValue = Random.value;
            if (randomValue < 0.05f)
            {
                anim.SetTrigger("BattleCryAttack");
            }
            else
            {
                anim.SetBool(attackAnimationNames[randomIndex], true);
            }
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
            SetAllAttackBoolsFalse(); 

            UnfreezeRBRotation();
            ResumeAgentMovement();
            
            ChangeState(EnemyState.MovingToPlayer);
        }
        else
        {
            // Stops enemy from moving
            StopAgentMovement();
            FreezeRBRotation();

            // Randomly choose an attack animation
            int randomIndex = Random.Range(0, attackAnimationNames.Length);

            // Set all attack bools to false so only one animation plays at a time
            SetAllAttackBoolsFalse(); 

            // Start random attack animation
            anim.SetBool(attackAnimationNames[randomIndex], true);
        }
    }

    void FreezeRBRotation()
    {
        enemyRB.freezeRotation = true;
    }

    void UnfreezeRBRotation()
    {
        enemyRB.freezeRotation = false;
    }

    void SetAllAttackBoolsFalse()
    {
        // Set all attack bools to false
        foreach (string attackName in attackAnimationNames)
        {
            anim.SetBool(attackName, false);
        }
    }

    // Called in kick animation
    void SpawnPoison()
    {
        // Play sfx 
        weaponAudioSource.PlayOneShot(poisonSFX); 

        // Spawn poison
        ParticleSystem poisonInstance = Instantiate(poisonVFX, PoisonSpawnPos.position, transform.rotation);

        // Try to damage player
        TryDamagePlayerInAOE(); 

        // Destroy
        Destroy(poisonInstance.gameObject, .55f);
    }

    public void TryDamagePlayerInAOE()
    {
        if (playerInPoisonAOE)
        {
            gameManager.instance.playerScript.playerHealth -= attackDmg;
            StartCoroutine(gameManager.instance.playerDamageFlash());
        }
    }

    void CheckIfPlayerIsInPoisonRange()
    {
        if (distanceToPlayer <= poisonRange)
        {
            playerInPoisonAOE = true;
        }
        else
        {
            playerInPoisonAOE = false;
        }
    }

    // Called in battlecry animation
    void BattleCryAttack()
    {
        PlayRandomSound();

        audioSource.PlayOneShot(spawnSFX);
        ParticleSystem leftVFX = Instantiate(spawnVFX, leftSpawnPos.position, transform.rotation);
        ParticleSystem rightVFX = Instantiate(spawnVFX, rightSpawnPos.position, transform.rotation);
        Instantiate(allyToSpawn, leftSpawnPos.position, transform.rotation);
        Instantiate(allyToSpawn, rightSpawnPos.position, transform.rotation);

        Destroy(leftVFX.gameObject, 2); 
        Destroy(rightVFX.gameObject, 2);
    }
}
