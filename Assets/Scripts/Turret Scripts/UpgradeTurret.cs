using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UpgradeTurret : MonoBehaviour
{
    private void Update()
    {
        if (CompareTag("LevelOneTurret"))
        {
            if (Input.GetButtonDown("PlaceItem") && gameManager.instance.playerScript.playerBolts > gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice)
            {
                Instantiate(gameManager.instance.level2Turret, transform.parent.position, transform.parent.rotation);
                Destroy(transform.parent.gameObject);
            }
        }
        else if (CompareTag("LevelTwoTurret"))
        {
            if (Input.GetButtonDown("PlaceItem") && gameManager.instance.playerScript.playerBolts > gameManager.instance.rocketTurretPrice - gameManager.instance.level2TurretPrice)
            {
                Instantiate(gameManager.instance.rocketTurret, transform.parent.position, transform.parent.rotation);
                Destroy(transform.parent.gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player"))
        {
            if (CompareTag("LevelOneTurret"))
            {
                if (gameManager.instance.playerScript.playerBolts < gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice)
                {
                    gameManager.instance.upgradeTurretPrompt.text = "You need " + (gameManager.instance.level2TurretPrice - gameManager.instance.level1TurretPrice) + " more bolts to upgrade Turret";
                }
                else
                {
                    gameManager.instance.upgradeTurretPrompt.text = "Press E to Upgrade Turret";
                }
                gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(true);
            }
            else if (CompareTag("LevelTwoTurret"))
            {
                if (gameManager.instance.playerScript.playerBolts < gameManager.instance.rocketTurretPrice - gameManager.instance.level2TurretPrice)
                {
                    gameManager.instance.upgradeTurretPrompt.text = "You need " + (gameManager.instance.rocketTurretPrice - gameManager.instance.level2TurretPrice) + " more bolts to upgrade Turret";
                }
                else
                {
                    gameManager.instance.upgradeTurretPrompt.text = "Press E to Upgrade Turret";
                }
                gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.upgradeTurretPrompt.gameObject.SetActive(false);
        }
    }
}
