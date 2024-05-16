using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class BaseEnemy : MonoBehaviour, IDamage
{
    // Components that need to be assgined in the editor
    [Header("---- General Components ----")]
    [SerializeField] protected Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected GameObject[] itemsToDrop;
    [SerializeField] protected int numOfItemsToDrop;
    [SerializeField] GameObject itemSpawnPoint;

    [Header("---- Audio Components ----")]
    [SerializeField] protected AudioClip[] enemySFX;
    [SerializeField] protected AudioClip[] footstepSFX;
    [SerializeField] protected AudioClip[] whooshSFX;
    [SerializeField] protected AudioClip thudSFX;


    // Stats that may need adjustment in the editor
    [Header("---- Enemy Stats ----")]
    [SerializeField] protected int HP;
    [SerializeField] protected float speed;
    [SerializeField] protected int attackDmg;
    [SerializeField] protected float attackRange;

    // Settings for SFX
    [Header("---- SFX Settings ----")]
    [SerializeField] protected float minTimeBetweenSounds = 2f;
    [SerializeField] protected float maxTimeBetweenSounds = 5f;

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
    protected float originalSpeed;
    protected AudioSource audioSource;
    protected AudioSource weaponAudioSource;
    protected float nextSoundTime;
    float upwardForceMagnitude = 6f;
    float origSpeed;

    int expGained = 11;

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

        // Save original speed
        originalSpeed = speed;

        // Set audio sources
        audioSource = GetComponent<AudioSource>();
        weaponAudioSource = GetComponentInChildren<AudioSource>();

        // Sets the initial time for the next sound
        nextSoundTime = Time.time + Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
    }

    protected virtual void Update()
    {
        // Keeps health bar facing player at all times
        healthBar.transform.LookAt(gameManager.instance.player.transform);

        // Check if it's time to play another sound & play it, then get another sound time
        if (Time.time >= nextSoundTime)
        {
            PlayRandomSound();
            nextSoundTime = Time.time + Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
        }
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
            gameManager.instance.playerScript.GainExperience(expGained); // Add experience points
            DropItems(); // Drop bolts / currency
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

    public void ChangeEnemySpeed(float speedScaler)
    {
        speed *= speedScaler;
        agent.speed = speed;
    }

    public void ResetSpeed()
    {
        speed = originalSpeed;
        agent.speed = speed;
    }

    public void PlayRandomSound()
    {
        if (enemySFX.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to enemySFX array.");
            return;
        }

        int randomIndex = Random.Range(0, enemySFX.Length);
        AudioClip randomClip = enemySFX[randomIndex];

        if (!audioSource.isPlaying && randomClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(randomClip);
        }
    }

    public void PlayRandomFootstepSFX()
    {
        if (footstepSFX.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to footstepSFX array.");
            return;
        }

        int randomIndex = Random.Range(0, footstepSFX.Length);
        AudioClip randomClip = footstepSFX[randomIndex];

        if (randomClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(randomClip);
        }
    }

    public void PlayWeaponThudSFX()
    {
        weaponAudioSource.PlayOneShot(thudSFX);
    }

    public void PlayWeaponWhooshSFX()
    {
        if (footstepSFX.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to whooshSFX array.");
            return;
        }

        int randomIndex = Random.Range(0, whooshSFX.Length);
        AudioClip randomClip = whooshSFX[randomIndex];

        if (randomClip != null && audioSource != null)
        {
            weaponAudioSource.PlayOneShot(randomClip);
        }
    }

    public void DropItems()
    {

        for (int i = 0; i < numOfItemsToDrop; i++)
        {
            // If there's multiple items in the items to drop array then drop a random item
            GameObject randDrop;
            if (itemsToDrop.Length > 1)
            {
                int randIndex = Random.Range(0, itemsToDrop.Length);
                randDrop = itemsToDrop[randIndex];
                GameObject droppedItem = Instantiate(randDrop, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);

                // Get rigidbody of dropped item
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

                //// Apply force to rigidbody
                //if (rb != null)
                //{
                //    Vector3 upwardForce = Vector3.up * upwardForceMagnitude;

                //    rb.AddForce(upwardForce, ForceMode.Impulse);
                //}

            }
            else
            {
                GameObject droppedItem = Instantiate(itemsToDrop[0], itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);

                // Get rigidbody of dropped item
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

                // Apply force to rigidbody
                //if (rb != null)
                //{
                //    Vector3 upwardForce = Vector3.up * upwardForceMagnitude;

                //    rb.AddForce(upwardForce, ForceMode.Impulse);
                //}
            }
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
}
