using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;

    [Header("---- Player Stats ----")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpCount;
    [SerializeField] float sprintMod;
    [SerializeField] int dashMod;
    [SerializeField] float dashTime;

    [Header("---- Weapon Stats ----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    public List<gunStats> gunList = new List<gunStats>();
    public List<itemStats> itemList = new List<itemStats>();
    
    public float scrollSensitivity = 1.0f; // Control the scroll wheel sensitivity 


    private Vector3 playerVelocity;
    int jumpTimes;
    Vector3 moveDir;
    bool isSprinting = false;
    bool isShooting;
    bool isDashing = false;
    bool isAiming = false;
    float speedOrig;
    float counter = 0;
    int HPorignal;
    int selectedGun;

    private void Start()
    {
        // Initalize the players speed
        speedOrig = playerSpeed;
    }

    // Called once per frame
    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
        SwitchGun();
        deployTower();
        Interact();
        pauseMenu();
        menu();
        shop();
        character();
       /* AimDownSights();*/
    }

    // Player movement logic
    public void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpTimes = 0;
            playerVelocity.y = 0f;
        }

        moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpTimes < jumpCount)
        {
            jumpTimes++;
            playerVelocity.y = jumpHeight;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Player sprint logic
   public void sprint()
    {
        if (!isSprinting && Input.GetButtonDown("Sprint") && playerVelocity.y <= 0)
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (isSprinting && Input.GetButtonUp("Sprint"))
        {
            playerSpeed = speedOrig;
            isSprinting = false;
        }
    }

    // Player gun pick up logic
    public void gunPickup(gunStats gunStat)
    {
            // Instantiate the gun model at the player's position
            GameObject newGun = Instantiate(gunStat.gunModel, transform.position, Quaternion.identity);

            // Parent the gun to the player's hand 
            newGun.transform.parent = gunModel.transform;

            // Set the gun's local position and rotation relative to the player's hand 
            newGun.transform.localPosition = Vector3.zero;
            newGun.transform.localRotation = Quaternion.identity;

            shootRate = gunStat.shootRate;
            shootDist = gunStat.shootDist;
            shootDmg = gunStat.shootDmg;
            hitEffect = gunStat.hitEffect;

        // Stores the gun in the players inventory and stores the gun stats in the gunList
        gunList.Add(gunStat);
        InstantiateGunModel(gunStat);
    }

    // Shooting logic
    IEnumerator shoot()
    {
        if (gunList.Count > 0 && isShooting == false && Input.GetButton("Shoot"))
        {
            isShooting = true;

            // Get the direction the player is aiming
            Vector3 shootingDirection = GetAimingDirection();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, shootingDirection, out hit, shootDist))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDmg);
                }

                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    // Get the aiming direction based on the center of the screen
    Vector3 GetAimingDirection()
    {
        // Get the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Cast a ray from the camera through the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // Return the direction of the ray
        return ray.direction;
    }


    // Instantiate the gun model in the scene
    void InstantiateGunModel(gunStats gunStat)
    {
        // Instantiate the gun model at the player's position
        GameObject newGun = Instantiate(gunStat.gunModel, transform.position, Quaternion.identity);

        // Parent the gun to the player's hand 
        newGun.transform.parent = gunModel.transform;

        // Set the gun's local position and rotation relative to the player's hand 
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
    }

    // Switch Weapons logic
    public void SwitchGun()
    {
        // Define scroll wheel sensitivity
        float scrollSensitivity = 0.1f; // Adjust this value to change sensitivity

        // Get scroll input
        float scrollInput = Input.GetAxis("MouseScrollWheel");

        // Apply sensitivity
        float adjustedScrollInput = scrollInput * scrollSensitivity;

        if (gunList.Count > 1)
        {
            // Cycle to the next gun in the list
            if (adjustedScrollInput > 0)
            {
                selectedGun = (selectedGun + 1) % gunList.Count;

            }
            // Cycle to the previous gun in the list
            else if (adjustedScrollInput < 0)
            {
                selectedGun = (selectedGun - 1 + gunList.Count) % gunList.Count;
            }

            EquipGun(selectedGun);

        }
    /*    else if (gunList.Count == 1)
        {
            // There's only one gun in the inventory
            Debug.Log("Only one gun in inventory.");
        }
        else
        {
            // There are no weapons in the inventory
            Debug.Log("No weapons in inventory");
        }*/
    }

    public void EquipGun(int index)
    {
        // Check if the index is valid
        if (index >= 0 && index < gunList.Count)
        {
            // Deactivate the current gun model if it exists
            if (gunModel != null)
            {
                Destroy(gunModel); // Destroy the current gun model
            }

            // Get the selected gun's stats
            gunStats selectedGunStats = gunList[index];

            // Instantiate the new gun model
            gunModel = Instantiate(selectedGunStats.gunModel, transform.position, Quaternion.identity);

            // Set the new gun model's parent to the player to follow its movements
            gunModel.transform.parent = transform;

            // Set the gun's local position and rotation relative to the player's hand
            gunModel.transform.localPosition = Vector3.zero; 
            gunModel.transform.localRotation = Quaternion.identity; 

            // Update the shoot stats
            shootRate = selectedGunStats.shootRate;
            shootDist = selectedGunStats.shootDist;
            shootDmg = selectedGunStats.shootDmg;
            hitEffect = selectedGunStats.hitEffect;
        }
        else
        {
            Debug.Log("Invalid index for equipping gun");
        }
    }

    public void ItemPickup(itemStats itemStat)
    {
        // Check to see if the index is valid
        // Get the selected item status
        // update the item stats
    }


    // Aim down sights logic
    public void AimDownSights()
    {
        if (Input.GetButtonDown("AimDownSights"))
        {
            // Reduce the players speed while aiming
            playerSpeed /= 2f;

            // Allow zooming in
            Camera.main.fieldOfView = 40f;

            // Moving the weapon position closer to the camera
            gunModel.transform.localPosition = new Vector3(0.5f, 0.5f, 1.0f);

            // Set aiming flag to true
            isAiming = true;

        }
        else if (Input.GetButtonUp("AimDownSights"))
        {
            // Restore the players speed
            playerSpeed = speedOrig;

            // Reset the camera
            Camera.main.fieldOfView = 60f;

            // Reset the weapon position to default
            gunModel.transform.localPosition = new Vector3(0f, 0f, 0f);

            // Set aiming flad to false
            isAiming = false;
        }
    }

   

   public void deployTower()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Instantiate a tower object at the players position
            // Instantiate(deployPosition, transform.position, Quaternion.identity);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // If a player holds the E button down then they can upgrade the tower
            //UpgradeTower();
        }
    }

   public void Interact()
    {
        // Interact logic
    }

    public void pauseMenu()
    {
        // Pause menu logic
    }

    public void menu()
    {
        // Menu logic
    }

   public void shop()
    {
        // Shop logic
    }

   public void character()
    {
        // Character logic
    }

   public void respawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPos.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
}
