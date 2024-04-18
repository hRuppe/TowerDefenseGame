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
    [SerializeField] Transform turretGun;
    public List<Transform> enemyList = new List<Transform>();
    private Vector3 direction;

    Transform enemy;

    private void Start()
    {
        gameManager.instance.defensiveScore += defensivePoints;
        gameManager.instance.updateUI();
    }

    void Update()
    {
        if (enemyList.Count > 0 && enemyList[0].IsDestroyed())
        {
            enemyList.RemoveAt(0);
            enemy = enemyList[0];
        }

        if (enemy != null)
        {
            // Rotate turret towards the enemy
            direction = enemy.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * faceEnemySpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            turretGun.rotation = Quaternion.Euler(Mathf.Abs(enemy.position.x), rotation.y, 0f);
            // Fire at the enemy
        }
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
            enemyList.Add(other.GetComponent<Transform>());
            enemy = enemyList[0];
            StartCoroutine(Fire());
        }

        Debug.Log(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.transform == enemyList[0])
            {
                enemyList.RemoveAt(0);
            }
        }
    }
}

