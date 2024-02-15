using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("---- Player Stuff ----")]
    public GameObject player;
    public playerController playerScript;

    [Header("---- UI ----")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
    public GameObject playerDamageScreen;

    public TextMeshProUGUI enemiesLeft;

    public GameObject spawnPos;
    public int enemiesToKill;

    public bool isPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("EnemyLoc");
        spawnPos = GameObject.FindGameObjectWithTag("Spawn Pos");
        playerScript.GetComponent<CharacterController>();
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
        else
        {
            unPause();
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
