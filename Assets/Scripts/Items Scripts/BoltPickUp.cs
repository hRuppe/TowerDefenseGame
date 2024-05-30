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
            gameManager.instance.updateBoltAmount();

            if (gameManager.instance.playerScript.hasPickedUpBolt == false)
            {
                gameManager.instance.tutorialUI.text = "This is a bolt! You can use these to buy and upgrade turrets! Press B to open the Buy menu!";
                gameManager.instance.tutorialUI.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
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
        transform.Rotate(0, rotation * Time.deltaTime * rotationSpeed, 0);
        isRotating = false;
    }
}
