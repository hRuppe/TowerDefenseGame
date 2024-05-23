using UnityEngine;

[CreateAssetMenu]
public class ItemStats : ScriptableObject
{
    public string itemName;
    public float healthAmount;
    public int itemCount = 1;
    public int boltCurrency;
    public int currencyAmount;
    public int itemPrice;
}
