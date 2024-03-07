using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.Experimental.GraphView;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("---- Player Stuff ----")]
    public GameObject player;
    public playerController playerScript;

    [Header("---- UI ----")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject BuyMenu;
    [SerializeField] Button basicTurretButton;
    [SerializeField] Button level2TurretButton;
    [SerializeField] Button rocketTurretButton;
    public GameObject basicTurret;
    public GameObject level2Turret;
    public GameObject rocketTurret;
    public GameObject playerDeadMenu;
    public GameObject playerDamageScreen;

    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI currency;

    public GameObject spawnPos;
    public int enemiesToKill;
    public List<GameObject> turretModels;
    public int turretIndex;

    public bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        spawnPos = GameObject.FindGameObjectWithTag("Spawn Pos");
        playerScript.GetComponent<CharacterController>();
    }

    private void Start()
    {
        //checks for button clicks on buy menu first is for the basic turrets and the second is for the leveled up turret
        basicTurretButton.onClick.AddListener(spawnBasicTurret);
        level2TurretButton.onClick.AddListener(spawnLevelTwoTurret);
        rocketTurretButton.onClick.AddListener(spawnRocketTurret);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
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
        if (Input.GetButtonDown("Menu") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            Cursor.visible = true;
            BuyMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    void spawnBasicTurret()
    {
        //turns off buymenu so player can no longer see it
        BuyMenu.SetActive(false);
        //sets the turrent index that is used in the playercontroller so that the correct turret is placed
        turretIndex = 0;
        //makes sure the correct turret is displayed for placement
        turretModels[0].SetActive(true);
    }
    void spawnLevelTwoTurret()
    {
        //turns off buymenu so player can no longer see it
        BuyMenu.SetActive(false);
        //sets the turrent index that is used in the playercontroller so that the correct turret is placed
        turretIndex = 1;
        //makes sure the correct turret is displayed for placement
        turretModels[1].SetActive(true);
    }
    void spawnRocketTurret()
    {
        //turns off buymenu so player can no longer see it
        BuyMenu.SetActive(false);
        //sets the turrent index that is used in the playercontroller so that the correct turret is placed
        turretIndex = 2;
        //makes sure the correct turret is displayed for placement
        turretModels[2].SetActive(true);
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
        yield return new WaitForSeconds(0.1f);
        playerDamageScreen.SetActive(false);
    }

    public void youWin()
    {
        winMenu.SetActive(true);
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
    }

    public void updateCurrency()
    {
        // Update the currency in the upper left side of the screen
        currency.text = playerScript.playerCurrency.ToString("F0");

    }
}
