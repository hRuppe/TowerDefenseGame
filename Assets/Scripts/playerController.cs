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
    [SerializeField] int sprintMod;
    [SerializeField] int dashMod;
    [SerializeField] float dashTime;

    [Header("---- Weapon Stats ----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    public List<gunStats> gunList = new List<gunStats>();
    public List<GameObject> normalStorage = new List<GameObject>();
    public List<GameObject> gunInventory = new List<GameObject>();



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

        // Player dashing
        if (playerVelocity.y > 0f && Input.GetButtonDown("Dash"))//Dash is set to F for now
        {
            StartCoroutine(dash());
            counter = 0;
        }

        StartCoroutine(shoot());
        aimDownSights();
        switchWeapons();
        deployTower();
        Interact();
        pauseMenu();
        menu();
        shop();
        character();
    }

    // Player movement logic
    void movement()
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
    void sprint()
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

    // Player dashing logic
    IEnumerator dash()
    {
        float gravOrig = gravityValue;
        float prevSpeed = playerSpeed;
        float startTime = Time.time;

        while (Time.time - startTime <= dashTime)
        {
            if (!isDashing)
            {
                playerSpeed *= dashMod;
                playerVelocity.y = 0;
                gravityValue = 0f;
                isDashing = true;
            }
            yield return null;
        }

        playerSpeed = prevSpeed;
        gravityValue = gravOrig;
        isDashing = false;
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
            gunInventory.Add(newGun);
        gunList.Add(gunStat);
    }

    // Player drop gun logic
    public void gunDrop()
    {
        shootRate = 0;
        shootDist = 0;
        shootDmg = 0;
        hitEffect = null;

        gunModel.GetComponent<MeshFilter>().sharedMesh = null;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = null;

        gunList.Clear();
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

    // Aim down sights logic
    void aimDownSights()
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

    // Switch Weapons logic
    void switchWeapons()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Cycle to the next gun in the list (scrolling up)
        if (scrollInput > 0f)
        {
            selectedGun = (selectedGun + 1) % gunList.Count;
        }
        // Cycle to the previous gun in the list (scrolling down)
        else if (scrollInput < 0f && selectedGun > 0)
        {
            selectedGun = (selectedGun - 1 + gunList.Count) % gunList.Count;
        }

        // Change the weapon model or properties based on the selected weapon
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        shootRate = gunList[selectedGun].shootRate;
        shootDist = gunList[selectedGun].shootDist;
        shootDmg = gunList[selectedGun].shootDmg;
        hitEffect = gunList[selectedGun].hitEffect;
    }

    void deployTower()
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

    void Interact()
    {
        // Interact logic
    }

    void pauseMenu()
    {
        // Pause menu logic
    }

    void menu()
    {
        // Menu logic
    }

    void shop()
    {
        // Shop logic
    }

    void character()
    {
        // Character logic
    }

    void respawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPos.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
}
