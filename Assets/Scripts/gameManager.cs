using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] Button Level2TurretButton;
    public GameObject basicTurret;
    public GameObject level2Turret;
    public GameObject playerDeadMenu;
    public GameObject playerDamageScreen;

    public TextMeshProUGUI enemiesLeft;

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
        basicTurretButton.onClick.AddListener(spawnBasicTurret);

        Level2TurretButton.onClick.AddListener(spawnLevelTwoTurret);
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
        BuyMenu.SetActive(false);
        turretIndex = 0;
        turretModels[0].SetActive(true);
    }
    void spawnLevelTwoTurret()
    {
        BuyMenu.SetActive(false);
        turretIndex = 1;
        turretModels[1].SetActive(true);
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
}
