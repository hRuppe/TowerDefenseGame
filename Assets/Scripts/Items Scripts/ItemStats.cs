using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemStats : ScriptableObject
{
    public string itemId;
    public float healthAmount;
    public int itemCount;
    public int upgradeAmount;
    public int currencyAmount;
    public int itemPrice;
}
