using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    int shootDmg = 2;
    public Vector3 shootDirection;
    float timer;

    private void Start()
    {
        timer = Time.time;
    }
    private void Update()
    {
        GetComponent<Rigidbody>().AddForce(shootDirection, ForceMode.VelocityChange);
        if (Time.time - timer > 0.5f)
        {
            Destroy(gameObject);
        }
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
