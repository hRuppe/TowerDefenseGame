using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class BaseEnemy : MonoBehaviour, IDamage
{
    // Components that need to be assgined in the editor
    [Header("----  Components ----")]
    [SerializeField] protected Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Slider healthBar; 

    // Stats that may need adjustment in the editor
    [Header("---- Enemy Stats ----")]
    [SerializeField] protected int HP;
    [SerializeField] protected float speed;
    [SerializeField] protected int attackDmg;
    [SerializeField] protected float attackRange;
    
    // Variables that don't need to be shown in the inspector
    protected Rigidbody enemyRB;
    protected EnemyState currentState; // The current state of this enemy (moving, attacking, etc)
    protected GameObject locationToAttack; // Gameobject that the enemy is attacking
    protected BoxCollider locationCollider; // Collider on the gameobject the enemy is attacking
    protected LocationToDefend locationScript; // Script attached to the location to access needed functions
    protected Vector3 positionToAttack; // Stores random position within the locationCollider to attack
    protected bool inAttackRange = false; // Start enemy out of attack range so it can be triggered later
    protected float speedToAnimationDefault = 4.75f; // This value is the speed value that looks best with a "1" value on the enemy run animation (shouldn't need adjustment, which is why it's hardcoded)                                                // Array of attacks to randomly choose from
    protected string[] attackAnimationNames = { "Attack1", "Attack2" };

    // Enum that holds all states the enemy could be in
    public enum EnemyState
    {
        MovingToLocation,
        AttackingLocation,
        MovingToPlayer,
        AttackingPlayer
    }

    protected virtual void Start()
    {
        // Add to enemy count & update UI
        gameManager.instance.enemiesToKill++;
        gameManager.instance.updateUI();

        // Looks for location the enemy will attack (based on tag) & assigns variable if it's found
        if (GameObject.FindWithTag("Location To Defend") != null)
        {
            locationToAttack = GameObject.FindWithTag("Location To Defend");
            locationCollider = locationToAttack.GetComponent<BoxCollider>();
            locationScript = locationToAttack.GetComponent<LocationToDefend>();
        }

        // Set rigidbody reference
        enemyRB = GetComponent<Rigidbody>();

        // Sets the enemy nav mesh speed to what it's set to in this script
        agent.speed = speed;

        // Adjust running animation speed with the enemy speed
        float scalingFactor = 1f / (speedToAnimationDefault / speed);
        anim.SetFloat("Running Speed Animation Multiplier", scalingFactor);

        // Set max healthbar value to enemies max health
        healthBar.maxValue = HP;
    }

    protected virtual void Update()
    {
        healthBar.transform.LookAt(gameManager.instance.player.transform); 
    }

    // Used to change the enemy state
    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    // Function is called by the enemy attack animation
    public void DamageLocation()
    {
        locationScript.TakeDamage(attackDmg);
    }

    // Function for enemy to take damage
    public void takeDamage(int dmg)
    {

        HP -= dmg;

        UpdateEnemyHealthBar(HP); 

        // Enemy flashes red to show it was hit
        StartCoroutine(flashDamage());

        // Enemy death scenario
        if (HP <= 0)
        {
            gameManager.instance.updateEnemyNumber();
            Destroy(gameObject);
        }
    }

    // Causes enemy to flash red when takes damage
    public IEnumerator flashDamage()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        model.material.color = Color.white;
    }

    public void StopAgentMovement()
    {
        agent.isStopped = true; 
    }

    public void ResumeAgentMovement()
    {
        agent.isStopped = false;
    }

    public void UpdateEnemyHealthBar(float currentHealth)
    {
        healthBar.value = currentHealth;
    }
}
