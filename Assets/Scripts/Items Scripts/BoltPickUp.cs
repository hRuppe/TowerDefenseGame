using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltPickUp : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tag == "Bolt")
        {
            gameManager.instance.playerScript.playerBolts += 5;
            Destroy(gameObject);
        }
    }
}
