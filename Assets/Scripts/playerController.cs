using JetBrains.Annotations;
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
    public string playerName = "Player One";
    public int playerHealth = 100;
    public ProgressBar Pb;
    public int playerCurrency;
    public int playerLevel = 1;
    public int playerExpPoints = 0;
    public int playerBolts;

    [Header("---- Weapon Stats ----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    public List<gunStats> gunList = new List<gunStats>();
    public List<ItemStats> itemList = new List<ItemStats>();
    public float scrollSensitivity = 1.0f; // Control the scroll wheel sensitivity 

    [Header("---- SFX ----")]
    [SerializeField] AudioSource sprintAudioSource;
    [SerializeField] AudioSource jumpAudioSource;
    [SerializeField] AudioClip[] jumpClips; 

    AudioSource walkingAudioSource; 
    private Vector3 playerVelocity;
    int jumpTimes;
    Vector3 moveDir;
    bool isSprinting = false;
    bool isShooting;
    bool isDashing = false;
    //bool isAiming = false;
    float speedOrig;
    //float counter = 0;
    //int HPorignal;
    int selectedGun;
    int expPtsToLvl = 100; 

    private void Start()
    {
        // Initalize the players speed
        speedOrig = playerSpeed;

        // Initialize player audio source
        walkingAudioSource = GetComponent<AudioSource>();
    }

    // Called once per frame
    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
        SwitchGun();
        pauseMenu();
        StartCoroutine(dash());
        menu();
        shop();
        placeTurret();
        UpdateProgressBar();
        GainExperience(playerExpPoints);
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
            PlayRandomJumpSound(); 
            jumpTimes++;
            playerVelocity.y = jumpHeight;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (controller.isGrounded && (horizontalInput != 0f || verticalInput != 0f))
        {
            if (isSprinting)
            {
                sprintAudioSource.enabled = true;
                walkingAudioSource.enabled = false;
            }
            else
            {
                walkingAudioSource.enabled = true;
                sprintAudioSource.enabled = false; 
            }
        }
        else
        {
            walkingAudioSource.enabled = false;
            sprintAudioSource.enabled = false;
        }
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

    IEnumerator dash()
    {
        float gravOrig = gravityValue;
        float prevSpeed = playerSpeed;

        //Checks if player is off the ground and checks for the dash key which is set to F right now
        if (playerVelocity.y > 0f && Input.GetButtonDown("Dash"))
        {
            float startTime = Time.time;

            //time minus the time that the dash started does not equal the amount of time we want the player to dash set in untiy then the player should dash in the air.
            while (Time.time - startTime < dashTime)
            {
                if (!isDashing)
                {
                    playerSpeed = 20;
                    playerVelocity.y = 0;
                    gravityValue = 0f;
                    isDashing = true;
                }
                yield return null;
            }
            //sets all stats back to orignal
            playerSpeed = prevSpeed;
            gravityValue = gravOrig;
            isDashing = false;
        }
    }

    // Player drop gun logic if the player leaves the collider it drops the weapon. 
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

    public void ItemPickup(ItemStats itemStats)
    {
        // If the item is something that does not need to be seen
        // newItem.GetComponent<Renderer>().enabled = false;
        // Store the item in the players inventory and store the item stats in the itemList
        itemList.Add(itemStats);
        // Add things later here to update the sound or the UI ECT
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
                if(hit.collider.tag != "Turret")
                {
                    Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
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
    }

    // Equipt weapon logic
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
    // Aim down sights logic
    //public void AimDownSights()
    //{
    //    if (Input.GetButtonDown("AimDownSights"))
    //    {
    //        // Reduce the players speed while aiming
    //        playerSpeed /= 2f;

    //        // Allow zooming in
    //        Camera.main.fieldOfView = 40f;

    //        // Moving the weapon position closer to the camera
    //        gunModel.transform.localPosition = new Vector3(0.5f, 0.5f, 1.0f);

    //        // Set aiming flag to true
    //        isAiming = true;

    //    }
    //    else if (Input.GetButtonUp("AimDownSights"))
    //    {
    //        // Restore the players speed
    //        playerSpeed = speedOrig;

    //        // Reset the camera
    //        Camera.main.fieldOfView = 60f;

    //        // Reset the weapon position to default
    //        gunModel.transform.localPosition = new Vector3(0f, 0f, 0f);

    //        // Set aiming flad to false
    //        isAiming = false;
    //    }
    //}
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
    public void placeTurret()
    {
        // checks for placeTurret button(E key) and checks to make sure that the turret is being displayed for placement so that the user can't just place turrets randomly
        if (Input.GetButtonDown("PlaceItem") && gameManager.instance.turretModels[gameManager.instance.turretIndex].activeSelf)
        {
            //gets the current turrentIndex that is set in the gamemanager when a player picks on a button in buy menu
            if (gameManager.instance.turretIndex == 0)
            {
                //Creates the basic turret that is set in the gameManager
                Instantiate(gameManager.instance.basicTurret, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.position, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.rotation);
                //Disables preview view for placing turret
                gameManager.instance.turretModels[gameManager.instance.turretIndex].SetActive(false);
            }
            else if (gameManager.instance.turretIndex == 1)
            {
                //Creates the level 2 turret that is set in the gameManager
                Instantiate(gameManager.instance.level2Turret, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.position, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.rotation);
                //Disables preview view for placing turret
                gameManager.instance.turretModels[gameManager.instance.turretIndex].SetActive(false);
            }
            else if (gameManager.instance.turretIndex == 2)
            {
                //Creates the level 2 turret that is set in the gameManager
                Instantiate(gameManager.instance.rocketTurret, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.position, gameManager.instance.turretModels[gameManager.instance.turretIndex].transform.rotation);
                //Disables preview view for placing turret
                gameManager.instance.turretModels[gameManager.instance.turretIndex].SetActive(false); 
            }
        }
    }
   
    // Logic for updating the progress bar
    public void UpdateProgressBar()
    {
        // Check if the progress bar refrence is not null and the player health is within range
        if(Pb != null && playerHealth >=0 && playerHealth <= 100){
            Pb.BarValue = playerHealth;
        }
    }

    // Logic for player currency
    public void IncreaseCurrency(int amount)
    {
        playerCurrency += amount;
        gameManager.instance.updateCurrency();
    }

    public void DecreaseCurrency(int amount)
    { 
        playerCurrency -= amount;
        gameManager.instance.updateCurrency();
    }

    public void GainExperience(int amount)
    {
        playerExpPoints += amount;

        // Ensure that the player's experience points are positive
        playerExpPoints = Mathf.Max(playerExpPoints, 0);

        // Calculate how many times the player has reached or exceeded 100 experience points
        int levelUps = playerExpPoints / 100;

        // Level up the player for each multiple of 100 experience points
        for (int i = 0; i < levelUps; i++)
        {
            LevelUp();
        }

        // Update the remaining experience points after leveling up
        playerExpPoints %= 100;
    }

    private void LevelUp()
    {
        playerLevel++;
        playerExpPoints -= expPtsToLvl;

        // Increase the amount the player needs next time to level up (can be modified)
        expPtsToLvl += 50;
    }

    public int GetCurrency()
    {
        return playerCurrency;
    }

    private void PlayRandomJumpSound()
    {
        int randIndex = Random.Range(0, jumpClips.Length);
        AudioClip selectedClip = jumpClips[randIndex];
        jumpAudioSource.clip = selectedClip;
        jumpAudioSource.Play();
    }
}
