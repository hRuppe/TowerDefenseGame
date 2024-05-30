using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("---- Game Settings ----")]
    [SerializeField] int defensiveScoreToProgress;

    [Header("---- Player Stuff ----")]
    public GameObject player;
    public playerController playerScript;

    [Header("---- UI Elements ----")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject shopMenu;
    [SerializeField] Button basicTurretButton;
    [SerializeField] Button level2TurretButton;
    [SerializeField] Button rocketTurretButton;
    public int level1TurretPrice;
    public int level2TurretPrice;
    public int rocketTurretPrice;
    public GameObject basicTurret;
    public GameObject level2Turret;
    public GameObject rocketTurret;
    public GameObject playerDeadMenu;
    public GameObject playerDamageScreen;
    public Slider defensiveLocationHealthBar;
    public GameObject noteObject;
    public TMP_Text noteText;
    public TMP_Text readNotePrompt;
    public TMP_Text upgradeTurretPrompt;
    public TMP_Text playerNameUI;
    public TMP_Text tutorialUI;

    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI currency;
    public TextMeshProUGUI shopCurrency;
    public TextMeshProUGUI defensiveScoreUI;
    public cameraController cameraController;

    [HideInInspector] public int enemiesToKill;
    [HideInInspector] public int defensiveScore;
    public List<GameObject> turretModels;
    [HideInInspector] public int turretIndex;

    [HideInInspector] public bool isPaused = false;
    bool spawnedPortal = false;

    [Header("---- Other ----")]
    [SerializeField] GameObject portalPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        playerScript.GetComponent<CharacterController>();
    }

    private void Start()
    {
        updateUI();

        //checks for button clicks on buy menu first is for the basic turrets and the second is for the leveled up turret

        updateCurrency();

        // Check for player name
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerNameUI.text = PlayerPrefs.GetString("PlayerName");
        }
      

    }

    // Update is called once per frame
    void Update()
    {
        // Check for Cancel key to toggle pause/unpause
        if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            TogglePauseMenu();
        }
        //Checks for input of menu button(Currently set to M) and checks no other menu screens are open
        if (Input.GetButtonDown("Menu") && !playerDeadMenu.activeSelf && !winMenu.activeSelf && !turretModels[0].activeSelf && !turretModels[1].activeSelf && !turretModels[2].activeSelf)
        {
            if (shopMenu.activeSelf)
            {
                shopMenu.SetActive(false);
            }
            else
            {
                Cursor.visible = true;
                shopMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        // Checks to see if player has hit required defensive score to progress
        CheckDefensiveScore();
        turretButtons();
    }
    void turretButtons()
    {
        if (gameManager.instance.playerScript.playerBolts >= level1TurretPrice)
        {
            basicTurretButton.interactable = true;
        }
        else
        {
            basicTurretButton.interactable = false;
        }

        if (gameManager.instance.playerScript.playerBolts >= level2TurretPrice)
        {
            level2TurretButton.interactable = true;
        }
        else
        {
            level2TurretButton.interactable = false;
        }
        if (gameManager.instance.playerScript.playerBolts >= rocketTurretPrice)
        {
            rocketTurretButton.interactable = true;
        }
        else
        {
            rocketTurretButton.interactable = false;
        }
    }
    public void spawnBasicTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= level1TurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= level1TurretPrice;
            //turns off buy menu so player can no longer see it
            shopMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 0;
            //makes sure the correct turret is displayed for placement
            turretModels[turretIndex].SetActive(true);
        }
    }
    public void spawnLevelTwoTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= level2TurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= level2TurretPrice;
            //turns off shopMenu so player can no longer see it
            shopMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 1;
            //makes sure the correct turret is displayed for placement
            turretModels[turretIndex].SetActive(true);
        }
    }
    public void spawnRocketTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= rocketTurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= rocketTurretPrice;
            //turns off shopMenu so player can no longer see it
            shopMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 2;
            //makes sure the correct turret is displayed for placement
            turretModels[turretIndex].SetActive(true);
        }
    }

    // New method to toggle the pause menu
    // New method to toggle the pause menu
    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            pause();
            LockCursor();
            BlockInputDuringPause(); // Call the method to block input during pause
            cameraController.enabled = false; // Disable camera controller script
            Debug.Log("Cursor locked and input blocked during pause");
        }
        else
        {
            unPause();
            UnlockCursor();
            // Re-enable player and camera movement scripts
            playerScript.enabled = true;
            cameraController.enabled = true;
            Debug.Log("Cursor unlocked and input restored after pause");
        }
    }


    // Method to lock cursor movement
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Method to unlock cursor movement
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void BlockInputDuringPause()
    {
        // Lock the cursor
        LockCursor();

        // Debug statement to check playerScript
        if (playerScript == null)
        {
            Debug.LogError("playerScript is null!");
        }

        // Debug statement to check cameraControllerScript
        if (cameraController == null)
        {
            Debug.LogError("cameraControllerScript is null!");
        }

        // Disable player movement scripts or components if playerScript is not null
        if (playerScript != null)
        {
            playerScript.enabled = false;
        }

        // Disable camera movement scripts or components if cameraControllerScript is not null
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        // Disable any other scripts or components that handle input or movement
        // You may need to adjust this based on your specific setup
    }


    public void pause()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void unPause()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }
    public IEnumerator playerDamageFlash()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        playerDamageScreen.SetActive(false);
    }

    public void youWin()
    {
        Cursor.visible = true;
        winMenu.SetActive(true);

    }

    public void YouLose()
    {
        pause();
        Cursor.visible = true;
        loseMenu.SetActive(true);
    }

    public void updateEnemyNumber()
    {
        enemiesToKill--;

        updateUI();

        if (enemiesToKill <= 0)
        {
            youWin();
        }
    }

    public void updateUI()
    {
        enemiesLeft.text = enemiesToKill.ToString("F0");
        defensiveScoreUI.text = "Defensive Score " + defensiveScore;
        defensiveScoreUI.text = "Defensive Score " + defensiveScore + "/" + defensiveScoreToProgress;
    }

    public void updateCurrency()
    {
        // Update the currency in the upper left side of the screen
        currency.text = playerScript.playerCurrency.ToString("F0");

        //Updates the currency in the shop
        shopCurrency.text = '$' + playerScript.playerCurrency.ToString("F0");
    }

    private void CheckDefensiveScore()
    {
        if (defensiveScore >= defensiveScoreToProgress)
        {
            StartCoroutine(SpawnPortal());
        }
    }

    IEnumerator SpawnPortal()
    {
        if (!spawnedPortal)
        {
            spawnedPortal = true;
            yield return new WaitForSeconds(3f);
            Vector3 portalSpawnPos = player.transform.position + player.transform.forward * 10f;
            portalPrefab.GetComponent<ParticleSystem>().Play();
            Instantiate(portalPrefab, portalSpawnPos, player.transform.rotation);
        }
    }


}


