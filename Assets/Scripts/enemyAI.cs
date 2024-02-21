using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----  Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    public GameObject damageZone;


    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;
    [SerializeField] float speed; 

    GameObject objectToAttack;
    // This value is the speed value that looks best with a "1" value on the enemy run animation (shouldn't need adjustment)
    float speedToAnimationDefault = 4.75f; 

    // Start is called before the first frame update
    void Start()
    {
        // Find location that the enemy is attacking
        if (GameObject.FindWithTag("Location To Defend") != null)
            objectToAttack = GameObject.FindWithTag("Location To Defend");

        // Set enemy speed in the navmesh 
        agent.speed = speed;

        // Adjust running animation speed with the enemy speed
        float scalingFactor = 1f / (speedToAnimationDefault / speed);
        anim.SetFloat("Running Speed Animation Multiplier", scalingFactor);

        // Increments enemies to kill when the enemy spawns so you have a count of all active enemies
        //gameManager.instance.enemiesToKill++;
        // Updates enemies to kill UI
        //gameManager.instance.updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

        agent.SetDestination(objectToAttack.transform.position);
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
