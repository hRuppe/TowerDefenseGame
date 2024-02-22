using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class itemStats : MonoBehaviour
{
    public string itemName;
    public int itemID; 
    public float healingAmount;
    public int despawTimer;
    public int itemCount;
    public GameObject itemEffect;
    public AudioClip itemSound;
}
