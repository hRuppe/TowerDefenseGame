using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currencyPickup : MonoBehaviour
{
    [SerializeField] int currencyAmount;
    
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.playerCurrency += currencyAmount;
            Destroy(gameObject);
        }
    }
}
