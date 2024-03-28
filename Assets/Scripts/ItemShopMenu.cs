using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopMenu : MonoBehaviour
{
    [SerializeField] GameObject turretMenu;
    [SerializeField] GameObject shopMenu;
    
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
