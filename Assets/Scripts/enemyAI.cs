using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float attackRange = 2f;

    EnemyState currentState;
    GameObject locationToAttack;
    BoxCollider locationCollider;
    Vector3 positionToAttack; // Stores random position within the locationCollider to attack
    Transform randomPositionOnLocation; 
    public bool inAttackRange = false; 
    // This value is the speed value that looks best with a "1" value on the enemy run animation (shouldn't need adjustment, which is why it's hardcoded)
    float speedToAnimationDefault = 4.75f;

    public enum EnemyState
    {
        MovingToLocation,
        AttackingLocation,
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find location that the enemy is attacking
        if (GameObject.FindWithTag("Location To Defend") != null)
        {
            locationToAttack = GameObject.FindWithTag("Location To Defend");
            locationCollider = locationToAttack.GetComponent<BoxCollider>();
            GetRandomPositionInCollider();
        }
            

        // Set enemy speed in the navmesh 
        agent.speed = speed;

        // Adjust running animation speed with the enemy speed
        float scalingFactor = 1f / (speedToAnimationDefault / speed);
        anim.SetFloat("Running Speed Animation Multiplier", scalingFactor);

        // Start enemy in moving state
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
        // Check if enemy is within range
        if (inAttackRange)
        {
            ChangeState(EnemyState.AttackingLocation);
        }
        else
        {
            if (agent.destination != positionToAttack)
            {
                agent.SetDestination(positionToAttack);
            }
        }
    }

    void AttackTower()
    {
        anim.SetBool("Attack", true);
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    void GetRandomPositionInCollider()
    {
        Vector3 min = locationCollider.bounds.min;
        Vector3 max = locationCollider.bounds.max;

        float randomX = Random.Range(min.x, max.x);
        float randomZ = Random.Range(min.z, max.z);

        // keep y-coordinate same as enemy's current position
        float y = transform.position.y;

        positionToAttack = new Vector3(randomX, y, randomZ);
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
