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

    void Update()
    {
        if (barbedWireModel.activeSelf && Input.GetButtonDown("PlaceItem"))
        {
            Instantiate(barbedWirePrefab, barbedWireModel.transform.position, barbedWireModel.transform.rotation);
            barbedWireModel.SetActive(false);
            gameManager.instance.shopMenu.GetComponent<Canvas>().enabled = true;
            gameManager.instance.shopMenu.SetActive(false);
        }
    }
    public void buyBarbedWire()
    {
        barbedWireModel.SetActive(true);
        gameManager.instance.shopMenu.GetComponent<Canvas>().enabled = false;
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
    }
}
