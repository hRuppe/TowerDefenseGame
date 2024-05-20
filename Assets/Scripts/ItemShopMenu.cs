using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopMenu : MonoBehaviour
{
    [SerializeField] GameObject turretMenu;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject barbedWireModel;
    [SerializeField] GameObject barbedWirePrefab;
    [SerializeField] int barbedWireCost;

    public void buyBarbedWire()
    {
        barbedWireModel.SetActive(true);
        gameManager.instance.shopMenu.gameObject.SetActive(false);
        gameManager.instance.playerScript.playerCurrency -= barbedWireCost;
        gameManager.instance.updateCurrency();
    }
    
    public void swapToTurretMenu()
    {
        turretMenu.SetActive(true);
        shopMenu.SetActive(false);
    }
    public void swapToShopMenu()
    {
        shopMenu.SetActive(true);
        turretMenu.SetActive(false);

        if(gameManager.instance.playerScript.hasPickedUpCoin)
        {
            gameManager.instance.tutorialUI.gameObject.SetActive(false);
            gameManager.instance.isPaused = false;
        }
    }
}
