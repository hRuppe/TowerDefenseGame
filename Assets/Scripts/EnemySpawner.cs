using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefabToSpawn; // Enemy prefab to spawn
    [SerializeField] int numberOfEnemiesToSpawn; // Num of enemies to spawn
    [SerializeField] float spawnTimeInterval; // Amount of tiime in between spawns

    private int numberOfEnemiesSpawned = 0; // Num of enemies that have been spawned

    // Gets the 'Spawn()' coroutine started 
    private void Start()
    {
        StartCoroutine(Spawn()); 
    }

    // Instantiates a new enemy at the location of the enemy spawner object & increases the # of enemies spawned counter
    void CreateEnemy()
    {
        Instantiate(enemyPrefabToSpawn, gameObject.transform.position, enemyPrefabToSpawn.transform.rotation);
        numberOfEnemiesSpawned++; 
    }

    // This function will spawn enemies while the # of enemies spawned is less than the amount of enemies that needs to spawn
    // Ienumerator allows the "yield return" so we can wait a certain amount of seconds in between spawns (interval is adjustable in unity editor)
    IEnumerator Spawn()
    {
        while (numberOfEnemiesSpawned < numberOfEnemiesToSpawn)
        {
            yield return new WaitForSeconds(spawnTimeInterval);
            CreateEnemy();
        }
    }
}
