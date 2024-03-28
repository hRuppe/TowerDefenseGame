using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltPickUp : MonoBehaviour
{
    [Header("---- Pickup Movement ----")]
    [SerializeField] float moveIncrement;
    [SerializeField] float rotationSpeed;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tag == "Bolt")
        {
            gameManager.instance.playerScript.playerBolts += this.GetComponent<ItemPickup>().itemStats.boltCurrency;
            Destroy(gameObject);
        }
    }

    float rotation = 180;
    private bool isRotating;

    void Update()
    {
        if (!isRotating)
            RotatePickup();
    }

    void RotatePickup()
    {
        isRotating = true;
        transform.Rotate(rotation * Time.deltaTime * rotationSpeed,0 , 0);
        isRotating = false;
    }
}
