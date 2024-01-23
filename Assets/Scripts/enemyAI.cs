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
    [SerializeField] GameObject endLoc;


    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int animLerpSpeed;

    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemiesToKill++;
        gameManager.instance.updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        faceLocation();

        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

        agent.SetDestination(endLoc.transform.position);
    }

    void faceLocation()
    {
        playerDir.y = 0;

        Quaternion rotation = Quaternion.LookRotation(endLoc.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
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
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
