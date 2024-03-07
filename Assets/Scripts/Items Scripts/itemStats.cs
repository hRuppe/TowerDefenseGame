using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class itemStats : ScriptableObject
{
    public string itemName;
    public int itemID; 
    public float healingAmount;
    public int despawTimer;
    public int itemCount;
    public GameObject itemModel;
    public GameObject itemEffect;
    public AudioClip itemSound;
}
