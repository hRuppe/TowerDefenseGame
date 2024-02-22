using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----  Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;


    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int attackDmg; 
    [SerializeField] float attackRange;
    [SerializeField] float rotationSpeed;
    

    EnemyState currentState; // The current state of this enemy (moving, attacking, etc)
    GameObject locationToAttack; // Gameobject that the enemy is attacking
    BoxCollider locationCollider; // Collider on the gameobject the enemy is attacking
    LocationToDefend locationScript; // Script attached to the location to access needed functions
    Vector3 positionToAttack; // Stores random position within the locationCollider to attack
    public bool inAttackRange = false; // Start enemy out of attack range so it can be triggered later
    float speedToAnimationDefault = 4.75f; // This value is the speed value that looks best with a "1" value on the enemy run animation (shouldn't need adjustment, which is why it's hardcoded)



    public enum EnemyState
    {
        MovingToLocation,
        AttackingLocation,
    }

    
    void Start()
    {
        // Finds gameobject that the enemy is attacking
        if (GameObject.FindWithTag("Location To Defend") != null)
        {
            locationToAttack = GameObject.FindWithTag("Location To Defend");
            locationCollider = locationToAttack.GetComponent<BoxCollider>();
            locationScript = locationToAttack.GetComponent<LocationToDefend>();
            SetPositionToAttack();
        }
            

        // Sets the enemy nav mesh speed to what it's set to in this script
        agent.speed = speed;

        // Adjust running animation speed with the enemy speed
        float scalingFactor = 1f / (speedToAnimationDefault / speed);
        anim.SetFloat("Running Speed Animation Multiplier", scalingFactor);

        // Starts enemy in moving state
        currentState = EnemyState.MovingToLocation; 
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

    void AttackTower()
    {
        // Stops enemy from moving
        agent.isStopped = true;

        anim.SetBool("Attack", true);
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    void SetPositionToAttack()
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

    // Function is called by the enemy attack animation
    public void AttackLocation()
    {
        locationScript.TakeDamage(attackDmg); 
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.updateEnemyNumber();
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        model.material.color = Color.white;
    } 
}
