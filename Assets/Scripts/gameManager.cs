using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("---- Player Stuff ----")]
    public GameObject player;
    public playerController playerScript;

    [Header("---- UI ----")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject BuyMenu;
    [SerializeField] Button basicTurretButton;
    [SerializeField] Button level2TurretButton;
    [SerializeField] Button rocketTurretButton;
    [SerializeField] TMP_Text needMoreBolts;
    [SerializeField] TMP_Text needMoreBolts2;
    [SerializeField] TMP_Text needMoreBolts3;
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


    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI currency;
    public TextMeshProUGUI defensiveScoreUI;

    public GameObject spawnPos;
    [HideInInspector] public int enemiesToKill;
    public int defensiveScore;
    public List<GameObject> turretModels;
    [HideInInspector] public int turretIndex;

    [HideInInspector] public bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        spawnPos = GameObject.FindGameObjectWithTag("Spawn Pos");
        playerScript.GetComponent<CharacterController>();
    }

    private void Start()
    {
        updateCurrency();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            if(BuyMenu.activeSelf)
            {
                BuyMenu.SetActive(false);
                unPause();
            }
            else
            {
                isPaused = !isPaused;
                pauseMenu.SetActive(isPaused);
            } 
        }
        if (isPaused)
        {
            pause();
        }
        else if (!isPaused && !BuyMenu.activeSelf)
        {
            unPause();
        }
        //Checks for input of menu button(Currently set to M) and checks no other menu screens are open
        if (Input.GetButtonDown("Menu") && !playerDeadMenu.activeSelf && !winMenu.activeSelf && !turretModels[0].activeSelf && !turretModels[1].activeSelf && !turretModels[2].activeSelf)
        {
            if (BuyMenu.activeSelf)
            {
                BuyMenu.SetActive(false);
            }
            else
            {
                Cursor.visible = true;
                BuyMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        turretButtons();
    }
    void turretButtons()
    {
        if (gameManager.instance.playerScript.playerBolts >= level1TurretPrice)
        {
            basicTurretButton.interactable = true;
            needMoreBolts.gameObject.SetActive(false);
        }
        else
        {
            basicTurretButton.interactable = false;
            needMoreBolts.SetText("Need " + (level1TurretPrice - gameManager.instance.playerScript.playerBolts).ToString() + " more bolts");
            needMoreBolts.gameObject.SetActive(true);
        }

        if (gameManager.instance.playerScript.playerBolts >= level2TurretPrice)
        {
            level2TurretButton.interactable = true;
            needMoreBolts2.gameObject.SetActive(false);
        }
        else
        {
            level2TurretButton.interactable = false;
            needMoreBolts2.SetText("Need " + (level2TurretPrice - gameManager.instance.playerScript.playerBolts).ToString() + " more bolts");
            needMoreBolts2.gameObject.SetActive(true);
        }
        if (gameManager.instance.playerScript.playerBolts >= rocketTurretPrice)
        {
            rocketTurretButton.interactable = true;
            needMoreBolts3.gameObject.SetActive(false);
        }
        else
        {
            rocketTurretButton.interactable = false;
            needMoreBolts3.SetText("Need " + (rocketTurretPrice - gameManager.instance.playerScript.playerBolts).ToString() + " more bolts");
            needMoreBolts3.gameObject.SetActive(true);
        }
    }
    public void spawnBasicTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= level1TurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= level1TurretPrice;
            //turns off buy menu so player can no longer see it
            BuyMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 0;
            //makes sure the correct turret is displayed for placement
            turretModels[0].SetActive(true);
        }
    }
    public void spawnLevelTwoTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= level2TurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= level2TurretPrice;
            //turns off buymenu so player can no longer see it
            BuyMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 1;
            //makes sure the correct turret is displayed for placement
            turretModels[1].SetActive(true);
        }
    }
    public void spawnRocketTurret()
    {
        if (gameManager.instance.playerScript.playerBolts >= rocketTurretPrice)
        {
            gameManager.instance.playerScript.playerBolts -= rocketTurretPrice;
            //turns off buymenu so player can no longer see it
            BuyMenu.SetActive(false);
            //sets the turrent index that is used in the playercontroller so that the correct turret is placed
            turretIndex = 2;
            //makes sure the correct turret is displayed for placement
            turretModels[2].SetActive(true);
        }
    }
    public void pause()
    {
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void unPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public IEnumerator playerDamageFlash()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        playerDamageScreen.SetActive(false);
    }

    public void youWin()
    {
        winMenu.SetActive(true);
        isPaused = true;
    }

    public void YouLose()
    {
        loseMenu.SetActive(true);
        isPaused = true;
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
    }

    public void updateCurrency()
    {
        // Update the currency in the upper left side of the screen
        currency.text = playerScript.playerCurrency.ToString("F0");

    }
}
