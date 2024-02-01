using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    int shootDmg = 2;
    public Vector3 shootDirection;

    private void Update()
    {
        GetComponent<Rigidbody>().AddForce(shootDirection, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamage>() != null)
        {
            other.GetComponent<IDamage>().takeDamage(shootDmg);
            Destroy(gameObject);
        }
    }
}
