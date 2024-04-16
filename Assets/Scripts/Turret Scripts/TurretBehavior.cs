using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TurretBehavior : MonoBehaviour
{
    [Header("---- Turret behavior ----")]
    [SerializeField] int faceEnemySpeed;
    [SerializeField] float shootRate;
    [SerializeField] int shootDmg;
    [SerializeField] int towerUpgrade;
    [SerializeField] int defensivePoints;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    private Vector3 direction;

    Transform enemy;

    private void Start()
    {
        gameManager.instance.defensiveScore += defensivePoints;
        gameManager.instance.updateUI(); 
    }

    void Update()
    {
        Debug.Log(gameManager.instance.player.transform.position.normalized - transform.position.normalized);

        if (enemy != null)
        {
            // Rotate turret towards the enemy
            direction = enemy.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * faceEnemySpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            // Fire at the enemy
        }

        //if(gameManager.instance.player.transform.position.x - transform.position.x <= 4 && gameManager.instance.player.transform.position.z - transform.position.z <= 4)
        //{
        //    if(CompareTag("LevelOneTurret"))
        //    {
        //        if(gameManager.instance.playerScript.playerBolts < gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice)
        //        {
        //            gameManager.instance.upgradeTurretPrompt.text = gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice + " bolts to upgrade Turret";
        //        }
        //        else
        //        {
        //            gameManager.instance.upgradeTurretPrompt.text = "Press E to Upgrade Turret";
        //        }
        //        gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(true);

        //        if (Input.GetButtonDown("PlaceItem") && gameManager.instance.playerScript.playerBolts > gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice)
        //        {
        //            Instantiate(gameManager.instance.level2Turret, transform.position, transform.rotation);
        //            Destroy(gameObject);
        //        }
        //    }
        //    else if(CompareTag("LevelTwoTurret"))
        //    {
        //        if (gameManager.instance.playerScript.playerBolts < gameManager.instance.rocketTurretPrice - gameManager.instance.level2TurretPrice)
        //        {
        //            gameManager.instance.upgradeTurretPrompt.text = gameManager.instance.rocketTurretPrice - gameManager.instance.level2TurretPrice + " bolts to upgrade Turret";
        //        }
        //        else
        //        {
        //            gameManager.instance.upgradeTurretPrompt.text = "Press E to Upgrade Turret";
        //        }
        //        gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(true);

        //        if (Input.GetButtonDown("PlaceItem") && gameManager.instance.playerScript.playerBolts > gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice)
        //        {
        //            Instantiate(gameManager.instance.rocketTurret, transform.position, transform.rotation);
        //            Destroy(gameObject);
        //        }
        //    }
        //}
        //else
        //{
        //    gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(false);
        //}
    }

    IEnumerator Fire()
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(shootRate);
            // Instantiate bullet prefab at firePoint position and rotation
            bulletPrefab.GetComponent<BulletDamage>().shootDirection = direction;
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemy = other.GetComponent<Transform>();
            StartCoroutine(Fire());
        }

        Debug.Log(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemy = null;
        }
    }
}

